using System;
using UnityEngine;

namespace Scriptable_Objects.Items.Scripts
{
    public enum ItemTypes
    {
        Consumable,
        Helmet,
        Chest,
        Legs,
        Boots,
        MainWeapon,
        OffHandWeapon,
        Default
    }

    public enum Attributes
    {
        Agility,
        Intellect,
        Stamina,
        Strength
    }
    public abstract class ItemObject : ScriptableObject
    {
        public int Id;
        public Sprite PSprite;
        public ItemTypes type;
        [TextArea(15, 20)]
        public string description;
        public ItemBuff[] buffs;
        public Item CreateItem()
        {
            Item newItem = new Item(this);

            return newItem;
        }
    }

    [Serializable]
    public class Item
    {
        public string Name;
        public int Id;
        public ItemBuff[] Buffs;

        public Item()
        {
            Name = "";
            Id = -1;
        }
        
        public Item(ItemObject item)
        {
            Name = item.name;
            Id = item.Id;
            Buffs = new ItemBuff[item.buffs.Length];
            for (int i = 0; i < Buffs.Length; i++)
            {
                Buffs[i] = new ItemBuff(item.buffs[i].min, item.buffs[i].max) {attribute = item.buffs[i].attribute};
            }
        }
    }

    [Serializable]
    public class ItemBuff
    {
        public Attributes attribute;
        public int value;
        public int min;
        public int max;
    
        public ItemBuff(int _min, int _max)
        {
            min = _min;
            max = _max;
            GenerateValues();
        }

        public void GenerateValues()
        {
            value = UnityEngine.Random.Range(min, max);
        }
    }
}