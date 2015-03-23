using System.Collections.Generic;
using Domain;
using Domain.InventoryDomain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using RabbitMQPubSubNode;
using Repository;

namespace RabbitMQPubSubNodeTest
{
    [TestClass]
    public class InventoryRepositoryTests
    {
        private const string FacilityIdConst = "ABC4321";
        private const string ObjectIdConst = "547b8ae5bae5d712b847b1d5";

        [TestMethod]
        public void TestWriteThenReadStorageFacilityToDb()
        {
            InventoryRepository inventoryRepository = new InventoryRepository();

            StorageFacility storageFacility = new StorageFacility
            {
                FacilityId = FacilityIdConst,
                Address = "2000 Ultimate Way",
                Id = ObjectId.Parse(ObjectIdConst),
                Inventory = new Inventory
                {
                    Items = new List<Item>
                    {
                        new Item
                        {
                            ItemName = "Item 1 - Plastic Widget",
                            ItemId = "ITEM1"
                        },
                        new Item
                        {
                            ItemName = "Item 2 - Metal Widget",
                            ItemId = "ITEM2"
                        },
                        new Item
                        {
                            ItemName = "Item 3 - Wooden Widget",
                            ItemId = "ITEM3"
                        },
                        new Item
                        {
                            ItemName = "Item 4 - Ceramic Widget",
                            ItemId = "ITEM4"
                        },
                    }
                }
            };

            inventoryRepository.SaveStorageFacility(storageFacility);
            StorageFacility storageFacilityFromDb = inventoryRepository.GetStorageFacility(storageFacility.FacilityId);

            Assert.IsNotNull(storageFacilityFromDb);
            Assert.AreEqual(storageFacility.FacilityId, storageFacilityFromDb.FacilityId);
            Assert.AreEqual(4, storageFacilityFromDb.Inventory.Items.Count);
        }

        [TestMethod]
        public void TestCheckOutItemsFromStorageFacility()
        {
            InventoryDomainEventCheckoutItems domainEventCheckoutItems = new InventoryDomainEventCheckoutItems()
            {
                EventType = DomainEventType.InventoryDomainEventCheckoutItems,
                FacilityId = "ABC4321",
                SourceDomain = "Test",
                TargetDomain = "Inventory",
                ItemIdsToBeRemovedList = new[]
                {
                    "ITEM3"
                }
            };

            InventoryRepository inventoryRepository = new InventoryRepository();
            inventoryRepository.RemoveItemsFromStorageFacility(domainEventCheckoutItems);

            StorageFacility storageFacilityFromDb = inventoryRepository.GetStorageFacility(FacilityIdConst);

            Assert.IsNotNull(storageFacilityFromDb);
            Assert.AreEqual(FacilityIdConst, storageFacilityFromDb.FacilityId);
            Assert.AreEqual(2, storageFacilityFromDb.Inventory.Items.Count);
        }
    }
}
