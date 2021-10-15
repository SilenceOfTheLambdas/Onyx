using System;
using System.Collections.Generic;

namespace Inventory_System
{
    public class EquipmentInventory
    {
        public event EventHandler OnItemListChanged;
        private readonly List<Item> itemList;

        public EquipmentInventory()
        {
            itemList = new List<Item>();
        }

        public void AddItem(Item item)
        {
            if (item.IsStackable())
            {
                var itemAlreadyInInventory = false;
                foreach (var inventoryItem in itemList)
                {
                    if (inventoryItem.Equals(item))
                    {
                        inventoryItem.amount += item.amount;
                        itemAlreadyInInventory = true;
                    }
                }

                if (!itemAlreadyInInventory)
                {
                    itemList.Add(item);
                }
            } else
            {
                itemList.Add(item);
            }
            OnItemListChanged?.Invoke(this, EventArgs.Empty);
        }

        public List<Item> GetItemList()
        {
            return itemList;
        }

        public void RemoveItem(Item item)
        {
            if (item.IsStackable())
            {
                Item itemInInventory = null;
                foreach (var inventoryItem in itemList)
                {
                    if (inventoryItem.Equals(item))
                    {
                        inventoryItem.amount -= item.amount;
                        itemInInventory = inventoryItem;
                    }
                }

                if (itemInInventory != null && itemInInventory.amount <= 0)
                {
                    itemList.Remove(itemInInventory);
                }
            } else
            {
                itemList.Remove(item);
            }
            OnItemListChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}