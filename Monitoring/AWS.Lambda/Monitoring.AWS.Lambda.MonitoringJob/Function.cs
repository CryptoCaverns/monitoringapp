using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using MongoDB.Bson;
using Monitoring.Infrastructure.MongoDB;
using Monitoring.Infrastructure.MongoDB.Documents;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Monitoring.AWS.Lambda.MonitoringJob
{
    public class Function
    {
        IAmazonS3 S3Client { get; }

        /// <summary>
        /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
        /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
        /// region the Lambda function is executed in.
        /// </summary>
        public Function()
        {
            S3Client = new AmazonS3Client();
        }

        /// <summary>
        /// Constructs an instance with a preconfigured S3 client. This can be used for testing the outside of the Lambda environment.
        /// </summary>
        /// <param name="s3Client"></param>
        public Function(IAmazonS3 s3Client)
        {
            S3Client = s3Client;
        }

        /// <summary>
        /// This method is called for every Lambda invocation. This method takes in an S3 event object and can be used 
        /// to respond to S3 notifications.
        /// </summary>
        /// <param name="evnt"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<string> FunctionHandler(S3Event evnt, ILambdaContext context)
        {
            var s3Event = evnt.Records?[0].S3;
            if (s3Event == null)
            {
                return null;
            }

            try
            {

                var mongo = new MongoRepository();
                context.Logger.LogLine($"Read file from Bucket: {s3Event.Bucket.Name}");
                context.Logger.LogLine($"Read file with Key: {s3Event.Object.Key}");

                var response = await S3Client.GetObjectAsync(s3Event.Bucket.Name, s3Event.Object.Key);
                using (var sr = new StreamReader(response.ResponseStream))
                {
                    var content = sr.ReadToEnd();
                    MinerUnitDocument model;
                    try
                    {
                        model = JsonConvert.DeserializeObject<MinerUnitDocument>(content);

                        if (model == null)
                        {
                            throw new Exception();
                        }

                        context.Logger.LogLine($"Read file with GPU SysLabel: {model.GPU?.SysLabel}");
                    }
                    catch
                    {
                        context.Logger.LogLine($"Can't ead file with content: {content}");

                        return string.Empty;
                    }

                    model.Id = ObjectId.GenerateNewId(DateTime.Now);
                    model.CreatedTimestamp = DateTime.UtcNow;
                    mongo.GetMinerUnits().InsertOne(model);

                    return model.GPU?.SysLabel;
                }
            }
            catch (Exception e)
            {
                context.Logger.LogLine($"Error getting object {s3Event.Object.Key} from bucket {s3Event.Bucket.Name}. Make sure they exist and your bucket is in the same region as this function.");
                context.Logger.LogLine(e.Message);
                context.Logger.LogLine(e.StackTrace);
                throw;
            }
        }
    }
}
