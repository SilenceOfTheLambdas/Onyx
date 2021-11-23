using Inventory_System;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class PlayerEquipmentManager : MonoBehaviour {
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

            _playerAbilitySystem.strengthPhysicalDamageIncreaseAmount += helmetItem.physicalArmour;
            _playerAbilitySystem.intelligenceElementalDamageIncreaseAmount += helmetItem.elementalArmour;
            _playerAbilitySystem.manaRegenerationPercentage += helmetItem.manaRegenerationPercentage;
            
            switch (helmetItem.strengthAmount)
            {
                // Update Player Stats
                case < 0:
                    _player.maxHp -= (_playerAbilitySystem.strengthHpIncreaseAmount * _playerAbilitySystem.strength);
                    break;
                case > 0:
                    _player.maxHp += (_playerAbilitySystem.strengthHpIncreaseAmount * _playerAbilitySystem.strength);
                    break;
            }
        
            switch (helmetItem.intelligenceAmount)
            {
                case < 0:
                    _playerAbilitySystem.maxMana -= _playerAbilitySystem.intelligenceManaIncreaseAmount * _playerAbilitySystem.intelligence;
                    break;
                case > 0:
                    _playerAbilitySystem.maxMana += _playerAbilitySystem.intelligenceManaIncreaseAmount * _playerAbilitySystem.intelligence;
                    break;
            }

            switch (helmetItem.healthAmount)
            {
                case > 0:
                    _player.maxHp += helmetItem.healthAmount;
                    break;
                case < 0:
                    _player.maxHp -= helmetItem.healthAmount;
                    break;
            }

            switch (helmetItem.manaAmount)
            {
                case > 0:
                    _playerAbilitySystem.maxMana += helmetItem.manaAmount;
                    break;
                case < 0:
                    _playerAbilitySystem.maxMana -= helmetItem.manaAmount;
                    break;
            }
        
            _equipmentInventory.AddItem(helmetItem);
        }

        public void EquipChest(ChestItem chestItem)
        {
            chest = chestItem;
            _playerAbilitySystem.strength += chestItem.strengthAmount;
            _playerAbilitySystem.intelligence += chestItem.intelligenceAmount;

            _playerAbilitySystem.strengthPhysicalDamageIncreaseAmount += chestItem.physicalArmour;
            _playerAbilitySystem.intelligenceElementalDamageIncreaseAmount += chestItem.elementalArmour;

            switch (chestItem.strengthAmount)
            {
                // Update Player Stats
                case < 0:
                    _player.maxHp -= (_playerAbilitySystem.strengthHpIncreaseAmount * _playerAbilitySystem.strength);
                    break;
                case > 0:
                    _player.maxHp += (_playerAbilitySystem.strengthHpIncreaseAmount * _playerAbilitySystem.strength);
                    break;
            }
        
            switch (chestItem.intelligenceAmount)
            {
                case < 0:
                    _playerAbilitySystem.maxMana -= _playerAbilitySystem.intelligenceManaIncreaseAmount * _playerAbilitySystem.intelligence;
                    break;
                case > 0:
                    _playerAbilitySystem.maxMana += _playerAbilitySystem.intelligenceManaIncreaseAmount * _playerAbilitySystem.intelligence;
                    break;
            }

            switch (chestItem.healthAmount)
            {
                case > 0:
                    _player.maxHp += chestItem.healthAmount;
                    break;
                case < 0:
                    _player.maxHp -= chestItem.healthAmount;
                    break;
            }

            switch (chestItem.manaAmount)
            {
                case > 0:
                    _playerAbilitySystem.maxMana += chestItem.manaAmount;
                    break;
                case < 0:
                    _playerAbilitySystem.maxMana -= chestItem.manaAmount;
                    break;
            }
            
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

                switch (helmetItem.strengthAmount)
                {
                    // Update Player Stats
                    case < 0:
                        _player.maxHp += (_playerAbilitySystem.strengthHpIncreaseAmount * _playerAbilitySystem.strength);
                        break;
                    case > 0:
                        _player.maxHp -= (_playerAbilitySystem.strengthHpIncreaseAmount * _playerAbilitySystem.strength);
                        break;
                }
                
                switch (helmetItem.intelligenceAmount)
                {
                    case < 0:
                        _playerAbilitySystem.maxMana += _playerAbilitySystem.intelligenceManaIncreaseAmount * _playerAbilitySystem.intelligence;
                        break;
                    case > 0:
                        _playerAbilitySystem.maxMana -= _playerAbilitySystem.intelligenceManaIncreaseAmount * _playerAbilitySystem.intelligence;
                        break;
                }


                switch (helmetItem.healthAmount)
                {
                    case > 0:
                        _player.maxHp -= helmetItem.healthAmount;
                        break;
                    case < 0:
                        _player.maxHp += helmetItem.healthAmount;
                        break;
                }

                switch (helmetItem.manaAmount)
                {
                    case > 0:
                        _playerAbilitySystem.maxMana -= helmetItem.manaAmount;
                        break;
                    case < 0:
                        _playerAbilitySystem.maxMana += helmetItem.manaAmount;
                        break;
                }
            
                _playerAbilitySystem.manaRegenerationPercentage -= helmetItem.manaRegenerationPercentage;
                _playerAbilitySystem.strength -= helmetItem.strengthAmount;
                _playerAbilitySystem.intelligence -= helmetItem.intelligenceAmount;

                _playerAbilitySystem.strengthPhysicalDamageIncreaseAmount -= helmetItem.physicalArmour;
                _playerAbilitySystem.intelligenceElementalDamageIncreaseAmount -= helmetItem.elementalArmour;

                _playerInventory.AddItem(item);
            }

            if (item is ChestItem chestItem)
            {
                chest = null;
                
                _playerAbilitySystem.strengthPhysicalDamageIncreaseAmount -= chestItem.physicalArmour;
                _playerAbilitySystem.intelligenceElementalDamageIncreaseAmount -= chestItem.elementalArmour;

                switch (chestItem.strengthAmount)
                {
                    // Update Player Stats
                    case < 0:
                        _player.maxHp += (_playerAbilitySystem.strengthHpIncreaseAmount * _playerAbilitySystem.strength);
                        break;
                    case > 0:
                        _player.maxHp -= (_playerAbilitySystem.strengthHpIncreaseAmount * _playerAbilitySystem.strength);
                        break;
                }
        
                switch (chestItem.intelligenceAmount)
                {
                    case < 0:
                        _playerAbilitySystem.maxMana += _playerAbilitySystem.intelligenceManaIncreaseAmount * _playerAbilitySystem.intelligence;
                        break;
                    case > 0:
                        _playerAbilitySystem.maxMana -= _playerAbilitySystem.intelligenceManaIncreaseAmount * _playerAbilitySystem.intelligence;
                        break;
                }

                switch (chestItem.healthAmount)
                {
                    case > 0:
                        _player.maxHp -= chestItem.healthAmount;
                        break;
                    case < 0:
                        _player.maxHp += chestItem.healthAmount;
                        break;
                }

                switch (chestItem.manaAmount)
                {
                    case > 0:
                        _playerAbilitySystem.maxMana -= chestItem.manaAmount;
                        break;
                    case < 0:
                        _playerAbilitySystem.maxMana += chestItem.manaAmount;
                        break;
                }
                
                _playerAbilitySystem.strength -= chestItem.strengthAmount;
                _playerAbilitySystem.intelligence -= chestItem.intelligenceAmount;

                if (_playerAbilitySystem.GetComponent<PlayerEquipmentManager>().weaponItem != null)
                    _playerAbilitySystem.GetComponent<PlayerEquipmentManager>().weaponItem.weaponRange -= chestItem.additionalWeaponRangeAmount;
                
                _playerInventory.AddItem(item);
            }
            // Double check to make sure player's HP is not over the maximum
            if (_player.CurrentHp > _player.maxHp)
                _player.CurrentHp = _player.maxHp;
        }
    }
}