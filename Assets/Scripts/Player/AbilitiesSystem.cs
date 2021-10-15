using UnityEditor;
using UnityEngine;
using Inventory_System;
using UI;

namespace Player
{
    [RequireComponent(typeof(PlayerEquipmentManager))]
    public class AbilitiesSystem : MonoBehaviour
    {
        #region Properties
        public int CurrentMana
        {
            get => _currentMana;
            set
            {
                _currentMana = (int)Mathf.Clamp(value, 0f, maxMana);
            }
        }
        #endregion

        #region Fields

        [Tooltip("The maximum amount of mana the player has, this is affected by Intelligence")]
        public int maxMana = 30;

        [Range(0, 20)]
        [Tooltip("The percentage of mana restored every second")]
        public int manaRegenerationPercentage = 1;
        
        [SerializeField] private float manaRegenerationTime;

        [Space]
        [Header("Player Attributes")]
        [Tooltip("Strength: for each level of strength: +strengthHpIncreaseAmount and +strengthPhysicalDamageIncreaseAmount to physical damage")]
        public int strength = 0;

        [Tooltip("The amount of HP to give the player per Strength level")]
        public int strengthHpIncreaseAmount = 3;

        [Tooltip("The amount of Physical Damage to add towards attacks per Strength level")]
        public int strengthPhysicalDamageIncreaseAmount = 5;

        [Space]
        [Tooltip("Intelligence: for each level: +intelligenceManaIncreaseAmount Mana and +intelligenceElementalDamageIncreaseAmount to elemental damage")]
        public int intelligence = 0;

        [Tooltip("The amount of Mana to give the player per Intelligence level")]
        public int intelligenceManaIncreaseAmount = 10;

        [Tooltip("The amount of Elemental Damage to add towards Skill attacks per Intelligence level")]
        public int intelligenceElementalDamageIncreaseAmount = 8;

        [Space] [Header("Experience")]
        public int skillPoints = 1;
        public int   curLevel = 1; // our current level
        public int   curXp = 0; // our current experience points
        public int   xpToNextLevel; // xp needed to level up
        public float levelXpModifier; // modifier applied to 'xpToNextLevel' when we level up

        private float _manaRegenTimer;
        private int _currentMana;
        private PlayerEquipmentManager _playerEquipmentManager;

        [Header("References")]
        public PlayerUi playerUi;
        
        #region Events
        public delegate void ValueChanged();
        public delegate void LevelChanged();
        /*public static event ValueChanged OnManaChanged;*/
        public static event LevelChanged OnLevelUp;
        #endregion


        #endregion

        private void Awake()
        {
            _currentMana = maxMana;
            CurrentMana = maxMana;
            _playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
        }

        private void Update()
        {
            ManaRegeneration();
        }

        // called when we gain xp
        public void AddXp(int xp)
        {
            curXp += xp;

            if (curXp >= xpToNextLevel)
            {
                LevelUp(xp);
            }
        }

        // called when our xp reaches the max for this level
        public void LevelUp(int xp)
        {
            // Fire off event
            OnLevelUp();

            curLevel++;
            skillPoints += 1;
            curXp = 0;
            if (xp > xpToNextLevel)
                curXp += xp - xpToNextLevel;
            if (xp < xpToNextLevel)
                curXp += xpToNextLevel - xp;
            xpToNextLevel = (int)(xpToNextLevel * levelXpModifier);
            playerUi.UpdateLevelText();
        }

        public void RemoveMana(int amountOfManaToTake)
        {
            CurrentMana -= amountOfManaToTake;
        }

        public void IncreaseMana(int amount)
        {
            if (CurrentMana + amount >= maxMana)
                CurrentMana = maxMana;
            else
                CurrentMana += amount;
        }

        private void ManaRegeneration()
        {
            _manaRegenTimer += Time.deltaTime;
            if (_manaRegenTimer >= manaRegenerationTime)
            {
                _manaRegenTimer = 0;
                IncreaseMana((manaRegenerationPercentage / 100) * maxMana);
            }
        }

        public void AttempToUseManaPotion(Inventory_System.Inventory inventory, ManaPotion manaPotion)
        {
            if (CurrentMana < maxMana)
            {
                IncreaseMana(manaPotion.restoreAmount);
                inventory.RemoveItem(manaPotion);
            }
        }

        /// <summary>
        /// Calculates the total amount of physical damage the player can deal.
        /// </summary>
        /// <returns>The amount of damage to inflict on a target</returns>
        public int CalculatePhysicalDamage()
        {
            var dmg = _playerEquipmentManager.weaponItem.damage; // the weapons damage is used as a base
            dmg += (strengthPhysicalDamageIncreaseAmount * strength);
            return dmg;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(AbilitiesSystem))]
    internal class AbilitiesSystemEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var abilitiesSystem = (AbilitiesSystem)target;
            if (abilitiesSystem == null) return;


            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Details");

            base.DrawDefaultInspector();

            // Increase player level
            Undo.RecordObject(abilitiesSystem, "Increment Level");
            if (GUILayout.Button("Increment Level"))
            {
                abilitiesSystem.LevelUp(abilitiesSystem.xpToNextLevel);
            }

            // Increase player strength
            Undo.RecordObject(abilitiesSystem, "Increase Strength");
            if (GUILayout.Button("Increment Strength"))
            {
                abilitiesSystem.strength += 1;
            }

            // Increase player intelligence
            Undo.RecordObject(abilitiesSystem, "Increase Intellegence");
            if (GUILayout.Button("Increment Int"))
            {
                abilitiesSystem.intelligence += 1;
            }

            #region Ability Information
            EditorGUILayout.LabelField("Level");
            EditorGUILayout.IntField(abilitiesSystem.curLevel);
            #endregion
        }
    }
#endif
}