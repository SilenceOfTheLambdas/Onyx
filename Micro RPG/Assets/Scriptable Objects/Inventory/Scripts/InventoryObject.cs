using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Scriptable_Objects.Items.Scripts;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject, ISerializationCallbackReceiver
{
    public string savePath;
    public ItemDatabaseObject DatabaseObject;
    public List<InventorySlot> Container = new List<InventorySlot>();
    public GameObject[] slots;
    public GameObject slotHolder;

    private void Awake()
    {
        slots = new GameObject[24];
        for (int i = 0; i < 24; i++)
        {
            slots[i] = slotHolder.transform.GetChild(i).gameObject;
        }
    }

    public void AddItem(ItemObject _item, int _amount)
    {
        for (int i = 0; i < Container.Count; i++)
        {
            if (Container[i].item == _item)
            {
                Container[i].AddAmount(_amount);
                return;
            }
        }
        Container.Add(new InventorySlot(DatabaseObject.getId[_item], _item, _amount));
    }

    public void OnBeforeSerialize()
    {
        throw new NotImplementedException();
    }

    public void OnAfterDeserialize()
    {
        for (int i = 0; i < Container.Count; i++)
            Container[i].item = DatabaseObject.getItem[Container[i].ID];
    }

    public void Save()
    {
        string saveData = JsonUtility.ToJson(true, true);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fileStream = File.Create(string.Concat(Application.persistentDataPath, saveData));
        bf.Serialize(fileStream, saveData);
        fileStream.Close();
    }

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
}

[System.Serializable]
public class InventorySlot
{
    public int ID;
    public ItemObject item;
    public int amount;

    public InventorySlot(int _id, ItemObject _item, int _amount)
    {
        ID = _id;
        item = _item;
        amount = _amount;
    }

    public void AddAmount(int value)
    {
        amount += value;
    }
}
