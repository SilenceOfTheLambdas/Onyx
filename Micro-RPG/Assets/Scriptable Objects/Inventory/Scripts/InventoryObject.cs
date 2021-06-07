using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Scriptable_Objects.Items.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scriptable_Objects.Inventory.Scripts
{
    [CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
    public class InventoryObject : ScriptableObject
    {
        public string savePath;
        [FormerlySerializedAs("DatabaseObject")] public ItemDatabaseObject databaseObject;
        [FormerlySerializedAs("Container")] public Inventory container;
    
        public GameObject slotHolder;

        private void Awake()
        {
            container = Inventory.Instance;
        }

        public void AddItem(Item item, int amount)
        {
            if (item.Buffs.Length > 0)
            {
                SetEmptySlot(item, amount);
                return;
            }

            for (int i = 0; i < container.items.Length; i++)
            {
                if (container.items[i].id == item.Id)
                {
                    container.items[i].AddAmount(amount);
                    return;
                }
            }
            SetEmptySlot(item, amount);
        }

        public InventorySlot SetEmptySlot(Item _item, int _amount)
        {
            foreach (var inventorySlot in container.items)
            {
                if (inventorySlot.id <= -1)
                {
                    inventorySlot.UpdateSlot(_item.Id, _item, _amount);
                    return inventorySlot;
                }
            }

            // TODO: Setup feedback when inventory is full
            return null;
        }

        /// <summary>
        /// This will allow movement of items from one inventory slot to another.
        /// </summary>
        /// <param name="item1">The inventory slot to move.</param>
        /// <param name="item2">The inventory slot to which the item will be moved to.</param>
        public void MoveItem(InventorySlot item1, InventorySlot item2)
        {
            InventorySlot temp = new InventorySlot(item2.id, item2.item, item2.amount);
            item2.UpdateSlot(item1.id, item1.item, item1.amount);
            item1.UpdateSlot(temp.id, temp.item, temp.amount);
        }

        public void RemoveItem(Item _item)
        {
            for (int i = 0; i < container.items.Length; i++)
            {
                if (container.items[i].item == _item)
                {
                    container.items[i].UpdateSlot(-1, null, 0);
                }
            }
        }

        public bool ContainsItem(int idOfItem)
        {
            for (var i = 0; i < container.items.Length; i++)
            {
                if (container.items[i].item.Id == idOfItem)
                {
                    return true;
                }
            }

            return false;
        }

        [ContextMenu("Save")]
        public void Save()
        {
            string saveData = JsonUtility.ToJson(this, true);
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(string.Concat(Application.persistentDataPath, savePath));
            bf.Serialize(file, saveData);
            file.Close();
        }

        [ContextMenu("Load")]
        public void Load()
        {
            if (File.Exists(string.Concat(Application.persistentDataPath, savePath)))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(string.Concat(Application.persistentDataPath, savePath), FileMode.Open);
                JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), this);
                file.Close();
            }
        }

        [ContextMenu("Clear")]
        public void Clear()
        {
            container.Clear();
        }
    }

    [Serializable]
    public class InventorySlot
    {
        public ItemTypes[]   allowedItems = new ItemTypes[0];
        public UserInterface parent;
        public int           id;
        public Item          item;
        public int           amount;

        public InventorySlot()
        {
            id = -1;
            item = null;
            amount = 0;
        }
    
        public InventorySlot(int id, Item item, int amount)
        {
            this.id = id;
            this.item = item;
            this.amount = amount;
        }

        public void AddAmount(int value)
        {
            amount += value;
        }

        public void UpdateSlot(int _id, Item _item, int _amount)
        {
            id = _id;
            item = _item;
            amount = _amount;
        }

        public bool CanPlaceInSlot(ItemObject _item)
        {
            if (allowedItems.Length <= 0) return true;

            for (int i = 0; i < allowedItems.Length; i++)
            {
                if (_item.type == allowedItems[i])
                    return true;
            }

            return false;
        }
    }

    [Serializable]
    public class Inventory : MonoBehaviour
    {
        public        InventorySlot[] items = new InventorySlot[25];
        public static Inventory       Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            } else 
                Destroy(Instance);
        }

        public void Clear()
        {
            for (int i = 0; i < items.Length; i++)
            {
                items[i].UpdateSlot(-1, new Item(), 0);
            }
        }
    }
}