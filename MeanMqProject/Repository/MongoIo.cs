using System.Collections.Generic;
using System.Linq;
using Domain;
using Domain.InventoryDomain;
using Domain.PurchasingDomain;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace RabbitMQPubSubNode
{
    public class MongoIo : Repository.IMongoIo
    {
        readonly string _databaseName;
        readonly MongoDatabase _database;

        public MongoIo(string dbName)
        {
            _databaseName = dbName;
            // Create server settings to pass connection string, timeout, etc.
            var settings = new MongoServerSettings
            {
                Server = new MongoServerAddress("localhost", 27017)
            };

            // Create server object to communicate with our server
            var server = new MongoServer(settings);
            // Get our database instance to reach collections and data
            _database = server.GetDatabase(_databaseName);
        }

        public void InsertToDb(string identifier, string sampleString)
        {
            var collectionOfItems = _database.GetCollection<AggregateRootBase>(_databaseName);
            var singleItem = new Cart
            {
                Notes = sampleString, 
                Id = ObjectId.Parse(identifier)
            };

            collectionOfItems.Insert(singleItem);
        }

        public void UpdateInDb(string identifier, string sampleString)
        {
            var collectionOfItems = _database.GetCollection<AggregateRootBase>(_databaseName);
            var singleItem = collectionOfItems.FindOneById(ObjectId.Parse(identifier));
            singleItem.Notes = sampleString;
            // singleItem["DateTimeUpdated"] = DateTime.Now;
            collectionOfItems.Save(singleItem);
        }

        public string ReadFromDb(string identifier)
        {
            var query = Query<AggregateRootBase>.EQ(e => e.Id, ObjectId.Parse(identifier));
            var singleItem = _database.GetCollection<AggregateRootBase>(_databaseName).FindOne(query);
            return singleItem.Notes;
        }

        public IEnumerable<Item> ReadItemsFromDb()
        {
            var collection= _database.GetCollection<Item>(_databaseName).FindAll();
            return collection.AsEnumerable();
        }

        public static string GetNewId()
        {
            return ObjectId.GenerateNewId().ToString();
        }

        public static ObjectId GetNewObjectId()
        {
            return ObjectId.GenerateNewId();
        }
    }
}
