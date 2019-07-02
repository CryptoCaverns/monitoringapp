using System;
using System.IO;
using Amazon.Lambda.Core;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Monitoring.Dto;
using Monitoring.Infrastructure.MongoDB;
using Monitoring.Infrastructure.MongoDB.Documents;
using Newtonsoft.Json;

namespace Monitoring.AWS.Lambda.Monitoring.Controllers
{
    /// <summary>
    /// ASP.NET Core controller acting as a S3 Proxy.
    /// </summary>
    [Route("api/[controller]")]
    public class MonitoringController : Controller
    {
        public MongoRepository MongoRepository { get; set; }

        public MonitoringController()
        {
            MongoRepository = new MongoRepository();
        }

        [HttpPost("miner-unit")]
        public IActionResult CreateMinerUnit()
        {
            try
            {
                using (var sr = new StreamReader(Request.Body))
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

                        LambdaLogger.Log($"Parse object with GPU SysLabel: {model.GPU?.SysLabel}");
                    }
                    catch
                    {
                        LambdaLogger.Log($"Can't parse object with content: {content}");

                        return base.BadRequest();
                    }

                    model.Id = ObjectId.GenerateNewId(DateTime.Now);
                    model.CreatedTimestamp = DateTime.UtcNow;
                    MongoRepository.GetMinerUnits().InsertOne(model);

                    return base.Ok(model.GPU?.SysLabel);
                }
            }
            catch (Exception e)
            {
                LambdaLogger.Log(e.Message);
                LambdaLogger.Log(e.StackTrace);
                throw;
            }
        }

        [HttpPost("stats")]
        public IActionResult AddStats()
        {
            try
            {
                using (var sr = new StreamReader(Request.Body))
                {
                    var content = sr.ReadToEnd();
                    LogDto model;
                    try
                    {
                        model = JsonConvert.DeserializeObject<LogDto>(content);

                        if (model == null)
                        {
                            throw new Exception();
                        }

                        LambdaLogger.Log($"Parse object with stats for GPU-SysLabel: {model.GPU.SysLabel}");
                    }
                    catch
                    {
                        LambdaLogger.Log($"Can't parse object with content: {content}");
                        return base.BadRequest();
                    }

                    model.GPU.Id = ObjectId.GenerateNewId(DateTime.Now);
                    MongoRepository.GetMinerLogs().InsertOne(model.GPU);

                    return base.Ok(model.GPU?.SysLabel);
                }
            }
            catch (Exception e)
            {
                LambdaLogger.Log(e.Message);
                LambdaLogger.Log(e.StackTrace);
                throw;
            }
        }
    }
}
