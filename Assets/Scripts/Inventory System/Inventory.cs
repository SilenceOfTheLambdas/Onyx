using System;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory_System
{
    public class Inventory : MonoBehaviour
    {
        public event EventHandler OnItemListChanged;
        private List<Item>   itemList;
        private Action<Item> useItemAction;

        public Inventory(Action<Item> useItemAction)
        {
            this.useItemAction = useItemAction;
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
                        inventoryItem.amount -= 1;
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

        public void UseItem(Item item)
        {
            useItemAction(item);
        }
    }
}
