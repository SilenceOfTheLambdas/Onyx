using Inventory_System;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class PlayerEquipmentManager : MonoBehaviour {
    
        [Header("User Interface")]
        [SerializeField] private Image                weaponSlotImage;
        [SerializeField] private UIEquipmentInventory equipmentInventoryUI;
        private                  Inventory            _playerInventory;
        private                  EquipmentInventory   _equipmentInventory;
        public                   bool                 hasWeaponEquipped;
        public                   WeaponItem           weaponItem;
        public                   HelmetItem           head;
        public                   ChestItem            chest;
        public                   GameObject           boots;

        [SerializeField] private Transform playerWeaponHolsterTransform;

        private                 Player          _player;
        private                 AbilitiesSystem _playerAbilitySystem;
        private static readonly int             AttackSpeedMultiplier = Animator.StringToHash("attackSpeedMultiplier");

        private void Start()
        {
            _player = GetComponent<Player>();
            _playerAbilitySystem = GetComponent<AbilitiesSystem>();
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
            GetComponent<Animator>().SetFloat(AttackSpeedMultiplier, weaponItem.attackRate);

            // Spawn the player weapon item prefab
            Instantiate(weaponItem.equippedWeaponPrefab, playerWeaponHolsterTransform);
        }

        public void EquipHelmet(HelmetItem helmetItem)
        {
            head = helmetItem;
            _playerAbilitySystem.strength += helmetItem.strengthAmount;
            _playerAbilitySystem.intelligence += helmetItem.intelligenceAmount;
        
            // Update Player Stats
            var startMana = _playerAbilitySystem.CurrentMana;
            if (helmetItem.strengthAmount < 0)
                _player.maxHp -= (_playerAbilitySystem.strengthHpIncreaseAmount * _playerAbilitySystem.strength);
            else if (helmetItem.strengthAmount > 0)
                _player.maxHp += (_playerAbilitySystem.strengthHpIncreaseAmount * _playerAbilitySystem.strength);
        
            if (helmetItem.intelligenceAmount < 0)
                _playerAbilitySystem.maxMana -= (_playerAbilitySystem.intelligenceManaIncreaseAmount * _playerAbilitySystem.intelligence);
            else if (helmetItem.intelligenceAmount > 0)
                _playerAbilitySystem.maxMana += (_playerAbilitySystem.intelligenceManaIncreaseAmount * _playerAbilitySystem.intelligence);

            _playerAbilitySystem.manaRegenerationPercentage += helmetItem.manaRegenerationPercentage;
        
            _equipmentInventory.AddItem(helmetItem);
        }

        public void EquipChest(ChestItem chestItem)
        {
            chest = chestItem;
            _playerAbilitySystem.strength += chestItem.strengthAmount;
            _playerAbilitySystem.intelligence += chestItem.intelligenceAmount;

            // Update Player Stats
            var startMana = _playerAbilitySystem.CurrentMana;
            if (chestItem.strengthAmount < 0)
                _player.maxHp -= (_playerAbilitySystem.strengthHpIncreaseAmount * _playerAbilitySystem.strength);
            else if (chestItem.strengthAmount > 0)
                _player.maxHp += (_playerAbilitySystem.strengthHpIncreaseAmount * _playerAbilitySystem.strength);
        
            if (chestItem.intelligenceAmount < 0)
                _playerAbilitySystem.maxMana -= (_playerAbilitySystem.intelligenceManaIncreaseAmount * _playerAbilitySystem.intelligence);
            else if (chestItem.intelligenceAmount > 0)
                _playerAbilitySystem.maxMana += (_playerAbilitySystem.intelligenceManaIncreaseAmount * _playerAbilitySystem.intelligence);

            if (_playerAbilitySystem.GetComponent<PlayerEquipmentManager>().weaponItem != null)
                _playerAbilitySystem.GetComponent<PlayerEquipmentManager>().weaponItem.weaponRange += chestItem.additionalWeaponRangeAmount;
        
            _equipmentInventory.AddItem(chestItem);
        }

        public void UnEquip(Item item)
        {
            equipmentInventoryUI.hoverInterface.SetActive(false);
            _equipmentInventory.RemoveItem(item);

            if (item is WeaponItem)
            {
                hasWeaponEquipped = false;
                weaponItem = null;
                _playerInventory.AddItem(item);

                // Remove weapon from players' hands
                Destroy(playerWeaponHolsterTransform.gameObject.GetComponentInChildren<EnemyHitDetection>().gameObject);
            }

            if (item is HelmetItem helmetItem)
            {
                head = null;
                _playerAbilitySystem.strength -= helmetItem.strengthAmount;
                _playerAbilitySystem.intelligence -= helmetItem.intelligenceAmount;
            
                // Update Player Stats
                var startMana = _playerAbilitySystem.CurrentMana;
                if (helmetItem.strengthAmount < 0)
                    _player.maxHp += (_playerAbilitySystem.strengthHpIncreaseAmount * helmetItem.strengthAmount);
                else if (helmetItem.strengthAmount > 0)
                    _player.maxHp -= (_playerAbilitySystem.strengthHpIncreaseAmount * helmetItem.strengthAmount);
        
                if (helmetItem.intelligenceAmount < 0)
                    _playerAbilitySystem.maxMana += (_playerAbilitySystem.intelligenceManaIncreaseAmount * helmetItem.intelligenceAmount);
                else if (helmetItem.intelligenceAmount > 0)
                    _playerAbilitySystem.maxMana -= (_playerAbilitySystem.intelligenceManaIncreaseAmount * helmetItem.intelligenceAmount);
            
                _playerInventory.AddItem(item);
            }

            if (item is ChestItem chestItem)
            {
                this.chest = null;
                _playerAbilitySystem.strength -= chestItem.strengthAmount;
                _playerAbilitySystem.intelligence -= chestItem.intelligenceAmount;
            
                // Update Player Stats
                var startMana = _playerAbilitySystem.CurrentMana;
                if (chestItem.strengthAmount < 0)
                    _player.maxHp += (_playerAbilitySystem.strengthHpIncreaseAmount * chestItem.strengthAmount);
                else if (chestItem.strengthAmount > 0)
                    _player.maxHp -= (_playerAbilitySystem.strengthHpIncreaseAmount * chestItem.strengthAmount);
        
                if (chestItem.intelligenceAmount < 0)
                    _playerAbilitySystem.maxMana += (_playerAbilitySystem.intelligenceManaIncreaseAmount * chestItem.intelligenceAmount);
                else if (chestItem.intelligenceAmount > 0)
                    _playerAbilitySystem.maxMana -= (_playerAbilitySystem.intelligenceManaIncreaseAmount * chestItem.intelligenceAmount);
            
                if (_playerAbilitySystem.GetComponent<PlayerEquipmentManager>().weaponItem != null)
                    _playerAbilitySystem.GetComponent<PlayerEquipmentManager>().weaponItem.weaponRange -= chestItem.additionalWeaponRangeAmount;    
                _playerInventory.AddItem(item);
            }
        }
    }
}