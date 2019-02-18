using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using Monitoring.Dto;
using Monitoring.Infrastructure.MongoDB;
using Monitoring.Infrastructure.MongoDB.Documents;
using Newtonsoft.Json;

namespace Monitoring.Routines.ReprocessData
{
    class Program
    {
        private static MongoRepository _mongoRepository;
        private static AmazonS3Client _s3Client;

        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true).Build();

            if (string.IsNullOrWhiteSpace(config["MongoSettings:mongoUrl"]))
            {
                Console.WriteLine("Can't find Environment Variable for MongoDB URL.");
                Console.ReadKey();
                return;
            }

            if (string.IsNullOrWhiteSpace(config["MongoSettings:mongoUser"]))
            {
                Console.WriteLine("Can't find Environment Variable for MongoDB User.");
                Console.ReadKey();
                return;
            }

            if (string.IsNullOrWhiteSpace(config["MongoSettings:mongoDBName"]))
            {
                Console.WriteLine("Can't find Environment Variable for MongoDB DB Name.");
                Console.ReadKey();
                return;
            }

            if (string.IsNullOrWhiteSpace(config["MongoSettings:mongoPassword"]))
            {
                Console.WriteLine("Can't find Environment Variable for MongoDB Password.");
                Console.ReadKey();
                return;
            }

            var credential = MongoCredential.CreateCredential(config["MongoSettings:mongoDBName"],
                config["MongoSettings:mongoUser"], config["MongoSettings:mongoPassword"]);

            _mongoRepository = new MongoRepository(credential, config["MongoSettings:mongoUrl"], config["MongoSettings:mongoDBName"]);

            if (string.IsNullOrWhiteSpace(config["AwsSettings:AccessKey"]))
            {
                Console.WriteLine("Can't find Environment Variable for AwsSettings AccessKey.");
                Console.ReadKey();
                return;
            }

            if (string.IsNullOrWhiteSpace(config["AwsSettings:SecretKey"]))
            {
                Console.WriteLine("Can't find Environment Variable for AwsSettings SecretKey.");
                Console.ReadKey();
                return;
            }

            if (string.IsNullOrWhiteSpace(config["AwsSettings:S3Url"]))
            {
                Console.WriteLine("Can't find Environment Variable for AwsSettings S3Url.");
                Console.ReadKey();
                return;
            }
            AWSCredentials credentials = new BasicAWSCredentials(config["AwsSettings:AccessKey"],
                config["AwsSettings:SecretKey"]);
            var amazonSqsConfig = new AmazonS3Config
            {
                ServiceURL = config["AwsSettings:S3Url"]
            };
            _s3Client = new AmazonS3Client(credentials, amazonSqsConfig);
            if (_s3Client == null)
            {
                Console.WriteLine("Cannot create Amazon S3 client from existing configuration.");
                Console.ReadKey();
                return;
            }



            // Menu
            while (true)
            {
                Console.WriteLine("Chose option:");
                Console.WriteLine("1 - Process rigs");
                Console.WriteLine("2 - Process logs");

                var key = Console.ReadKey();
                Console.WriteLine();
                switch (key.KeyChar)
                {
                    case '1':
                        ProcessRigs();
                        break;
                    case '2':
                        ProcessLogs();
                        break;
                }
            }
        }

        private static void ProcessRigs()
        {
            var exclude = new[] { "rigs/test/" };
            string continuationToken = null;
            string last = null;
            while (true)
            {
                var listObjectsRequest = new ListObjectsV2Request
                {
                    BucketName = "monitoringapp-dev",
                    Prefix = "rigs/",
                    ContinuationToken = continuationToken,
                    StartAfter = last
                };
                var listObjectsResponse = _s3Client.ListObjectsV2Async(listObjectsRequest).Result;
                foreach (var entry in listObjectsResponse.S3Objects)
                {
                    if (entry.Size > 0 && !exclude.Any(x => entry.Key.Contains(x)))
                    {
                        Console.WriteLine("Found object with key {0}, size {1}", entry.Key, entry.Size);

                        var response = _s3Client.GetObjectAsync(entry.BucketName, entry.Key).Result;
                        using (var sr = new StreamReader(response.ResponseStream))
                        {
                            var content = sr.ReadToEnd();
                            MinerUnitDocument model;
                            try
                            {
                                model = JsonConvert.DeserializeObject<MinerUnitDocument>(content);

                                if (model == null)
                                {
                                    continue;
                                }

                                Console.WriteLine($"Read file with GPU SysLabel: {model.GPU?.SysLabel}");
                            }
                            catch
                            {
                                Console.WriteLine($"Can't ead file with content: {content}");
                                continue;
                            }

                            model.Id = ObjectId.GenerateNewId(DateTime.Now);
                            model.CreatedTimestamp = DateTime.UtcNow;
                            _mongoRepository.GetMinerUnits().InsertOne(model);
                        }
                        last = entry.Key;
                    }
                }
                last = listObjectsResponse.S3Objects.LastOrDefault() == null ? last : listObjectsResponse.S3Objects.Last().Key;
                continuationToken = listObjectsResponse.NextContinuationToken;
                if (listObjectsResponse.KeyCount == 0)
                {
                    return;
                }
            }
        }

        private static void ProcessLogs()
        {
            var exclude = new[] { "logs/dev/" };
            string continuationToken = null;
            string last = null;
            while (true)
            {
                var listObjectsRequest = new ListObjectsV2Request
                {
                    BucketName = "monitoringapp-dev",
                    Prefix = "logs/",
                    ContinuationToken = continuationToken,
                    StartAfter = last
                };
                var listObjectsResponse = _s3Client.ListObjectsV2Async(listObjectsRequest).Result;
                foreach (var entry in listObjectsResponse.S3Objects)
                {
                    if (entry.Size > 0 && !exclude.Any(x => entry.Key.Contains(x)))
                    {
                        Console.WriteLine("Found object with key {0}, size {1}", entry.Key, entry.Size);

                        var response = _s3Client.GetObjectAsync(entry.BucketName, entry.Key).Result;
                        using (var sr = new StreamReader(response.ResponseStream))
                        {
                            var content = sr.ReadToEnd();
                            LogDto model;
                            try
                            {
                                model = JsonConvert.DeserializeObject<LogDto>(content);

                                if (model == null || model.GPU == null)
                                {
                                    continue;
                                }

                                Console.WriteLine($"Read log file with GPU SysLabel: {model.GPU?.SysLabel}");
                            }
                            catch
                            {
                                Console.WriteLine($"Can't ead file with content: {content}");
                                continue;
                            }

                            model.GPU.Id = ObjectId.GenerateNewId(DateTime.Now);
                            _mongoRepository.GetMinerLogs().InsertOne(model.GPU);
                        }
                        last = entry.Key;
                    }
                }
                last = listObjectsResponse.S3Objects.LastOrDefault() == null ? last : listObjectsResponse.S3Objects.Last().Key;
                continuationToken = listObjectsResponse.NextContinuationToken;
                if (listObjectsResponse.KeyCount == 0)
                {
                    return;
                }
            }
        }
    }
}
