using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Skills
{
    [CreateAssetMenu(fileName = "Create Skill", menuName = "Skills/Create New Skill")] [Serializable]
    public class Skill : ScriptableObject
    {
        [Header("Skill Information")]
        
        [Tooltip("The name of this skill")]
        public string skillName;
        
        [Tooltip("The description of this skill")]
        public string description;

        [Tooltip("The skills sprite (used for the Hotbar and Skill tree)")]
        public Sprite skillSprite;
        [Tooltip("The prefab object that spawn when this skill is activated")]
        public GameObject skillEffect;
        
        [Tooltip("The Type of skill this is")]
        public SkillType  skillType;
        
        [Tooltip("The amount of damage this skill does to a target")]
        public int amountOfDamage;

        [Tooltip("If applicable; the total time this skill lasts for")]
        public int skillUseTime;

        [Tooltip("Damage over Time multiplier (will damage every n seconds)")] [Range(0.1f, 2f)]
        public float dotTimeMultiplier;
        
        [Tooltip("The amount of seconds it takes for this skill to be used again")]
        public int coolDownTime;
        
        [Tooltip("If 1: this skill can be used once before going into cooldown, if more than 1, than the amount given can be used before cooldown is activated.")]
        public int stackSize;

        [Header("Requirements")] 
        
        [Tooltip("The required ACTIVE skills needed before this one can be unlocked")]
        public List<Skill> requiredActiveSkills;

        [Tooltip("The amount of mana this skill costs to use")]
        public int manaCost;
        
        [Tooltip("The amount of intelligence required to use this skill (unlock it)")]
        public int requiredIntelligenceLevel;
        
        [Tooltip("The required level the player must be to use/unlock this skill")]
        public int requiredNumberOfSkillPointsToUnlockOrUpgrade;
        
        [Tooltip("The maximum level of this skill")]
        public int maximumSkillLevel;
        
        [Tooltip("The current level of this skill")]
        public int skillLevel;
        
        /// <summary>
        /// The image used to represent the cooldown timer.
        /// </summary>
        [NonSerialized] public Image CoolDownImage;

        /// <summary>
        /// The text that displays the timer;
        /// </summary>
        [NonSerialized] public TextMeshProUGUI CoolDownText;

        [NonSerialized] public bool  HasBeenUsed;
        [NonSerialized] public bool  InCoolDown;
        [NonSerialized] public float SkillTimer;
    }

    [System.Serializable]
    public enum SkillType
    {
        Projectile,
        Beam,
        Buff
    }
}