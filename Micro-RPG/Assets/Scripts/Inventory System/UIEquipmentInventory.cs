using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory_System
{
    public class UIEquipmentInventory : MonoBehaviour
    {
        private EquipmentInventory _inventory;
        private Transform          _itemSlotContainer;
        private Player             _player;

        private void Awake()
        {
            _itemSlotContainer = transform.Find("itemSlotContainer");
        }

        public void SetPlayer(Player player)
        {
            _player = player;
        }
        
        public void SetInventory(EquipmentInventory inventory)
        {
            _inventory = inventory;
            inventory.OnItemListChanged += Inventory_OnItemListChanged;
            RefreshInventoryItems();
        }

        private void Inventory_OnItemListChanged(object sender, System.EventArgs e)
        {
            RefreshInventoryItems();
        }

        private void RefreshInventoryItems()
        {
            foreach (var item in _inventory.GetItemList())
            {
                if (item is WeaponItem)
                {
                    var weaponSlot      = _itemSlotContainer.Find("weaponSlot");
                    var weaponSlotImage = weaponSlot.Find("image").GetComponent<Image>();
                    weaponSlot.GetComponent<Button_UI>().enabled = true;
                    weaponSlotImage.sprite = item.GetSprite();
                    weaponSlotImage.gameObject.SetActive(true);

                    weaponSlot.GetComponent<Button_UI>().MouseRightClickFunc = () =>
                    {
                        // Un-equip the item
                        //_inventory.UnEquipItem(item);
                        _player.GetComponent<PlayerEquipmentManager>().UnEquip(item);
                        weaponSlot.GetComponent<Button_UI>().enabled = false;
                        weaponSlotImage.gameObject.SetActive(false);
                    };
                    break;
                }
                // switch (item.itemType)
                // {
                //     case Item.ItemType.WoodenSword:
                //         var weaponSlot = _itemSlotContainer.Find("weaponSlot");
                //         var weaponSlotImage = weaponSlot.Find("image").GetComponent<Image>();
                //         weaponSlot.GetComponent<Button_UI>().enabled = true;
                //         weaponSlotImage.sprite = item.GetSprite();
                //         weaponSlotImage.gameObject.SetActive(true);
                //
                //         weaponSlot.GetComponent<Button_UI>().MouseRightClickFunc = () =>
                //         {
                //             // Un-equip the item
                //             //_inventory.UnEquipItem(item);
                //             _player.GetComponent<PlayerEquipmentManager>().UnEquip(item);
                //             weaponSlot.GetComponent<Button_UI>().enabled = false;
                //             weaponSlotImage.gameObject.SetActive(false);
                //         };
                //         break;
                //     case Item.ItemType.SpellBook:
                //         break;
                //     case Item.ItemType.HealthPotion:
                //         break;
                //     case Item.ItemType.ManaPotion:
                //         break;
                //     case Item.ItemType.Coin:
                //         break;
                //     default:
                //         throw new ArgumentOutOfRangeException();
                // }
                // var itemSlotRectTransform =
                //     Instantiate(_itemSlotTemplate, _itemSlotContainer).GetComponent<RectTransform>();
                // itemSlotRectTransform.gameObject.SetActive(true);
                //
                // // Action when left-clicking on an item
                
                //
                // Action when right-clicking
            }
        }
    }
}