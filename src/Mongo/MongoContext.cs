using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hangfire.MongoSample.Mongo
{
    public class MongoContext
    {
        private readonly HashSet<string> _collectionNames;

        static MongoContext()
        {
            BsonSerializer.RegisterSerializer(typeof(Guid), new GuidSerializer(BsonType.String));
            BsonSerializer.RegisterIdGenerator(typeof(Guid), new GuidGenerator());

            var pack = new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new IgnoreExtraElementsConvention(true)
            };
            ConventionRegistry.Register("Custom Conventions", pack, t => true);
        }

        public MongoContext(IOptions<MongoSettings> options)
        {
            var mongoClientSettings = MongoClientSettings.FromConnectionString(options.Value.ConnectionString);

            Client = new MongoClient(mongoClientSettings);

            DatabaseName = options.Value.DatabaseName;
            Database = Client.GetDatabase(options.Value.DatabaseName);

            _collectionNames = Database.ListCollectionNames().ToList().ToHashSet();

            Appointments = GetCreatedCollection<Appointment>("appointments");
        }

        public MongoClient Client { get; }

        public IMongoDatabase Database { get; }

        public string DatabaseName { get;  }

        public IMongoCollection<Appointment> Appointments { get; }

        private IMongoCollection<T> GetCreatedCollection<T>(string name)
        {
            if (!_collectionNames.Contains(name))
            {
                Database.CreateCollection(name);
            }

            return Database.GetCollection<T>(name);
        }
    }
}
