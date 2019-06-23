﻿using System;
using System.IO;
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
using Monitoring.Infrastructure.RomEditor.Helpers;

namespace Monitoring.AWS.Lambda.RomProcessor.Controllers
{
    [Route("api/[controller]")]
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

        [HttpPost("")]
        public async Task Post()
        {
            // Copy the request body into a seekable stream required by the AWS SDK for .NET.
            var seekableStream = new MemoryStream();
            await Request.Body.CopyToAsync(seekableStream);
            seekableStream.Position = 0;

            // open bios file
            var biosEditor = new BiosEditor();
            biosEditor.Open(seekableStream);

            if (CheckIfRegister(biosEditor.BiosBootUpMessage))
            {
                // already registered - just return current version from s3
                try
                {
                    var getRequest = new GetObjectRequest
                    {
                        BucketName = BucketName,
                        Key = $"bios/{biosEditor.BiosBootUpMessage}-current.rom",
                    };

                    var response = await S3Client.GetObjectAsync(getRequest);

                    if (response.HttpStatusCode == HttpStatusCode.OK)
                    {
                        Response.ContentType = "application/octet-stream";
                        Response.Headers.Add("Content-Disposition", $"attachment;{biosEditor.BiosBootUpMessage}-current.rom");
                        response.ResponseStream.CopyTo(Response.Body);
                    }
                    else
                    {
                        Response.StatusCode = (int)HttpStatusCode.NotFound;
                    }
                }
                catch (AmazonS3Exception e)
                {
                    LambdaLogger.Log(e.Message);
                    Response.StatusCode = (int)e.StatusCode;
                    var writer = new StreamWriter(Response.Body);
                    writer.Write(e.Message);
                }
            }
            else
            {
                // register bios in system
                var name = Guid.NewGuid().ToString();
                LambdaLogger.Log($"New name {name}");
                biosEditor.BiosBootUpMessage = name;
                var outputStream = biosEditor.Save();

                var putRequest = new PutObjectRequest
                {
                    BucketName = BucketName,
                    Key = $"bios/{name}-original.rom",
                    InputStream = new MemoryStream(seekableStream.ToArray())
                };

                try
                {
                    // save record to mongo
                    RegisterBios(name, outputStream.ToArray().GetHashCode(), seekableStream.ToArray().GetHashCode());

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

                    Response.ContentType = "application/octet-stream";
                    Response.Headers.Add("Content-Disposition", $"attachment;{name}-current.rom");
                    new MemoryStream(outputStream.ToArray()).CopyTo(Response.Body);
                }
                catch (AmazonS3Exception e)
                {
                    LambdaLogger.Log(e.Message);
                    Response.StatusCode = (int)e.StatusCode;
                    var writer = new StreamWriter(Response.Body);
                    writer.Write(e.Message);
                }

            }
        }

        private bool CheckIfRegister(string name)
        {
            return MongoRepository.GetBios().Find(x => x.Name == name).Any();
        }

        private void RegisterBios(string name, int currentHash, int originalHash)
        {
            MongoRepository.GetBios().InsertOne(new BiosDocument
            {
                Name = name,
                CurrentHash = currentHash,
                OriginalHash = originalHash,
                Timestamp = DateTime.Now,
                Id = ObjectId.GenerateNewId(DateTime.Now)
        });
        }
    }
}