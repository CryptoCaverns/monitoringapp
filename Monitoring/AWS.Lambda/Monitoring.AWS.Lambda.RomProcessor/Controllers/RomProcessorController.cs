using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using Monitoring.Infrastructure.MongoDB;
using Monitoring.Infrastructure.MongoDB.Documents;
using Monitoring.Infrastructure.RomEditor;

namespace Monitoring.AWS.Lambda.RomProcessor.Controllers
{
    [Route("api/romprocessor")]
    public class RomProcessorController : Controller
    {
        IAmazonS3 S3Client { get; set; }
        string BucketName { get; set; }
        public MongoRepository MongoRepository { get; set; }

        public RomProcessorController(IConfiguration configuration, IAmazonS3 s3Client)
        {
            S3Client = s3Client;
            MongoRepository = new MongoRepository();

            BucketName = configuration[Startup.AppS3BucketKey];
            if (string.IsNullOrEmpty(BucketName))
            {
                LambdaLogger.Log("Missing configuration for S3 bucket. The RomRegister configuration must be set to a S3 bucket.");
                throw new Exception("Missing configuration for S3 bucket. The RomRegister configuration must be set to a S3 bucket.");
            }

            LambdaLogger.Log($"Configured to use bucket {BucketName}");
        }

        [HttpPost("rigId")]
        public async Task<IActionResult> Post(string rigId)
        {
            // Copy the request body into a seekable stream required by the AWS SDK for .NET.
            var seekableStream = new MemoryStream();
            await Request.Body.CopyToAsync(seekableStream);
            seekableStream.Position = 0;

            var originRom = seekableStream.ToArray();
            var originHash = originRom.GetHashCode();

            LambdaLogger.Log($"Length of rom file: {seekableStream.Length.ToString()}");                       

            // open bios file
            var biosEditor = new BiosEditor();

            try
            {
                biosEditor.Open(seekableStream);
                LambdaLogger.Log($"SysLabel for rom - {biosEditor.BiosBootUpMessage}");
            }
            catch(Exception ex)
            {
                var putOriginFileRequest = new PutObjectRequest
                {
                    BucketName = BucketName,
                    Key = $"errors/{originHash}-{DateTime.UtcNow.Ticks}.rom",
                    InputStream = new MemoryStream(originRom)
                };
                await S3Client.PutObjectAsync(putOriginFileRequest);
                               
                LambdaLogger.Log($"Error opening rom file. {ex}");
            }
                      
            if (CheckIfRegister(biosEditor.BiosBootUpMessage))
            {
                // already registered - just return current version from s3
                string key = $"bios/{biosEditor.BiosBootUpMessage}-current.rom";

                try
                {
                    var getRequest = new GetObjectMetadataRequest
                    {
                        BucketName = BucketName,
                        Key = key,
                    };

                    var response = await S3Client.GetObjectMetadataAsync(getRequest);

                    if (response.HttpStatusCode == HttpStatusCode.OK)
                    {
                        return base.Ok(GenerateTempUrl(key));
                    }

                    return base.NotFound();
                }
                catch (AmazonS3Exception e)
                {
                    LambdaLogger.Log(e.Message);
                    return base.StatusCode((int) e.StatusCode, e.Message);
                }
            }
            else
            {
                // register bios in system
                var name = Guid.NewGuid().ToString();
                LambdaLogger.Log($"New name {name}");
                biosEditor.BiosBootUpMessage = name;
                var outputStream = biosEditor.Save();
                                
                try
                {
                    // save record to mongo
                    RegisterBios(name, outputStream.ToArray().GetHashCode(), originHash);
                    // save miner unit to mongo 
                    RegisterMinerUnit(name, rigId, biosEditor);

                    var putRequest = new PutObjectRequest
                    {
                        BucketName = BucketName,
                        Key = $"bios/{name}-original.rom",
                        InputStream = new MemoryStream(originRom)
                    };

                    var response = await S3Client.PutObjectAsync(putRequest);
                    LambdaLogger.Log($"Uploaded object bios/{name}-original.rom to bucket {BucketName}. Request Id: {response.ResponseMetadata.RequestId}");

                    putRequest = new PutObjectRequest
                    {
                        BucketName = BucketName,
                        Key = $"bios/{name}-current.rom",
                        InputStream = outputStream
                    };

                    response = await S3Client.PutObjectAsync(putRequest);
                    LambdaLogger.Log($"Uploaded object bios/{name}-current.rom to bucket {BucketName}. Request Id: {response.ResponseMetadata.RequestId}");
                    
                    return base.Ok(GenerateTempUrl($"bios/{biosEditor.BiosBootUpMessage}-current.rom"));
                }
                catch (AmazonS3Exception e)
                {
                    LambdaLogger.Log(e.Message);
                    return base.StatusCode((int)e.StatusCode, e.Message);
                }

            }
        }

        #region private methods

        private bool CheckIfRegister(string name)
        {
            return MongoRepository.GetBios().Find(x => x.SysLabel == name).Any();
        }

        private void RegisterBios(string name, int currentHash, int originalHash)
        {
            MongoRepository.GetBios().InsertOne(new BiosDocument
            {
                SysLabel = name,
                CurrentHash = currentHash,
                OriginalHash = originalHash,
                Timestamp = DateTime.Now,
                Id = ObjectId.GenerateNewId(DateTime.Now)
            });
            }

        private void RegisterMinerUnit(string name, string rigId, BiosEditor editor)
        {
            MongoRepository.GetMinerUnits().InsertOne(new MinerUnitDocument()
            {
                Id = ObjectId.GenerateNewId(DateTime.Now),
                RigId = rigId,
                SysLabel = name,
                CreatedTimestamp = DateTime.Now,
                MemorySpeed = editor.MemorySpeed.ToList(),
                ClockSpeed = editor.ClockSpeed.ToList(),
                PowerTune = editor.PowerTune,
            });
        }

        private string GenerateTempUrl(string key)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = BucketName,
                Key = key,
                Expires = DateTime.Now.AddMinutes(5)
            };
            return S3Client.GetPreSignedURL(request);
        }

        #endregion
    }
}