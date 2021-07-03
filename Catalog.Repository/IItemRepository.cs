using System;
using System.Collections.Generic;
using Catalog.Domain;

namespace Catalog.Repository
{
    public interface IItemRepository
    {
        IEnumerable<Item> GetItems();
        Item GetItem(Guid id);
        void CreateItem(Item item);
        void UpdateItem(Item item);
        void DeleteItem(Guid id);
    }
}