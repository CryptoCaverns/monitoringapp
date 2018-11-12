﻿using System;
using MongoDB.Driver;
using Monitoring.Infrastructure.MongoDB.Documents;

namespace Monitoring.Infrastructure.MongoDB
{
    public class MongoRepository
    {
        private readonly IMongoClient _client;

        public MongoRepository()
        {
            if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("mongoUrl")))
            {
                throw new Exception("Can't find Environment Variable for MongoDB URL.");
            }

            if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("mongoUser")))
            {
                throw new Exception("Can't find Environment Variable for MongoDB User.");
            }

            if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("mongoDBName")))
            {
                throw new Exception("Can't find Environment Variable for MongoDB DB Name.");
            }

            if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("mongoPassword")))
            {
                throw new Exception("Can't find Environment Variable for MongoDB Password.");
            }

            var credential = MongoCredential.CreateCredential(Environment.GetEnvironmentVariable("mongoDBName"),
                Environment.GetEnvironmentVariable("mongoUser"), Environment.GetEnvironmentVariable("mongoPassword"));

            var settings = new MongoClientSettings
            {
                Credential = credential,
                Server = new MongoServerAddress(Environment.GetEnvironmentVariable("mongoUrl"))
            };
            _client = new MongoClient(settings);
        }

        public IMongoCollection<MinerUnitDocument> GetMinerUnits()
        {
            return GetDb().GetCollection<MinerUnitDocument>("miner-units");
        }

        public IMongoCollection<MinerLogDocument> GetMinerLogs()
        {
            return GetDb().GetCollection<MinerLogDocument>("miner-logs");
        }

        private IMongoDatabase GetDb()
        {
            try
            {
                return _client.GetDatabase(Environment.GetEnvironmentVariable("mongoDBName"));
            }
            catch (Exception e)
            {
                throw new Exception("Error instantiating database monitoring. " + e.Message);
            }
        }
    }
}