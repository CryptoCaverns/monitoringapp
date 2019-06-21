using System;
using System.Buffers.Text;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
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
        ILambdaLogger Logger { get; set; }
        string BucketName { get; set; }

        public RomRegisterController(IConfiguration configuration, IAmazonS3 s3Client)
        {
            S3Client = s3Client;

            BucketName = configuration[Startup.AppS3BucketKey];
            if (string.IsNullOrEmpty(BucketName))
            {
                LambdaLogger.Log("Missing configuration for S3 bucket. The RomRegister configuration must be set to a S3 bucket.");
                throw new Exception("Missing configuration for S3 bucket. The RomRegister configuration must be set to a S3 bucket.");
            }

            LambdaLogger.Log($"Configured to use bucket {BucketName}");
        }

        public static object DeserializeFromStream(MemoryStream stream)
        {
            IFormatter formatter = new BinaryFormatter();
            stream.Seek(0, SeekOrigin.Begin);
            object objectType = formatter.Deserialize(stream);
            return objectType;
        }

        [HttpPut("")]
        public async Task<string> Put()
        {
            // Copy the request body into a seekable stream required by the AWS SDK for .NET.
            var seekableStream = new MemoryStream();
            await Request.Body.CopyToAsync(seekableStream);
            seekableStream.Position = 0;
            
            try
            {
                return Encoding.UTF8.GetString(seekableStream.ToArray()).Length.ToString();
            }
            catch (Exception e)
            {
                return e.Message;
            }


            //Response.ContentType = "application/octet-stream";
            //    Response.Headers.Add("Content-Disposition", $"attachment;current.rom");
            //    new MemoryStream(seekableStream.ToArray()).CopyTo(Response.Body);

            var biosEditor = new BiosEditor();
                //biosEditor.Open(seekableStream);
            

            //var name = Guid.NewGuid().ToString();
            //LambdaLogger.Log($"New name {name}");
            //biosEditor.BiosBootUpMessage = name;
            //var outputStream = biosEditor.Save();

            //var putRequest = new PutObjectRequest
            //{
            //    BucketName = BucketName,
            //    Key = $"bios/{biosEditor.BiosBootUpMessage}-original.rom",
            //    InputStream = new MemoryStream(seekableStream.ToArray())
            //};

            //try
            //{
            //    var response = await S3Client.PutObjectAsync(putRequest);
            //    LambdaLogger.Log($"Uploaded object bios/{biosEditor.BiosBootUpMessage}-original.rom to bucket {BucketName}. Request Id: {response.ResponseMetadata.RequestId}");

            //    putRequest = new PutObjectRequest
            //    {
            //        BucketName = BucketName,
            //        Key = $"bios/{name}-current.rom",
            //        InputStream = outputStream
            //    };
            //    response = await S3Client.PutObjectAsync(putRequest);
            //    LambdaLogger.Log($"Uploaded object bios/{name}-current.rom to bucket {BucketName}. Request Id: {response.ResponseMetadata.RequestId}");

            //    Response.ContentType = "application/octet-stream";
            //    Response.Headers.Add("Content-Disposition", $"attachment;{name}-current.rom");
            //    new MemoryStream(outputStream.ToArray()).CopyTo(Response.Body);
            //}
            //catch (AmazonS3Exception e)
            //{
            //    LambdaLogger.Log(e.Message);
            //    Response.StatusCode = (int)e.StatusCode;
            //    var writer = new StreamWriter(Response.Body);
            //    writer.Write(e.Message);
            //}
        }
    }
}