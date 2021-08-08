using System;
using UnityEngine;

namespace Skills
{
    [CreateAssetMenu(menuName = "Skills/Create Skill")]
    public class Skill : ScriptableObject
    {
        /// <summary>
        /// The description of this skill.
        /// </summary>
        public string description;

        public GameObject skillEffect;
        public SkillType  skillType;
        /// <summary>
        /// The amount of damage this skill does.
        /// </summary>
        public int amountOfDamage;
        /// <summary>
        /// The cooldown time for this skill.
        /// </summary>
        public int coolDownTime;
        /// <summary>
        /// If 1: this skill can be used once before going into cooldown, if more than 1, than the amount given can be used
        /// before cooldown is activated.
        /// </summary>
        public int stackSize;

        [NonSerialized] public bool  HasBeenUsed;
        [NonSerialized] public bool  InCoolDown;
        [NonSerialized] public float SkillTimer;
    }

    public enum SkillType
    {
        Projectile,
        Swing,
        Buff
    }
}