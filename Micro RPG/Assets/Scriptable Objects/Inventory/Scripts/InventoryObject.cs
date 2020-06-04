using System;
using System.IO;
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

        public void AddItem(Item _item, int _amount)
        {
            if (_item.Buffs.Length > 0)
            {
                SetEmptySlot(_item, _amount);
                return;
            }

            for (int i = 0; i < container.Items.Length; i++)
            {
                if (container.Items[i].ID == _item.Id)
                {
                    container.Items[i].AddAmount(_amount);
                    return;
                }
            }
            SetEmptySlot(_item, _amount);
        }

        public InventorySlot SetEmptySlot(Item _item, int _amount)
        {
            for (int i = 0; i < container.Items.Length; i++)
            {
                if (container.Items[i].ID <= -1)
                {
                    container.Items[i].UpdateSlot(_item.Id, _item, _amount);
                    return container.Items[i];
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
            InventorySlot temp = new InventorySlot(item2.ID, item2.item, item2.amount);
            item2.UpdateSlot(item1.ID, item1.item, item1.amount);
            item1.UpdateSlot(temp.ID, temp.item, temp.amount);
        }

        public void RemoveItem(Item _item)
        {
            for (int i = 0; i < container.Items.Length; i++)
            {
                if (container.Items[i].item == _item)
                {
                    container.Items[i].UpdateSlot(-1, null, 0);
                }
            }
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
            container = new Inventory();
        }
    }

    [Serializable]
    public class InventorySlot
    {
        public int ID = -1;
        public Item item;
        public int amount;

        public InventorySlot()
        {
            ID = -1;
            item = null;
            amount = 0;
        }
    
        public InventorySlot(int _id, Item _item, int _amount)
        {
            ID = _id;
            item = _item;
            amount = _amount;
        }

        public void AddAmount(int value)
        {
            amount += value;
        }

        public void UpdateSlot(int _id, Item _item, int _amount)
        {
            ID = _id;
            item = _item;
            amount = _amount;
        }
    }

    [Serializable]
    public class Inventory
    {
        public InventorySlot[] Items = new InventorySlot[25];
    }
}