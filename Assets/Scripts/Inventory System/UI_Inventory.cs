using System;
using System.Collections.Generic;
using System.Linq;
using Player;
using SuperuserUtils;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Inventory_System
{
    public class UI_Inventory : GenericSingletonClass<UI_Inventory>
    {
        private                  Inventory                  _inventory;
        [SerializeField] private Transform                  itemSlotContainer;
        [SerializeField] private Transform                  itemSlotTemplate;
        [SerializeField] private Sprite                     itemSlotBackgroundFocused;
        [SerializeField] public  Vector2                    hoverOverlayPositionOffset;
        public                   GameObject                 hoverInterface;
        public                   List<ItemRarityHoverImage> itemRarityHoverImages;
        private                  Player.Player              _player;
        private                  AbilitiesSystem            _playerAbilitySystem;
        private                  Sprite                     _itemSlotBackgroundOriginal;

        private void Start()
        {
            _itemSlotBackgroundOriginal = itemSlotTemplate.Find("background").GetComponent<Image>().sprite;
        }

        /// <summary>
        /// Set the player, and the ability system variable
        /// </summary>
        /// <param name="player">The player</param>
        public void SetPlayer(Player.Player player)
        {
            _player = player;
            _playerAbilitySystem = _player.GetComponent<AbilitiesSystem>();
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
            foreach (Transform child in itemSlotContainer)
            {
                if (child == itemSlotTemplate) continue;
                Destroy(child.gameObject);
            }

            var x                = 0;
            var y                = 0;
            var itemSlotCellSize = 80f;

            foreach (var item in _inventory.GetItemList())
            {
                var itemSlotRectTransform =
                    Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
                itemSlotRectTransform.gameObject.SetActive(true);

                // Action when left-clicking on an item
                itemSlotRectTransform.GetComponent<Button_UI>().ClickFunc = () =>
                {
                    // Use item
                    _inventory.UseItem(item);
                };

                // Action when right-clicking
                itemSlotRectTransform.GetComponent<Button_UI>().MouseRightClickFunc = () =>
                {
                    // Ask for confirmation to destroy item
                    QuestionDialog.Instance.ShowQuestion("Are you sure you want to delete this item?", () =>
                    {
                        // Delete the item
                        _inventory.RemoveItem(item);
                    }, () => {});
                };

                // Display item stats when hovering over
                itemSlotRectTransform.GetComponent<Button_UI>().MouseOverOnceTooltipFunc = () =>
                {
                    hoverInterface.SetActive(true);
                    
                    itemSlotRectTransform.Find("background").GetComponent<Image>().sprite =
                        itemSlotBackgroundFocused;

                    hoverInterface.GetComponent<Image>().sprite = itemRarityHoverImages
                        .First(key => key.key.ToLower().Equals($"{item.itemRarity}".ToLower())).image;

                    hoverInterface.transform.position = itemSlotRectTransform.position + (Vector3) hoverOverlayPositionOffset;
                    CursorController.Instance.SetCursor(CursorController.CursorTypes.Equip);
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
                    itemSlotRectTransform.Find("background").GetComponent<Image>().sprite = _itemSlotBackgroundOriginal;
                    CursorController.Instance.SetCursor(CursorController.CursorTypes.Default);
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

        public void SetItemRequirements(Item item)
        {
            var itemRequirementsText =
                hoverInterface.transform.Find("itemRequirements").GetComponent<TextMeshProUGUI>();
            string requirementString;
            switch (item)
            {
                case ArmourItem armourItem:
                    requirementString = $"Level:{armourItem.levelRequirement} Intelligence:{armourItem.intelligenceRequirement} Strength:{armourItem.strengthRequirement}";
                    if (_playerAbilitySystem.curLevel < armourItem.levelRequirement)
                        requirementString = requirementString.Replace($"{armourItem.levelRequirement}",
                            $"<color=red>{armourItem.levelRequirement}</color>");
                    if (_playerAbilitySystem.intelligence < armourItem.intelligenceRequirement)
                        requirementString = requirementString.Replace($"{armourItem.intelligenceRequirement}",
                            $"<color=red>{armourItem.intelligenceRequirement}</color>");
                    if (_playerAbilitySystem.strength < armourItem.strengthRequirement)
                        requirementString = requirementString.Replace($"{armourItem.strengthRequirement}",
                            $"<color=red>{armourItem.strengthRequirement}</color>");
                    
                    itemRequirementsText.SetText($"{requirementString}");
                    break;
                case WeaponItem weaponItem:
                    requirementString = $"Level:{weaponItem.levelRequirement} Intelligence:{weaponItem.intelligenceRequirement} Strength:{weaponItem.strengthRequirement}";
                    if (_playerAbilitySystem.curLevel < weaponItem.levelRequirement)
                        requirementString = requirementString.Replace($"{weaponItem.levelRequirement}",
                            $"<color=red>{weaponItem.levelRequirement}</color>");
                    if (_playerAbilitySystem.intelligence < weaponItem.intelligenceRequirement)
                        requirementString = requirementString.Replace($"{weaponItem.intelligenceRequirement}",
                            $"<color=red>{weaponItem.intelligenceRequirement}</color>");
                    if (_playerAbilitySystem.strength < weaponItem.strengthRequirement)
                        requirementString = requirementString.Replace($"{weaponItem.strengthRequirement}",
                            $"<color=red>{weaponItem.strengthRequirement}</color>");
                    
                    itemRequirementsText.SetText($"{requirementString}");
                    break;
            }
        }

        public void SetItemStats(Item item)
        {
            var    itemStats       = hoverInterface.transform.Find("ItemStats").GetComponent<TextMeshProUGUI>();
            itemStats.SetText(""); // Reset Item Stats Text Box
            var    playerEquipment = _player.GetComponent<PlayerEquipmentManager>();
            switch (item)
            {
                case WeaponItem weaponItem:
                    var damageString         = $"<b>Damage:</b> {weaponItem.damage}";
                    var rangeString          = $"<b>Range:</b> {weaponItem.weaponRange:F}".Replace("-", "");
                    var attackSpeedString    = $"<b>Attack Rate:</b> {weaponItem.attackRate:F}";
                    var specialAbilityString = $"<b>Unique Ability:</b> <i>{weaponItem.specialAbility.ability.ToString()}</i>";
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
                                $"<color=green>{weaponItem.damage:F}</color> <size=75%>+{weaponItem.damage - equippedItem.damage}</size>");
                        }

                        // Weapon Range
                        if (equippedItem.weaponRange > weaponItem.weaponRange)
                        {
                            rangeString = rangeString.Replace($"{weaponItem.weaponRange:F}",
                                $"<color=red>{weaponItem.weaponRange:F}</color> <size=75%>-{(float)equippedItem.weaponRange - (float)weaponItem.weaponRange:F}</size>");
                        }

                        if (equippedItem.weaponRange < weaponItem.weaponRange)
                        {
                            rangeString = rangeString.Replace($"{weaponItem.weaponRange:F}",
                                $"<color=green>{weaponItem.weaponRange:F}</color> <size=75%>+{weaponItem.weaponRange - equippedItem.weaponRange:F}</size>");
                        }

                        // Weapon attack speed
                        if (equippedItem.attackRate > weaponItem.attackRate)
                        {
                            attackSpeedString = attackSpeedString.Replace($"{weaponItem.attackRate:F}",
                                $"<color=red>{weaponItem.attackRate:F}</color> <size=75%>+{weaponItem.attackRate - equippedItem.attackRate:F}</size>");
                        }

                        if (equippedItem.attackRate < weaponItem.attackRate)
                        {
                            attackSpeedString = attackSpeedString.Replace($"{weaponItem.attackRate:F}",
                                $"<color=green>{weaponItem.attackRate:F}</color> <size=75%>-{equippedItem.attackRate - weaponItem.attackRate:F}</size>");
                        }
                    }

                    if (weaponItem.itemRarity == ItemRarity.Unique)
                    {
                        itemStats.SetText($"{specialAbilityString}\n" +
                                          $"{damageString}\n" +
                                          $"{rangeString}\n" +
                                          $"{attackSpeedString}\n");
                    }
                    else
                    {
                        itemStats.SetText($"{damageString}\n" +
                                          $"{rangeString}\n" +
                                          $"{attackSpeedString}");
                    }
                    break;
                case ArmourItem armourItem:
                    var physicalArmourString  = $"Physical Armour: {armourItem.physicalArmour}";
                    var elementalArmourString = $"Elemental Armour: {armourItem.elementalArmour}";
                    var healthString          = $"Health: {armourItem.healthAmount}";
                    var manaString            = $"Mana: {armourItem.manaAmount}";
                    var strengthString        = $"Strength: {armourItem.strengthAmount}";
                    var intelligenceString    = $"Intelligence: {armourItem.intelligenceAmount}";
                    var specialAbility        = $"Unique Ability: {armourItem.specialAbility.ability.ToString()}";

                    switch (armourItem)
                    {
                        case HelmetItem helmetItem:
                        {
                            var manaRegenerationAmount = $"Mana Regen: {helmetItem.manaRegenerationPercentage}%";
                            var reducedManaCostString  = $"Skills Mana Cost: {helmetItem.reducedManaCostOfSkillsAmount}";

                            // ## If we already have a helmet equipped, compare. ##
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
                                        $"<color=red>{helmetItem.manaRegenerationPercentage}</color> <size=75%>-{equippedItem.manaRegenerationPercentage - helmetItem.manaRegenerationPercentage}%</size>");
                                }

                                if (equippedItem.manaRegenerationPercentage < helmetItem.manaRegenerationPercentage)
                                {
                                    manaRegenerationAmount = manaRegenerationAmount.Replace(
                                        $"{helmetItem.manaRegenerationPercentage}",
                                        $"<color=green>{helmetItem.manaRegenerationPercentage}</color> <size=75%>+{helmetItem.manaRegenerationPercentage - equippedItem.manaRegenerationPercentage}%</size>");
                                }

                                // Reduced Mana Cost of skills
                                if (equippedItem.reducedManaCostOfSkillsAmount > helmetItem.reducedManaCostOfSkillsAmount)
                                {
                                    reducedManaCostString = reducedManaCostString.Replace(
                                        $"{helmetItem.reducedManaCostOfSkillsAmount}",
                                        $"<color=red>{helmetItem.reducedManaCostOfSkillsAmount}</color> <size=75%>-{equippedItem.reducedManaCostOfSkillsAmount - helmetItem.reducedManaCostOfSkillsAmount}</size>");
                                }

                                if (equippedItem.reducedManaCostOfSkillsAmount < helmetItem.reducedManaCostOfSkillsAmount)
                                {
                                    reducedManaCostString = reducedManaCostString.Replace(
                                        $"{helmetItem.reducedManaCostOfSkillsAmount}",
                                        $"<color=green>{helmetItem.reducedManaCostOfSkillsAmount}</color> <size=75%>+{helmetItem.reducedManaCostOfSkillsAmount - equippedItem.reducedManaCostOfSkillsAmount}</size>");
                                }
                            }

                            if (helmetItem.itemRarity == ItemRarity.Unique)
                            {
                                itemStats.SetText($"{physicalArmourString}\n" +
                                                  $"{elementalArmourString}\n" +
                                                  $"{healthString}\n" +
                                                  $"{manaString}\n" +
                                                  $"{strengthString}\n" +
                                                  $"{intelligenceString}\n" +
                                                  $"{specialAbility}\n" +
                                                  $"{manaRegenerationAmount}\n" +
                                                  $"{reducedManaCostString}");
                            }
                            else
                            {
                                itemStats.SetText($"{physicalArmourString}\n" +
                                                  $"{elementalArmourString}\n" +
                                                  $"{healthString}\n" +
                                                  $"{manaString}\n" +
                                                  $"{strengthString}\n" +
                                                  $"{intelligenceString}\n" +
                                                  $"{manaRegenerationAmount}\n" +
                                                  $"{reducedManaCostString}");   
                            }
                            break;
                        }
                        case ChestItem chestItem:
                        {
                            var healthOnHitAmountString = $"Health On Hit: {chestItem.healthOnHitAmount}";
                            var weaponRangeString       = $"Weapon Range: {chestItem.additionalWeaponRangeAmount}";
                        
                            // ## If we already have a chest equipped, compare. ##
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
                                        $"<color=red>{chestItem.healthOnHitAmount}</color> <size=75%>-{equippedItem.healthOnHitAmount - chestItem.healthOnHitAmount}</size>");
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
                                        $"<color=red>{chestItem.additionalWeaponRangeAmount}</color> <size=75%>-{equippedItem.additionalWeaponRangeAmount - chestItem.additionalWeaponRangeAmount}</size>");
                                }

                                if (equippedItem.additionalWeaponRangeAmount < chestItem.additionalWeaponRangeAmount)
                                {
                                    weaponRangeString = weaponRangeString.Replace(
                                        $"{chestItem.additionalWeaponRangeAmount}",
                                        $"<color=green>{chestItem.additionalWeaponRangeAmount}</color> <size=75%>+{chestItem.additionalWeaponRangeAmount - equippedItem.additionalWeaponRangeAmount}</size>");
                                }
                            }

                            if (chestItem.itemRarity == ItemRarity.Unique)
                            {
                                itemStats.SetText($"{physicalArmourString}\n" +
                                                  $"{elementalArmourString}\n" +
                                                  $"{healthString}\n" +
                                                  $"{manaString}\n" +
                                                  $"{strengthString}\n" +
                                                  $"{intelligenceString}\n" +
                                                  $"{specialAbility}\n" +
                                                  $"{healthOnHitAmountString}\n" +
                                                  $"{weaponRangeString}\n");
                            }
                            else
                            {
                                itemStats.SetText($"{physicalArmourString}\n" +
                                                  $"{elementalArmourString}\n" +
                                                  $"{healthString}\n" +
                                                  $"{manaString}\n" +
                                                  $"{strengthString}\n" +
                                                  $"{intelligenceString}\n" +
                                                  $"{healthOnHitAmountString}\n" +
                                                  $"{weaponRangeString}\n");   
                            }
                            break;
                        }
                        case BootItem bootItem:
                        {
                            var moveSpeedIncreaseString = $"Move Speed Increase: {bootItem.moveSpeedIncrease:F}%";

                            // ## If we already have boots equipped, compare. ##
                            if (playerEquipment.boots != null)
                            {
                                var equippedItem = playerEquipment.boots;

                                // Helmet physical armour
                                if (equippedItem.physicalArmour > bootItem.physicalArmour)
                                {
                                    physicalArmourString = physicalArmourString.Replace($"{bootItem.physicalArmour}",
                                        $"<color=red>{bootItem.physicalArmour}</color> <size=75%>-{equippedItem.physicalArmour - bootItem.physicalArmour}</size>");
                                }

                                if (equippedItem.physicalArmour < bootItem.physicalArmour)
                                {
                                    physicalArmourString = physicalArmourString.Replace($"{bootItem.physicalArmour}",
                                        $"<color=green>{bootItem.physicalArmour}</color> <size=75%>+{bootItem.physicalArmour - equippedItem.physicalArmour}</size>");
                                }

                                // Helmet elemental armour
                                if (equippedItem.elementalArmour > bootItem.elementalArmour)
                                {
                                    elementalArmourString = elementalArmourString.Replace($"{bootItem.elementalArmour}",
                                        $"<color=red>{bootItem.elementalArmour}</color> <size=75%>-{equippedItem.elementalArmour - bootItem.elementalArmour}</size>");
                                }

                                if (equippedItem.elementalArmour < bootItem.elementalArmour)
                                {
                                    elementalArmourString = elementalArmourString.Replace($"{bootItem.elementalArmour}",
                                        $"<color=green>{bootItem.elementalArmour}</color> <size=75%>+{bootItem.elementalArmour - equippedItem.elementalArmour}</size>");
                                }

                                // Helmet health
                                if (equippedItem.healthAmount > bootItem.healthAmount)
                                {
                                    healthString = healthString.Replace($"{bootItem.healthAmount}",
                                        $"<color=red>{bootItem.healthAmount}</color> <size=75%>{bootItem.healthAmount - equippedItem.healthAmount}</size>");
                                }

                                if (equippedItem.healthAmount < bootItem.healthAmount)
                                {
                                    healthString = healthString.Replace($"{bootItem.healthAmount}",
                                        $"<color=green>{bootItem.healthAmount}</color> <size=75%>+{bootItem.healthAmount - equippedItem.healthAmount}</size>");
                                }

                                // Helmet mana
                                if (equippedItem.manaAmount > bootItem.manaAmount)
                                {
                                    manaString = manaString.Replace($"{bootItem.manaAmount}",
                                        $"<color=red>{bootItem.manaAmount}</color> <size=75%>{bootItem.manaAmount - equippedItem.manaAmount}</size>");
                                }

                                if (equippedItem.manaAmount < bootItem.manaAmount)
                                {
                                    manaString = manaString.Replace($"{bootItem.manaAmount}",
                                        $"<color=green>{bootItem.manaAmount}</color> <size=75%>+{bootItem.manaAmount - equippedItem.manaAmount}</size>");
                                }

                                // Helmet strength
                                if (equippedItem.strengthAmount > bootItem.strengthAmount)
                                {
                                    strengthString = strengthString.Replace($"{bootItem.strengthAmount}",
                                        $"<color=red>{bootItem.strengthAmount}</color> <size=75%>{bootItem.strengthAmount - equippedItem.strengthAmount}</size>");
                                }

                                if (equippedItem.strengthAmount < bootItem.strengthAmount)
                                {
                                    strengthString = strengthString.Replace($"{bootItem.strengthAmount}",
                                        $"<color=green>{bootItem.strengthAmount}</color> <size=75%>+{bootItem.strengthAmount - equippedItem.strengthAmount}</size>");
                                }

                                // Helmet intelligence
                                if (equippedItem.intelligenceAmount > bootItem.intelligenceAmount)
                                {
                                    intelligenceString = intelligenceString.Replace($"{bootItem.intelligenceAmount}",
                                        $"<color=red>{bootItem.intelligenceAmount}</color> <size=75%>{bootItem.intelligenceAmount - equippedItem.intelligenceAmount}</size>");
                                }

                                if (equippedItem.intelligenceAmount < bootItem.intelligenceAmount)
                                {
                                    intelligenceString = intelligenceString.Replace($"{bootItem.intelligenceAmount}",
                                        $"<color=green>{bootItem.intelligenceAmount}</color> <size=75%>+{bootItem.intelligenceAmount - equippedItem.intelligenceAmount}</size>");
                                }

                                // Move Speed Increase
                                if (equippedItem.moveSpeedIncrease > bootItem.moveSpeedIncrease)
                                {
                                    moveSpeedIncreaseString = moveSpeedIncreaseString.Replace(
                                        $"{bootItem.moveSpeedIncrease}",
                                        $"<color=red>{bootItem.moveSpeedIncrease}</color> <size=75%>-{equippedItem.moveSpeedIncrease - bootItem.moveSpeedIncrease}</size");
                                }

                                if (equippedItem.moveSpeedIncrease < bootItem.moveSpeedIncrease)
                                {
                                    moveSpeedIncreaseString = moveSpeedIncreaseString.Replace(
                                        $"{bootItem.moveSpeedIncrease}",
                                        $"<color=green>{bootItem.moveSpeedIncrease}</color> <size=75%>+{bootItem.moveSpeedIncrease - equippedItem.moveSpeedIncrease}</size>");
                                }
                            }

                            if (bootItem.itemRarity == ItemRarity.Unique)
                            {
                                itemStats.SetText($"{physicalArmourString}\n" +
                                                  $"{elementalArmourString}\n" +
                                                  $"{healthString}\n" +
                                                  $"{manaString}\n" +
                                                  $"{strengthString}\n" +
                                                  $"{intelligenceString}\n" +
                                                  $"{specialAbility}\n" +
                                                  $"{moveSpeedIncreaseString}\n");
                            }
                            else
                            {
                                itemStats.SetText($"{physicalArmourString}\n" +
                                                  $"{elementalArmourString}\n" +
                                                  $"{healthString}\n" +
                                                  $"{manaString}\n" +
                                                  $"{strengthString}\n" +
                                                  $"{intelligenceString}\n" +
                                                  $"{moveSpeedIncreaseString}\n");
                            }
                            break;
                        }
                    }

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

        public static string GetItemDescription(Item item)
        {
            var description = item.itemDescription.Replace("HP", "<color=red>HP</color>");
            description = description.Replace("Mana", "<color=blue>Mana</color>");
            
            return description;
        }
    }

    [Serializable]
    public struct ItemRarityHoverImage
    {
        public string key;
        public Sprite image;
    }
}