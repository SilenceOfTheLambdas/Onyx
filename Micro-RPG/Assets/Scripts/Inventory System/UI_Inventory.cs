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
            var itemStats       = hoverInterface.transform.Find("ItemStats").GetComponent<TextMeshProUGUI>();
            var playerEquipment = _player.GetComponent<PlayerEquipmentManager>();
            switch (item)
            {
                case WeaponItem weaponItem:
                    var damageString      = $"Damage: {weaponItem.damage}";
                    var rangeString       = $"Range: {weaponItem.weaponRange}";
                    var attackSpeedString = $"Attack Rate: {weaponItem.attackRate}";
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
                        
                        // Weapon attack speed
                        if (equippedItem.attackRate < weaponItem.attackRate)
                        {
                            attackSpeedString = attackSpeedString.Replace($"{weaponItem.attackRate}",
                                $"<color=red>{weaponItem.attackRate}</color> <size=75%>+{weaponItem.attackRate - equippedItem.attackRate}</size>");
                        }

                        if (equippedItem.attackRate > weaponItem.attackRate)
                        {
                            attackSpeedString = attackSpeedString.Replace($"{weaponItem.attackRate}",
                                $"<color=green>{weaponItem.attackRate}</color> <size=75%>-{equippedItem.attackRate - weaponItem.attackRate}</size>");
                        }
                    }

                    itemStats.SetText($"{damageString}\n" +
                                      $"{rangeString}\n" +
                                      $"{attackSpeedString}");
                    break;
                case HelmetItem helmetItem:
                    var physicalArmourString  = $"Physical Armour: {helmetItem.physicalArmour}";
                    var elementalArmourString = $"Elemental Armour: {helmetItem.elementalArmour}";
                    var healthString          = $"Health: {helmetItem.healthAmount}";
                    var manaString            = $"Mana: {helmetItem.manaAmount}";
                    var strengthString        = $"Strength: {helmetItem.strengthAmount}";
                    var intelligenceString    = $"Intelligence: {helmetItem.intelligenceAmount}";

                    if (playerEquipment.head != null)
                    {
                        var equippedItem = playerEquipment.head;

                        // Helmet physical armour
                        if (equippedItem.physicalArmour > helmetItem.physicalArmour)
                        {
                            physicalArmourString = physicalArmourString.Replace($"{helmetItem.physicalArmour}",
                                $"<color=red>{helmetItem.physicalArmour}</color> <size=75%>-{equippedItem.physicalArmour - helmetItem.physicalArmour}</size>");
                        }

                        if (equippedItem.physicalArmour < helmetItem.physicalArmour)
                        {
                            physicalArmourString = physicalArmourString.Replace($"{helmetItem.physicalArmour}",
                                $"<color=green>{helmetItem.physicalArmour}</color> <size=75%>+{helmetItem.physicalArmour - equippedItem.physicalArmour}</size>");
                        }

                        // Helmet elemental armour
                        if (equippedItem.elementalArmour > helmetItem.elementalArmour)
                        {
                            elementalArmourString = elementalArmourString.Replace($"{helmetItem.elementalArmour}",
                                $"<color=red>{helmetItem.elementalArmour}</color> <size=75%>-{equippedItem.elementalArmour - helmetItem.elementalArmour}</size>");
                        }

                        if (equippedItem.elementalArmour < helmetItem.elementalArmour)
                        {
                            elementalArmourString = elementalArmourString.Replace($"{helmetItem.elementalArmour}",
                                $"<color=green>{helmetItem.elementalArmour}</color> <size=75%>+{helmetItem.elementalArmour - equippedItem.elementalArmour}</size>");
                        }
                        
                        // Helmet health
                        if (equippedItem.healthAmount > helmetItem.healthAmount)
                        {
                            healthString = healthString.Replace($"{helmetItem.healthAmount}",
                                $"<color=red>{helmetItem.healthAmount}</color> <size=75%>{helmetItem.healthAmount - equippedItem.healthAmount}</size>");
                        }

                        if (equippedItem.healthAmount < helmetItem.healthAmount)
                        {
                            healthString = healthString.Replace($"{helmetItem.healthAmount}",
                                $"<color=green>{helmetItem.healthAmount}</color> <size=75%>+{helmetItem.healthAmount - equippedItem.healthAmount}</size>");
                        }
                        
                        // Helmet mana
                        if (equippedItem.manaAmount > helmetItem.manaAmount)
                        {
                            manaString = manaString.Replace($"{helmetItem.manaAmount}",
                                $"<color=red>{helmetItem.manaAmount}</color> <size=75%>{helmetItem.manaAmount - equippedItem.manaAmount}</size>");
                        }

                        if (equippedItem.manaAmount < helmetItem.manaAmount)
                        {
                            manaString = manaString.Replace($"{helmetItem.manaAmount}",
                                $"<color=green>{helmetItem.manaAmount}</color> <size=75%>+{helmetItem.manaAmount - equippedItem.manaAmount}</size>");
                        }
                        
                        // Helmet strength
                        if (equippedItem.strengthAmount > helmetItem.strengthAmount)
                        {
                            strengthString = strengthString.Replace($"{helmetItem.strengthAmount}",
                                $"<color=red>{helmetItem.strengthAmount}</color> <size=75%>{helmetItem.strengthAmount - equippedItem.strengthAmount}</size>");
                        }

                        if (equippedItem.strengthAmount < helmetItem.strengthAmount)
                        {
                            strengthString = strengthString.Replace($"{helmetItem.strengthAmount}",
                                $"<color=green>{helmetItem.strengthAmount}</color> <size=75%>+{helmetItem.strengthAmount - equippedItem.strengthAmount}</size>");
                        }
                        
                        // Helmet intelligence
                        if (equippedItem.intelligenceAmount > helmetItem.intelligenceAmount)
                        {
                            intelligenceString = intelligenceString.Replace($"{helmetItem.intelligenceAmount}",
                                $"<color=red>{helmetItem.intelligenceAmount}</color> <size=75%>{helmetItem.intelligenceAmount - equippedItem.intelligenceAmount}</size>");
                        }

                        if (equippedItem.intelligenceAmount < helmetItem.intelligenceAmount)
                        {
                            intelligenceString = intelligenceString.Replace($"{helmetItem.intelligenceAmount}",
                                $"<color=green>{helmetItem.intelligenceAmount}</color> <size=75%>+{helmetItem.intelligenceAmount - equippedItem.intelligenceAmount}</size>");
                        }
                    }
                    
                    itemStats.SetText($"{physicalArmourString}\n" +
                                      $"{elementalArmourString}\n" +
                                      $"{healthString}\n" +
                                      $"{manaString}\n" +
                                      $"{strengthString}\n" +
                                      $"{intelligenceString}");
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
            var description = item.itemDescription.Replace("HP", "<color=red>HP</color>");
            description = description.Replace("Mana", "<color=blue>Mana</color>");
            
            return description;
        }
    }
}