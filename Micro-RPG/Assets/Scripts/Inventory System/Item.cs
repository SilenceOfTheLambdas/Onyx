using System;
using UnityEngine;

namespace Inventory_System
{
    [Serializable]
    public class Item
    {
        public enum ItemType
        {
            Sword,
            SpellBook,
            HealthPotion,
            ManaPotion,
            Coin
        }

        public ItemType itemType;
        public int      amount;

        public Sprite GetSprite()
        {
            return itemType switch
            {
                ItemType.Sword => ItemAssets.Instance.swordSprite,
                ItemType.SpellBook => ItemAssets.Instance.spellBookSprite,
                ItemType.HealthPotion => ItemAssets.Instance.healthPotionSprite,
                ItemType.ManaPotion => ItemAssets.Instance.manaPotionSprite,
                ItemType.Coin => ItemAssets.Instance.coinSprite,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public bool IsStackable()
        {
            switch (itemType)
            {
                case ItemType.Sword:
                case ItemType.SpellBook:
                    return false;
                case ItemType.HealthPotion:
                case ItemType.ManaPotion:
                case ItemType.Coin:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
