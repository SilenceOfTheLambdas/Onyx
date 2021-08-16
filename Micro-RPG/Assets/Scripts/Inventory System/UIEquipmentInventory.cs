using System;
using System.Linq;
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
        public  GameObject         hoverInterface;

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

                    // Display item stats when hovering over
                    weaponSlot.GetComponent<Button_UI>().MouseOverOnceTooltipFunc = () =>
                    {
                        hoverInterface.SetActive(true);
                        var itemName  = hoverInterface.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
                        var itemDescription = hoverInterface.transform.Find("itemDescription")
                            .GetComponent<TextMeshProUGUI>();
                        var itemStats = hoverInterface.transform.Find("ItemStats").GetComponent<TextMeshProUGUI>();
                    
                        // Set item name
                        itemName.SetText($"{item.itemName}");
                        itemDescription.SetText($"{item.itemDescription}");
                        
                        if (item is WeaponItem weaponItem)
                            itemStats.SetText($"Damage: {weaponItem.damage}\n" +
                                              $"Range: {weaponItem.weaponRange}");
                    };

                    weaponSlot.GetComponent<Button_UI>().MouseOutOnceTooltipFunc = () =>
                    {
                        hoverInterface.SetActive(false);
                    };
                }
            }
        }
    }
}