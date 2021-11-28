using System;
using System.Collections.Generic;
using Player;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Inventory_System
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Items/Create Weapon")] [Serializable]
    public class WeaponItem : Item
    {
        [Header("Requirements")]
        
        [Tooltip("The level the player MUST be to equip this item")]
        public int levelRequirement;
        
        [Tooltip("How much strength is required to use this item")]
        public int strengthRequirement;
        
        [Tooltip("How much intelligence is required to use this item")]
        public int intelligenceRequirement;
        
        [Header("Weapon Statistics")]
        
        [Tooltip("The amount of damage this weapon deals to an enemy when hit")]
        public int damage;
        
        [Tooltip("The maximum range this weapon can be used")] [Range(2f, 10f)]
        public float weaponRange;

        [Tooltip("Attack speed of the weapon (1 = Full animation)")][Range(0.1f, 2f)]
        public float attackRate = 1f;

        [Header("Special Ability")] 
        public ItemAbility specialAbility;
        
        [Space]
        [Header("Weapon Prefab")] [Tooltip("The prefab of the weapon that spawns when this weapon is equipped")]
        public GameObject equippedWeaponPrefab;

        [Header("### Random Weapon Generation ###")]
        
        [Header("Level Requirement")]
        [Tooltip("The lower limit of this items level requirement, this will taken away from the players current level.")]
        public int lowerLevelRequirement;
        [Tooltip("The upper limit of this items level requirement, this will added on top of the players current level.")]
        public int upperLevelRequirement;
        
        [Range(-30, 30)] [Header("Strength Requirement")]
        public int minStrengthRequirement;
        [Range(-30, 30)]
        public int maxStrengthRequirement;

        [Range(-30, 30)] [Header("Intellect Requirement")]
        public int minIntelligenceRequirement;
        [Range(-30, 30)]
        public int maxIntelligenceRequirement;
        
        [Header("Weapon Stats")]
        
        [Range(0, 300)]
        public int minPhysicalDamage;
        [Range(0, 300)]
        public int maxPhysicalDamage;

        [Range(-2, 4)]
        public float minWeaponRange;
        [Range(-2, 4)]
        public float maxWeaponRange;

        [Range(0.1f, 2f)]
        public float minAttackRate;
        [Range(0.1f, 2f)]
        public float maxAttackRate;
        
        [Header("Special Ability Pool")]
        public List<ItemAbility> itemAbilityPool;

        public override void RandomlyGenerateItem()
        {
            var playerAbilitiesSystem = GameManager.Instance.player.GetComponent<AbilitiesSystem>();
            levelRequirement = Random.Range(playerAbilitiesSystem.curLevel - lowerLevelRequirement, playerAbilitiesSystem.curLevel + upperLevelRequirement);
            strengthRequirement = Random.Range(minStrengthRequirement, maxStrengthRequirement);
            intelligenceRequirement = Random.Range(minIntelligenceRequirement, maxIntelligenceRequirement);
            damage = Random.Range(minPhysicalDamage, maxPhysicalDamage);
            weaponRange = Random.Range(minWeaponRange, maxWeaponRange);
            attackRate = Random.Range(minAttackRate, maxAttackRate);
            specialAbility = ArmourItem.GetRandom(itemAbilityPool);
        }
    }
}