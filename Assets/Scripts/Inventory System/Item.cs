using System;
using UnityEngine;

namespace Inventory_System
{
    [Serializable]
    public abstract class Item : ScriptableObject
    {
        [Tooltip("The name of this item")]
        public            string     itemName;
        [Tooltip("Description")]
        [TextArea] public string     itemDescription;
        
        [Tooltip("Sprite that appears in the inventory system")]
        public            Sprite     sprite;
        [Tooltip("The prefab object when spawned on the ground")]
        public            GameObject itemWorldPrefab;
        public            bool       isStackable;
        public            int        amount;

        public Sprite GetSprite() => sprite;

        public bool IsStackable() => isStackable;

        public bool Equals(Item item)
        {
            // First check to see if the item is the same type
            if (this.GetType() == item.GetType())
            {
                if (item is Coin) return true;
                // Check to see if stats are the same
                if (item is WeaponItem weaponItem)
                {
                    var thisWeapon = this as WeaponItem;
                    if (thisWeapon != null)
                    {
                        if (weaponItem.damage == thisWeapon.damage
                            && weaponItem.weaponRange == thisWeapon.weaponRange)
                        {
                            return true;
                        }
                    }
                }

                if (item is HealthPotion healthPotion)
                {
                    var thisHealthPotion = this as HealthPotion;
                    if (thisHealthPotion != null)
                        if (healthPotion.restoreAmount == thisHealthPotion.restoreAmount)
                            return true;
                }
                
                if (item is ManaPotion manaPotion)
                {
                    var thisManaPotion = this as ManaPotion;
                    if (thisManaPotion != null)
                        if (manaPotion.restoreAmount == thisManaPotion.restoreAmount)
                            return true;
                }
            }

            return false;
        }

        [ContextMenu("Add Item to Player")]
        private void AddToPlayerInventory()
        {
            GameManager.Instance.player.Inventory.AddItem(this);
        }

        public abstract void RandomlyGenerateItem();
    }
}
