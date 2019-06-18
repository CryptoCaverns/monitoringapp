using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Monitoring.Infrastructure.RomEditor;

namespace Monitoring.AWS.Lambda.RomProcessor.Controllers
{
    [Route("api/[controller]")]
    public class RomRegisterController : Controller
    {
        IAmazonS3 S3Client { get; set; }
        ILogger Logger { get; set; }
        string BucketName { get; set; }

        public RomRegisterController(IConfiguration configuration, ILogger<RomRegisterController> logger, IAmazonS3 s3Client)
        {
            Logger = logger;
            S3Client = s3Client;

            BucketName = configuration[Startup.AppS3BucketKey];
            if (string.IsNullOrEmpty(BucketName))
            {
                logger.LogCritical("Missing configuration for S3 bucket. The RomRegister configuration must be set to a S3 bucket.");
                throw new Exception("Missing configuration for S3 bucket. The RomRegister configuration must be set to a S3 bucket.");
            }

            logger.LogInformation($"Configured to use bucket {BucketName}");
        }

        [HttpPut("")]
        public async Task Put()
        {
            // Copy the request body into a seekable stream required by the AWS SDK for .NET.
            var seekableStream = new MemoryStream();
            await Request.Body.CopyToAsync(seekableStream);
            seekableStream.Position = 0;
            
            var biosEditor = new BiosEditor();
            biosEditor.Open(seekableStream);

            var name = Guid.NewGuid().ToString();
            Logger.LogInformation($"New name {name}");
            biosEditor.BiosBootUpMessage = name;

            seekableStream = new MemoryStream(seekableStream.ToArray());
            var putRequest = new PutObjectRequest
            {
                BucketName = BucketName,
                Key = $"bios/{name}-original.rom",
                InputStream = seekableStream
            };

            try
            {
                var response = await S3Client.PutObjectAsync(putRequest);
                Logger.LogInformation($"Uploaded object bios/{name}-original.rom to bucket {BucketName}. Request Id: {response.ResponseMetadata.RequestId}");
            }
            catch (AmazonS3Exception e)
            {
                Response.StatusCode = (int)e.StatusCode;
                var writer = new StreamWriter(Response.Body);
                writer.Write(e.Message);
            }
            catch (Exception e)
            {
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                var writer = new StreamWriter(Response.Body);
                writer.Write(e.Message);
            }
        }
    }
}