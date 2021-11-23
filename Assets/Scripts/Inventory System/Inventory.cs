using System;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory_System
{
    public class Inventory : MonoBehaviour
    {
        public event EventHandler OnItemListChanged;
        private readonly List<Item>   _itemList;
        private readonly Action<Item> _useItemAction;

        public Inventory(Action<Item> useItemAction)
        {
            _useItemAction = useItemAction;
            _itemList = new List<Item>();
        }

        public void AddItem(Item item)
        {
            if (item.IsStackable())
            {
                var itemAlreadyInInventory = false;
                foreach (var inventoryItem in _itemList)
                {
                    if (inventoryItem.Equals(item))
                    {
                        inventoryItem.amount += item.amount;
                        itemAlreadyInInventory = true;
                    }
                }

                if (!itemAlreadyInInventory)
                {
                    _itemList.Add(item);
                }
            } else
            {
                _itemList.Add(item);
            }
            OnItemListChanged?.Invoke(this, EventArgs.Empty);
        }

        public List<Item> GetItemList()
        {
            return _itemList;
        }

        public void RemoveItem(Item item)
        {
            if (item.IsStackable())
            {
                Item itemInInventory = null;
                foreach (var inventoryItem in _itemList)
                {
                    if (inventoryItem.Equals(item))
                    {
                        inventoryItem.amount -= 1;
                        itemInInventory = inventoryItem;
                    }
                }

                if (itemInInventory != null && itemInInventory.amount <= 0)
                {
                    _itemList.Remove(itemInInventory);
                }
            } 
            else
            {
                _itemList.Remove(item);
            }
            OnItemListChanged?.Invoke(this, EventArgs.Empty);
        }

        public void UseItem(Item item)
        {
            _useItemAction(item);
        }
    }
}
