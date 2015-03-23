using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.InventoryDomain
{
    public class StorageFacility : AggregateRootBase
    {
        public string FacilityId { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public Inventory Inventory { get; set; } 

        public void ReserveItem()
        {
        }

        public void InitiateDeliveryOfItem()
        {
        }

        public void CreateShipmentDetails()
        {
        }

        public void ShipItem()
        {
        }
    }

    public class Inventory
    {
        public Inventory()
        {
            Items = new List<Item>();
        }

        public IList<Item> Items { get; set; } 

        public int GetQuantityOfModelId(string modelId)
        {
            return 1; // todo need to flesh out logic for this
        }
    }

    public class Item
    {
        [BsonElement("itemid")]
        public string ItemId { get; set; }
        
        [BsonElement("itemname")]
        public string ItemName { get; set; }
    }
}
