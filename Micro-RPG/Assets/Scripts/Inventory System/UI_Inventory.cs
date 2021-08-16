using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory_System
{
    public class UI_Inventory : MonoBehaviour
    {
        private Inventory  _inventory;
        private Transform  _itemSlotContainer;
        private Transform  _itemSlotTemplate;
        private Player     _player;
        public  GameObject hoverInterface;

        private void Awake()
        {
            _itemSlotContainer = transform.Find("itemSlotContainer");
            _itemSlotTemplate = _itemSlotContainer.Find("itemSlotTemplate");
        }

        public void SetPlayer(Player player)
        {
            _player = player;
        }
        
        public void SetInventory(Inventory inventory)
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
            foreach (Transform child in _itemSlotContainer)
            {
                if (child == _itemSlotTemplate) continue;
                Destroy(child.gameObject);
            }
            var x                = 0;
            var y                = 0;
            var itemSlotCellSize = 80f;
            
            foreach (var item in _inventory.GetItemList())
            {
                var itemSlotRectTransform =
                    Instantiate(_itemSlotTemplate, _itemSlotContainer).GetComponent<RectTransform>();
                itemSlotRectTransform.gameObject.SetActive(true);
                
                // TODO: Might want to use a grid component instead
                
                // Action when left-clicking on an item
                itemSlotRectTransform.GetComponent<Button_UI>().ClickFunc = () =>
                {
                    // Use item
                    _inventory.UseItem(item);
                };
                
                // Action when right-clicking
                itemSlotRectTransform.GetComponent<Button_UI>().MouseRightClickFunc = () =>
                {
                    // Drop Item
                    var duplicateItem = item;
                    _inventory.RemoveItem(item);
                    ItemWorld.DropItem(_player.transform.position, duplicateItem);
                };
                
                // Display item stats when hovering over
                itemSlotRectTransform.GetComponent<Button_UI>().MouseOverOnceTooltipFunc = () =>
                {
                    hoverInterface.SetActive(true);
                    var itemName = hoverInterface.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
                    var itemDescription =
                        hoverInterface.transform.Find("itemDescription").GetComponent<TextMeshProUGUI>();

                    // Set item name
                    itemName.SetText($"{item.itemName}");
                    // Check for certain keywords and add colour to them
                    itemDescription.SetText($"{GetItemDescription(item)}");
                    // Set the stats for the item
                    SetItemStats(item);
                };

                itemSlotRectTransform.GetComponent<Button_UI>().MouseOutOnceTooltipFunc = () =>
                {
                    hoverInterface.SetActive(false);
                };
                
                itemSlotRectTransform.anchoredPosition = new Vector2(x * itemSlotCellSize, y * itemSlotCellSize);
                
                var image = itemSlotRectTransform.Find("image").GetComponent<Image>();
                image.sprite = item.GetSprite();

                var uiText = itemSlotRectTransform.Find("amountText").GetComponent<TextMeshProUGUI>();
                uiText.SetText(item.amount > 1 ? item.amount.ToString() : "");
                x++;
                if (x > 4)
                {
                    x = 0;
                    y--;
                }
            }
        }

        private void SetItemStats(Item item)
        {
            var itemStats = hoverInterface.transform.Find("ItemStats").GetComponent<TextMeshProUGUI>();
            
            switch (item)
            {
                case WeaponItem weaponItem:
                    var damageString    = $"Damage: {weaponItem.damage}";
                    var rangeString     = $"Range: {weaponItem.weaponRange}";
                    var playerEquipment = _player.GetComponent<PlayerEquipmentManager>();
                    if (playerEquipment.hasWeaponEquipped)
                    {
                        var equippedItem = playerEquipment.weaponItem;
                        
                        // Weapon Damage
                        if (equippedItem.damage > weaponItem.damage)
                        {
                            damageString = damageString.Replace($"{weaponItem.damage}",
                                $"<color=red>{weaponItem.damage}</color> <size=75%>-{equippedItem.damage - weaponItem.damage}</size>");
                        }
                        if (equippedItem.damage < weaponItem.damage)
                        {
                            damageString = damageString.Replace($"{weaponItem.damage}",
                                $"<color=green>{weaponItem.damage}</color> <size=75%>+{weaponItem.damage - equippedItem.damage}</size>");
                        }
                        
                        // Weapon Range
                        if (equippedItem.weaponRange > weaponItem.weaponRange)
                        {
                            rangeString = rangeString.Replace($"{weaponItem.weaponRange}",
                                $"<color=red>{weaponItem.weaponRange}</color> <size=75%>-{equippedItem.weaponRange - weaponItem.weaponRange}</size>");
                        }
                        if (equippedItem.weaponRange < weaponItem.weaponRange)
                        {
                            rangeString = rangeString.Replace($"{weaponItem.weaponRange}",
                                $"<color=green>{weaponItem.weaponRange}</color> <size=75%>+{weaponItem.weaponRange - equippedItem.weaponRange}</size>");
                        }
                    }
                    itemStats.SetText($"{damageString}\n"+
                                      $"{rangeString}");
                    break;
                case HealthPotion healthPotion:
                    itemStats.SetText($"Restore Amount: {healthPotion.restoreAmount}");
                    break;
                case ManaPotion manaPotion:
                    itemStats.SetText($"Restore Amount: {manaPotion.restoreAmount}");
                    break;
                case Coin _:
                    itemStats.SetText("");
                    break;
            }
        }

        private static string GetItemDescription(Item item)
        {
            return item switch
            {
                HealthPotion healthPotion => healthPotion.itemDescription.Replace("HP", "<color=red>HP</color>"),
                ManaPotion manaPotion => manaPotion.itemDescription.Replace("Mana", "<color=blue>Mana</color>"),
                _ => item.itemDescription
            };
        }
    }
}
