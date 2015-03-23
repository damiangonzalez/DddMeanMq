using Domain.InventoryDomain;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Linq;

namespace Repository
{
    internal interface IInventoryRepository
    {
        StorageFacility GetStorageFacility(string identifier);
    }

    public class InventoryRepository : IInventoryRepository
    {
        public static string DatabaseName = "Inventory";
        private readonly MongoDatabase _database;

        public InventoryRepository()
        {
            // Create server settings to pass connection string, timeout, etc.
            var settings = new MongoServerSettings
            {
                Server = new MongoServerAddress("localhost", 27017)
            };

            // Create server object to communicate with our server
            var server = new MongoServer(settings);
            // Get our database instance to reach collections and data
            _database = server.GetDatabase(DatabaseName);

        }

        public StorageFacility GetStorageFacility(string identifier)
        {
            var query = Query<StorageFacility>.EQ(e => e.FacilityId, identifier);
            var singleItem = _database.GetCollection<StorageFacility>(DatabaseName).FindOneAs<StorageFacility>(query);
            return singleItem;
        }

        public void SaveStorageFacility(StorageFacility storageFacility)
        {
            // This will perform an add or an edit as appropriate
            var collectionOfItems = _database.GetCollection<StorageFacility>(DatabaseName);
            collectionOfItems.Save(storageFacility);
        }

        public void RemoveItemsFromStorageFacility(InventoryDomainEventCheckoutItems domainEvent)
        {
            IMongoQuery query = Query<StorageFacility>.EQ(e => e.FacilityId, domainEvent.FacilityId);
            StorageFacility storageFacility = _database.GetCollection<StorageFacility>(DatabaseName).FindOneAs<StorageFacility>(query);

            foreach (string itemId in domainEvent.ItemIdsToBeRemovedList)
            {
                Item itemToRemove = storageFacility.Inventory.Items.FirstOrDefault(x => x.ItemId == itemId);
                storageFacility.Inventory.Items.Remove(itemToRemove);
            }

            SaveStorageFacility(storageFacility);
        }
    }
}
