using System.Linq;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory_System
{
    public class UIEquipmentInventory : MonoBehaviour
    {
        private EquipmentInventory _inventory;
        private Transform          _itemSlotContainer;
        private Player.Player      _player;
        public  GameObject         hoverInterface;

        private void Awake()
        {
            _itemSlotContainer = transform.Find("itemSlotContainer");
        }

        public void SetPlayer(Player.Player player)
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
                switch (item)
                {
                    case WeaponItem:
                    {
                        var weaponSlot      = _itemSlotContainer.Find("weaponSlot");
                        var weaponSlotImage = weaponSlot.Find("image").GetComponent<Image>();
                        weaponSlot.GetComponent<Button_UI>().enabled = true;
                        weaponSlotImage.sprite = item.GetSprite();
                        weaponSlotImage.gameObject.SetActive(true);

                        weaponSlot.GetComponent<Button_UI>().MouseRightClickFunc = () =>
                        {
                            // Un-equip the item
                            CursorController.Instance.SetCursor(CursorController.CursorTypes.Dequip);
                            _player.GetComponent<PlayerEquipmentManager>().UnEquip(item);
                            weaponSlot.GetComponent<Button_UI>().enabled = false;
                            weaponSlotImage.gameObject.SetActive(false);
                        };

                        // Display item stats when hovering over
                        weaponSlot.GetComponent<Button_UI>().MouseOverOnceTooltipFunc = () =>
                        {
                            GenerateItemTooltip(item, weaponSlot.position);
                        };

                        weaponSlot.GetComponent<Button_UI>().MouseOutOnceTooltipFunc = () =>
                        {
                            UI_Inventory.Instance.hoverInterface.SetActive(false);
                        };
                        break;
                    }
                    case HelmetItem:
                    {
                        var headSlot      = _itemSlotContainer.Find("headSlot");
                        var headSlotImage = headSlot.Find("image").GetComponent<Image>();
                        headSlot.GetComponent<Button_UI>().enabled = true;
                        headSlotImage.sprite = item.GetSprite();
                        headSlotImage.gameObject.SetActive(true);

                        headSlot.GetComponent<Button_UI>().MouseRightClickFunc = () =>
                        {
                            // Un-equip the item
                            _player.GetComponent<PlayerEquipmentManager>().UnEquip(item);
                            headSlot.GetComponent<Button_UI>().enabled = false;
                            headSlotImage.gameObject.SetActive(false);
                        };

                        // Display item stats when hovering over
                        headSlot.GetComponent<Button_UI>().MouseOverOnceTooltipFunc = () => { GenerateItemTooltip(item, headSlot.position); };

                        headSlot.GetComponent<Button_UI>().MouseOutOnceTooltipFunc = () =>
                        {
                            UI_Inventory.Instance.hoverInterface.SetActive(false);
                        };
                        break;
                    }
                    case ChestItem:
                    {
                        var chestSlot      = _itemSlotContainer.Find("chestSlot");
                        var chestSlotImage = chestSlot.Find("image").GetComponent<Image>();
                        chestSlot.GetComponent<Button_UI>().enabled = true;
                        chestSlotImage.sprite = item.GetSprite();
                        chestSlotImage.gameObject.SetActive(true);

                        chestSlot.GetComponent<Button_UI>().MouseRightClickFunc = () =>
                        {
                            // Un-equip the item
                            _player.GetComponent<PlayerEquipmentManager>().UnEquip(item);
                            chestSlot.GetComponent<Button_UI>().enabled = false;
                            chestSlotImage.gameObject.SetActive(false);
                        };

                        // Display item stats when hovering over
                        chestSlot.GetComponent<Button_UI>().MouseOverOnceTooltipFunc = () =>
                        {
                            GenerateItemTooltip(item, chestSlot.position);
                        };

                        chestSlot.GetComponent<Button_UI>().MouseOutOnceTooltipFunc = () =>
                        {
                            UI_Inventory.Instance.hoverInterface.SetActive(false);
                        };
                        break;
                    }
                    case BootItem:
                    {
                        var bootsSlot      = _itemSlotContainer.Find("bootsSlot");
                        var bootsSlotImage = bootsSlot.Find("image").GetComponent<Image>();
                        bootsSlot.GetComponent<Button_UI>().enabled = true;
                        bootsSlotImage.sprite = item.GetSprite();
                        bootsSlotImage.gameObject.SetActive(true);

                        bootsSlot.GetComponent<Button_UI>().MouseRightClickFunc = () =>
                        {
                            // Un-equip the item
                            _player.GetComponent<PlayerEquipmentManager>().UnEquip(item);
                            bootsSlot.GetComponent<Button_UI>().enabled = false;
                            bootsSlotImage.gameObject.SetActive(false);
                        };

                        // Display item stats when hovering over
                        bootsSlot.GetComponent<Button_UI>().MouseOverOnceTooltipFunc = () =>
                        {
                            GenerateItemTooltip(item, bootsSlot.position);
                        };

                        bootsSlot.GetComponent<Button_UI>().MouseOutOnceTooltipFunc = () =>
                        {
                            UI_Inventory.Instance.hoverInterface.SetActive(false);
                        };
                        break;
                    }
                }
            }
        }

        private void GenerateItemTooltip(Item item, Vector3 slotPosition)
        {
            UI_Inventory.Instance.hoverInterface.SetActive(true);
            UI_Inventory.Instance.hoverInterface.GetComponent<Image>().sprite = UI_Inventory.Instance.itemRarityHoverImages
                .First(key => key.key.ToLower().Equals($"{item.itemRarity}".ToLower())).image;
            
            UI_Inventory.Instance.hoverInterface.transform.position = slotPosition
                + new Vector3(0, UI_Inventory.Instance.hoverInterface.GetComponent<Image>().sprite.rect.yMax - 20, 0);
            
            CursorController.Instance.SetCursor(CursorController.CursorTypes.Equip);
            var itemName = UI_Inventory.Instance.hoverInterface.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
            var itemDescription =
                UI_Inventory.Instance.hoverInterface.transform.Find("itemDescription").GetComponent<TextMeshProUGUI>();

            // Set item name
            itemName.SetText($"{item.itemName}");
            // Check for certain keywords and add colour to them
            itemDescription.SetText($"{UI_Inventory.GetItemDescription(item)}");
            // Set the stats for the item
            // Set the stats for the item
            UI_Inventory.Instance.SetItemStats(item);
            UI_Inventory.Instance.SetItemRequirements(item);
        }
    }
}