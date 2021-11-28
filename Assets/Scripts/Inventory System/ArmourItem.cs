using System.Collections.Generic;
using System.Linq;
using Player;
using UnityEditor;
using UnityEngine;

namespace Inventory_System
{
    public class ArmourItem : Item
    {
        [Header("Requirements")] [Tooltip("The level the player MUST be to equip this item")]
        public int levelRequirement;
        
        [Tooltip("How much strength is required to use this item")]
        public int strengthRequirement;
        
        [Tooltip("How much intelligence is required to use this item")]
        public int intelligenceRequirement;

        [Header("Base Stats")]
        
        [Range(-30, 30)] [Tooltip("The amount of physical damage; any positive value will mitigate damage by that amount, and any negative value will add damage of that amount when attacked")] 
        public int physicalArmour;
        
        [Range(-30, 30)] [Tooltip("The amount of elemental damage; any positive value will mitigate damage by that amount, and any negative value will add damage of that amount when attacked")]
        public int elementalArmour;
        
        [Range(-50, 50)] [Tooltip("The amount of health this item gives or takes away from the player")]
        public int healthAmount = 0;
        
        [Range(-50, 50)] [Tooltip("The amount of mana this item gives of takes away from the player")]
        public int manaAmount = 0;

        [Range(-20, 20)] [Tooltip("The amount of Strength this item gives of takes away from the player")]
        public int strengthAmount = 0;

        [Range(-20, 20)] [Tooltip("The amount of Intelligence this item gives of takes away from the player")]
        public int intelligenceAmount = 0;

        [Header("Special Ability")] 
        public ItemAbility specialAbility;

        [Space] [Header("Random Range Requirements")] [Space] 
        
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

        [Space] [Header("Random Range Base Stats")] [Space]
        
        [Range(-30, 30)] [Header("Physical Armour")]
        public int minPhysicalArmour;
        [Range(-30, 30)]
        public int maxPhysicalArmour;

        [Range(-30, 30)] [Header("Elemental Increase/Decrease")]
        public int minElementalArmour;
        [Range(-30, 30)]
        public int maxElementalArmour;

        [Range(-30, 30)] [Header("Health Increase/Decrease")]
        public int minHealthAmount;
        [Range(-30, 30)]
        public int maxHealthAmount;

        [Range(-30, 30)] [Header("Mana Increase/Decrease")]
        public int minManaAmount;
        [Range(-30, 30)]
        public int maxManaAmount;

        [Range(-30, 30)] [Header("Strength Stat Increase/Decrease")]
        public int minStrengthAmount;
        [Range(-30, 30)]
        public int maxStrengthAmount;

        [Range(-30, 30)] [Header("Intellect Stat Increase/Decrease")]
        public int minIntelligenceAmount;
        [Range(-30, 30)]
        public int maxIntelligenceAmount;

        [Header("Special Ability Pool")]
        public List<ItemAbility> itemAbilityPool;

        public override void RandomlyGenerateItem()
        {
            var playerAbilitiesSystem = GameManager.Instance.player.GetComponent<AbilitiesSystem>();
            levelRequirement = Random.Range(playerAbilitiesSystem.curLevel - lowerLevelRequirement, playerAbilitiesSystem.curLevel + upperLevelRequirement);
            strengthRequirement = Random.Range(minStrengthRequirement, maxStrengthRequirement);
            intelligenceRequirement = Random.Range(minIntelligenceRequirement, maxIntelligenceRequirement);
            physicalArmour = Random.Range(minPhysicalArmour, maxPhysicalArmour);
            elementalArmour = Random.Range(minElementalArmour, maxElementalArmour);
            healthAmount = Random.Range(minHealthAmount, maxHealthAmount);
            manaAmount = Random.Range(minManaAmount, maxManaAmount);
            strengthAmount = Random.Range(minStrengthAmount, maxStrengthAmount);
            intelligenceAmount = Random.Range(minIntelligenceAmount, maxIntelligenceAmount);
            specialAbility = GetRandom(itemAbilityPool);
        }

        /// <summary>
        /// Gets a random item from a given pool, factoring in the probability of each item.
        /// Original source from: https://stackoverflow.com/a/38086831/12878373
        /// </summary>
        /// <param name="pool">A pool of ItemAbility to get a random ItemAbility from</param>
        /// <returns>A random ItemAbility</returns>
        public static ItemAbility GetRandom(IEnumerable<ItemAbility> pool)
        {
            // Get Universal probability
            var itemAbilities = pool.ToList();

            var u = itemAbilities.Sum(p => p.chanceToSpawn);
            
            // Pick a random number between 0 and u
            var r = Random.Range(0, u);

            float sum = 0;

            foreach (var ability in itemAbilities)
            {
                // loop until the random number is less than our cumulative probability
                if (r <= (sum += ability.chanceToSpawn))
                    return ability;
            }
            // Should never get here
            var none = new ItemAbility
            {
                ability = ItemAbility.Abilities.None
            };
            return none;
        }
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(ArmourItem))]
    internal class ArmourItemEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var          armourItem         = (ArmourItem)target;
            List<string> excludedProperties = new List<string>();
            if (armourItem == null) return;
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Armour Item Details");

            if (!armourItem.randomlyGenerateStats)
            {
                excludedProperties.Clear();
                excludedProperties.Add("minStrengthRequirement");
                excludedProperties.Add("maxStrengthRequirement");
                
                excludedProperties.Add("minIntelligenceRequirement");
                excludedProperties.Add("maxIntelligenceRequirement");
                
                excludedProperties.Add("minPhysicalArmour");
                excludedProperties.Add("maxPhysicalArmour");
                
                excludedProperties.Add("minElementalArmour");
                excludedProperties.Add("maxElementalArmour");
                
                excludedProperties.Add("minHealthAmount");
                excludedProperties.Add("maxHealthAmount");
                
                excludedProperties.Add("minManaAmount");
                excludedProperties.Add("maxManaAmount");
                
                excludedProperties.Add("minStrengthAmount");
                excludedProperties.Add("maxStrengthAmount");
                
                excludedProperties.Add("minIntelligenceAmount");
                excludedProperties.Add("maxIntelligenceAmount");
            }

            if (armourItem.randomlyGenerateStats)
            {
                excludedProperties.Clear();
                excludedProperties.Add("strengthRequirement");
                excludedProperties.Add("intelligenceRequirement");
                excludedProperties.Add("physicalArmour");
                excludedProperties.Add("elementalArmour");
                excludedProperties.Add("healthAmount");
                excludedProperties.Add("manaAmount");
                excludedProperties.Add("strengthAmount");
                excludedProperties.Add("intelligenceAmount");
            }
            
            DrawPropertiesExcluding(serializedObject, excludedProperties.ToArray());
            
            armourItem.randomlyGenerateStats = GUILayout.Toggle(armourItem.randomlyGenerateStats, "Random Generation");
            
        }
    }
#endif
}