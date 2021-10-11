using Inventory_System;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class PlayerEquipmentManager : MonoBehaviour {
    
        [Header("User Interface")]
        [SerializeField] private Image weaponSlotImage;
        private                  Inventory            _playerInventory;
        private                  EquipmentInventory   _equipmentInventory;
        [SerializeField] private UIEquipmentInventory equipmentInventoryUI;
        public                   bool                 hasWeaponEquipped;
        public                   WeaponItem           weaponItem; // Store the item in-case we need it back
        public                   HelmetItem           head;
        public                   ChestItem            chest;
        public                   GameObject           boots;

        private void Start()
        {
            _equipmentInventory = new EquipmentInventory();
            _playerInventory = GetComponent<Player>().Inventory;
            equipmentInventoryUI.SetPlayer(GetComponent<Player>());
            equipmentInventoryUI.SetInventory(_equipmentInventory);
        }

        public void EquipWeapon(WeaponItem item)
        {
            weaponItem = item;
            hasWeaponEquipped = true;
            _equipmentInventory.AddItem(item);
        }

        public void EquipHelmet(HelmetItem helmetItem)
        {
            var player = GetComponent<Player>();
            head = helmetItem;
            GetComponent<Player>().strength += helmetItem.strengthAmount;
            GetComponent<Player>().intelligence += helmetItem.intelligenceAmount;
        
            // Update Player Stats
            var startMana = player.CurrentMana;
            if (helmetItem.strengthAmount < 0)
                player.maxHp -= (player.strengthHpIncreaseAmount * player.strength);
            else if (helmetItem.strengthAmount > 0)
                player.maxHp += (player.strengthHpIncreaseAmount * player.strength);
        
            if (helmetItem.intelligenceAmount < 0)
                player.maxMana -= (player.intelligenceManaIncreaseAmount * player.intelligence);
            else if (helmetItem.intelligenceAmount > 0)
                player.maxMana += (player.intelligenceManaIncreaseAmount * player.intelligence);

            player.manaRegenerationPercentage += helmetItem.manaRegenerationPercentage;
        
            _equipmentInventory.AddItem(helmetItem);
        }

        public void EquipChest(ChestItem chestItem)
        {
            var player = GetComponent<Player>();
            chest = chestItem;
            GetComponent<Player>().strength += chestItem.strengthAmount;
            GetComponent<Player>().intelligence += chestItem.intelligenceAmount;

            // Update Player Stats
            var startMana = player.CurrentMana;
            if (chestItem.strengthAmount < 0)
                player.maxHp -= (player.strengthHpIncreaseAmount * player.strength);
            else if (chestItem.strengthAmount > 0)
                player.maxHp += (player.strengthHpIncreaseAmount * player.strength);
        
            if (chestItem.intelligenceAmount < 0)
                player.maxMana -= (player.intelligenceManaIncreaseAmount * player.intelligence);
            else if (chestItem.intelligenceAmount > 0)
                player.maxMana += (player.intelligenceManaIncreaseAmount * player.intelligence);

            if (player.GetComponent<PlayerEquipmentManager>().weaponItem != null)
                player.GetComponent<PlayerEquipmentManager>().weaponItem.weaponRange += chestItem.additionalWeaponRangeAmount;
        
            _equipmentInventory.AddItem(chestItem);
        }

        public void UnEquip(Item item)
        {
            var player = GetComponent<Player>();
            equipmentInventoryUI.hoverInterface.SetActive(false);
            _equipmentInventory.RemoveItem(item);

            if (item is WeaponItem)
            {
                hasWeaponEquipped = false;
                weaponItem = null;
                _playerInventory.AddItem(item);
            }

            if (item is HelmetItem helmetItem)
            {
                head = null;
                player.strength -= helmetItem.strengthAmount;
                player.intelligence -= helmetItem.intelligenceAmount;
            
                // Update Player Stats
                var startMana = player.CurrentMana;
                if (helmetItem.strengthAmount < 0)
                    player.maxHp += (player.strengthHpIncreaseAmount * helmetItem.strengthAmount);
                else if (helmetItem.strengthAmount > 0)
                    player.maxHp -= (player.strengthHpIncreaseAmount * helmetItem.strengthAmount);
        
                if (helmetItem.intelligenceAmount < 0)
                    player.maxMana += (player.intelligenceManaIncreaseAmount * helmetItem.intelligenceAmount);
                else if (helmetItem.intelligenceAmount > 0)
                    player.maxMana -= (player.intelligenceManaIncreaseAmount * helmetItem.intelligenceAmount);
            
                _playerInventory.AddItem(item);
            }

            if (item is ChestItem chestItem)
            {
                this.chest = null;
                player.strength -= chestItem.strengthAmount;
                player.intelligence -= chestItem.intelligenceAmount;
            
                // Update Player Stats
                var startMana = player.CurrentMana;
                if (chestItem.strengthAmount < 0)
                    player.maxHp += (player.strengthHpIncreaseAmount * chestItem.strengthAmount);
                else if (chestItem.strengthAmount > 0)
                    player.maxHp -= (player.strengthHpIncreaseAmount * chestItem.strengthAmount);
        
                if (chestItem.intelligenceAmount < 0)
                    player.maxMana += (player.intelligenceManaIncreaseAmount * chestItem.intelligenceAmount);
                else if (chestItem.intelligenceAmount > 0)
                    player.maxMana -= (player.intelligenceManaIncreaseAmount * chestItem.intelligenceAmount);
            
                if (player.GetComponent<PlayerEquipmentManager>().weaponItem != null)
                    player.GetComponent<PlayerEquipmentManager>().weaponItem.weaponRange -= chestItem.additionalWeaponRangeAmount;    
                _playerInventory.AddItem(item);
            }
        }
    }
}