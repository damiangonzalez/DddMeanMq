using System;
using System.Collections.Generic;
using Domain;
using Domain.InventoryDomain;

namespace Repository
{
    public interface IMongoIo
    {
        void InsertToDb(string identifier, string sampleString);
        string ReadFromDb(string identifier);
        IEnumerable<Item> ReadItemsFromDb();
        
        void UpdateInDb(string identifier, string sampleString);
    }
}
