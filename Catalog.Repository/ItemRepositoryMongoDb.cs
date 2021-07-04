using System;
using System.Collections.Generic;
using Catalog.Domain;
using MongoDB.Driver;

namespace Catalog.Repository
{
    public class ItemRepositoryMongoDb : IItemRepository
    {
        private const string databaseName = "catalog";
        private const string collectionName = "items";
        private readonly IMongoCollection<Item> itemsCollection;
        
        public ItemRepositoryMongoDb(IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(databaseName);
            itemsCollection = database.GetCollection<Item>(collectionName);
        }
        public IEnumerable<Item> GetItems()
        {
            throw new NotImplementedException();
        }

        public Item GetItem(Guid id)
        {
            throw new NotImplementedException();
        }

        public void CreateItem(Item item)
        {
            itemsCollection.InsertOne(item);
        }

        public void UpdateItem(Item item)
        {
            throw new NotImplementedException();
        }

        public void DeleteItem(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}