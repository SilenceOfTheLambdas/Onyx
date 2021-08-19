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
                    SetItemRequirements(item);
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

        private void SetItemRequirements(Item item)
        {
            var itemRequirementsText =
                hoverInterface.transform.Find("itemRequirements").GetComponent<TextMeshProUGUI>();
            var playerEquipment = _player.GetComponent<PlayerEquipmentManager>();
            switch (item)
            {
                case HelmetItem helmetItem:
                    var requirementString = $"Intelligence: {helmetItem.intelligenceRequirement}    Strength: {helmetItem.strengthRequirement}";
                    if (_player.intelligence < helmetItem.intelligenceRequirement)
                        requirementString = requirementString.Replace($"{helmetItem.intelligenceRequirement}",
                            $"<color=red>{helmetItem.intelligenceRequirement}</color>");
                    if (_player.strength < helmetItem.strengthRequirement)
                        requirementString = requirementString.Replace($"{helmetItem.strengthRequirement}",
                            $"<color=red>{helmetItem.strengthRequirement}</color>");
                    
                    itemRequirementsText.SetText($"{requirementString}");
                    break;
            }
        }

        private void SetItemStats(Item item)
        {
            var    itemStats       = hoverInterface.transform.Find("ItemStats").GetComponent<TextMeshProUGUI>();
            var    playerEquipment = _player.GetComponent<PlayerEquipmentManager>();
            string physicalArmourString;
            string elementalArmourString;
            string healthString;
            string manaString;
            string strengthString;
            string intelligenceString;
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
                    physicalArmourString = $"Physical Armour: {helmetItem.physicalArmour}";
                    elementalArmourString = $"Elemental Armour: {helmetItem.elementalArmour}";
                    healthString = $"Health: {helmetItem.healthAmount}";
                    manaString = $"Mana: {helmetItem.manaAmount}";
                    strengthString = $"Strength: {helmetItem.strengthAmount}";
                    intelligenceString = $"Intelligence: {helmetItem.intelligenceAmount}";
                    var manaRegenerationAmount = $"Mana Regen: {helmetItem.manaRegenerationPercentage}%";

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

                        // Mana
                        if (equippedItem.manaRegenerationPercentage > helmetItem.manaRegenerationPercentage)
                        {
                            manaRegenerationAmount = manaRegenerationAmount.Replace(
                                $"{helmetItem.manaRegenerationPercentage}",
                                $"<color=red>{helmetItem.manaRegenerationPercentage}%</color> <size=75%>-{equippedItem.manaRegenerationPercentage - helmetItem.manaRegenerationPercentage}%</size>");
                        }

                        if (equippedItem.manaRegenerationPercentage < helmetItem.manaRegenerationPercentage)
                        {
                            manaRegenerationAmount = manaRegenerationAmount.Replace(
                                $"{helmetItem.manaRegenerationPercentage}",
                                $"<color=green>{helmetItem.manaRegenerationPercentage}%</color> <size=75%>+{helmetItem.manaRegenerationPercentage - equippedItem.manaRegenerationPercentage}%</size>");
                        }
                    }
                    
                    itemStats.SetText($"{physicalArmourString}\n" +
                                      $"{elementalArmourString}\n" +
                                      $"{healthString}\n" +
                                      $"{manaString}\n" +
                                      $"{strengthString}\n" +
                                      $"{intelligenceString}\n" +
                                      $"{manaRegenerationAmount}");
                    break;
                case ChestItem chestItem:
                    physicalArmourString = $"Physical Armour: {chestItem.physicalArmour}";
                    elementalArmourString = $"Elemental Armour: {chestItem.elementalArmour}";
                    healthString = $"Health: {chestItem.healthAmount}";
                    manaString = $"Mana: {chestItem.manaAmount}";
                    strengthString = $"Strength: {chestItem.strengthAmount}";
                    intelligenceString = $"Intelligence: {chestItem.intelligenceAmount}";
                    var healthOnHitAmountString = $"Health On Hit: {chestItem.healthOnHitAmount}";
                    var weaponRangeString       = $"Weapon Range: {chestItem.additionalWeaponRangeAmount}";
                    var reducedManaCostString   = $"Skills Mana Cost: {chestItem.reducedManaCostOfSkillsAmount}";
                    
                    if (playerEquipment.chest != null)
                    {
                        var equippedItem = playerEquipment.chest;

                        // Helmet physical armour
                        if (equippedItem.physicalArmour > chestItem.physicalArmour)
                        {
                            physicalArmourString = physicalArmourString.Replace($"{chestItem.physicalArmour}",
                                $"<color=red>{chestItem.physicalArmour}</color> <size=75%>-{equippedItem.physicalArmour - chestItem.physicalArmour}</size>");
                        }

                        if (equippedItem.physicalArmour < chestItem.physicalArmour)
                        {
                            physicalArmourString = physicalArmourString.Replace($"{chestItem.physicalArmour}",
                                $"<color=green>{chestItem.physicalArmour}</color> <size=75%>+{chestItem.physicalArmour - equippedItem.physicalArmour}</size>");
                        }

                        // Helmet elemental armour
                        if (equippedItem.elementalArmour > chestItem.elementalArmour)
                        {
                            elementalArmourString = elementalArmourString.Replace($"{chestItem.elementalArmour}",
                                $"<color=red>{chestItem.elementalArmour}</color> <size=75%>-{equippedItem.elementalArmour - chestItem.elementalArmour}</size>");
                        }

                        if (equippedItem.elementalArmour < chestItem.elementalArmour)
                        {
                            elementalArmourString = elementalArmourString.Replace($"{chestItem.elementalArmour}",
                                $"<color=green>{chestItem.elementalArmour}</color> <size=75%>+{chestItem.elementalArmour - equippedItem.elementalArmour}</size>");
                        }
                        
                        // Helmet health
                        if (equippedItem.healthAmount > chestItem.healthAmount)
                        {
                            healthString = healthString.Replace($"{chestItem.healthAmount}",
                                $"<color=red>{chestItem.healthAmount}</color> <size=75%>{chestItem.healthAmount - equippedItem.healthAmount}</size>");
                        }

                        if (equippedItem.healthAmount < chestItem.healthAmount)
                        {
                            healthString = healthString.Replace($"{chestItem.healthAmount}",
                                $"<color=green>{chestItem.healthAmount}</color> <size=75%>+{chestItem.healthAmount - equippedItem.healthAmount}</size>");
                        }
                        
                        // Helmet mana
                        if (equippedItem.manaAmount > chestItem.manaAmount)
                        {
                            manaString = manaString.Replace($"{chestItem.manaAmount}",
                                $"<color=red>{chestItem.manaAmount}</color> <size=75%>{chestItem.manaAmount - equippedItem.manaAmount}</size>");
                        }

                        if (equippedItem.manaAmount < chestItem.manaAmount)
                        {
                            manaString = manaString.Replace($"{chestItem.manaAmount}",
                                $"<color=green>{chestItem.manaAmount}</color> <size=75%>+{chestItem.manaAmount - equippedItem.manaAmount}</size>");
                        }
                        
                        // Helmet strength
                        if (equippedItem.strengthAmount > chestItem.strengthAmount)
                        {
                            strengthString = strengthString.Replace($"{chestItem.strengthAmount}",
                                $"<color=red>{chestItem.strengthAmount}</color> <size=75%>{chestItem.strengthAmount - equippedItem.strengthAmount}</size>");
                        }

                        if (equippedItem.strengthAmount < chestItem.strengthAmount)
                        {
                            strengthString = strengthString.Replace($"{chestItem.strengthAmount}",
                                $"<color=green>{chestItem.strengthAmount}</color> <size=75%>+{chestItem.strengthAmount - equippedItem.strengthAmount}</size>");
                        }
                        
                        // Helmet intelligence
                        if (equippedItem.intelligenceAmount > chestItem.intelligenceAmount)
                        {
                            intelligenceString = intelligenceString.Replace($"{chestItem.intelligenceAmount}",
                                $"<color=red>{chestItem.intelligenceAmount}</color> <size=75%>{chestItem.intelligenceAmount - equippedItem.intelligenceAmount}</size>");
                        }

                        if (equippedItem.intelligenceAmount < chestItem.intelligenceAmount)
                        {
                            intelligenceString = intelligenceString.Replace($"{chestItem.intelligenceAmount}",
                                $"<color=green>{chestItem.intelligenceAmount}</color> <size=75%>+{chestItem.intelligenceAmount - equippedItem.intelligenceAmount}</size>");
                        }

                        // Health on Hit Amount
                        if (equippedItem.healthOnHitAmount > chestItem.healthOnHitAmount)
                        {
                            healthOnHitAmountString = healthOnHitAmountString.Replace(
                                $"{chestItem.healthOnHitAmount}",
                                $"<color=red>{chestItem.healthOnHitAmount}</color> <size=75%>-{equippedItem.healthOnHitAmount - chestItem.healthOnHitAmount}</size");
                        }

                        if (equippedItem.healthOnHitAmount < chestItem.healthOnHitAmount)
                        {
                            healthOnHitAmountString = healthOnHitAmountString.Replace(
                                $"{chestItem.healthOnHitAmount}",
                                $"<color=green>{chestItem.healthOnHitAmount}</color> <size=75%>+{chestItem.healthOnHitAmount - equippedItem.healthOnHitAmount}</size>");
                        }
                        
                        // Additional Weapon Range
                        if (equippedItem.additionalWeaponRangeAmount > chestItem.additionalWeaponRangeAmount)
                        {
                            weaponRangeString = weaponRangeString.Replace(
                                $"{chestItem.additionalWeaponRangeAmount}",
                                $"<color=red>{chestItem.additionalWeaponRangeAmount}</color> <size=75%>-{equippedItem.additionalWeaponRangeAmount - chestItem.additionalWeaponRangeAmount}</size");
                        }

                        if (equippedItem.additionalWeaponRangeAmount < chestItem.additionalWeaponRangeAmount)
                        {
                            weaponRangeString = weaponRangeString.Replace(
                                $"{chestItem.additionalWeaponRangeAmount}",
                                $"<color=green>{chestItem.additionalWeaponRangeAmount}</color> <size=75%>+{chestItem.additionalWeaponRangeAmount - equippedItem.additionalWeaponRangeAmount}</size>");
                        }
                        
                        // Reduced Mana Cost of skills
                        if (equippedItem.reducedManaCostOfSkillsAmount > chestItem.reducedManaCostOfSkillsAmount)
                        {
                            reducedManaCostString = reducedManaCostString.Replace(
                                $"{chestItem.reducedManaCostOfSkillsAmount}",
                                $"<color=red>{chestItem.reducedManaCostOfSkillsAmount}%</color> <size=75%>-{equippedItem.reducedManaCostOfSkillsAmount - chestItem.reducedManaCostOfSkillsAmount}</size");
                        }

                        if (equippedItem.reducedManaCostOfSkillsAmount < chestItem.reducedManaCostOfSkillsAmount)
                        {
                            reducedManaCostString = reducedManaCostString.Replace(
                                $"{chestItem.reducedManaCostOfSkillsAmount}",
                                $"<color=green>{chestItem.reducedManaCostOfSkillsAmount}%</color> <size=75%>+{chestItem.reducedManaCostOfSkillsAmount - equippedItem.reducedManaCostOfSkillsAmount}</size>");
                        }
                    }
                    
                    itemStats.SetText($"{physicalArmourString}\n" +
                                      $"{elementalArmourString}\n" +
                                      $"{healthString}\n" +
                                      $"{manaString}\n" +
                                      $"{strengthString}\n" +
                                      $"{intelligenceString}\n" +
                                      $"{healthOnHitAmountString}\n" +
                                      $"{weaponRangeString}\n" +
                                      $"{reducedManaCostString}");
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