using System;
using System.Collections;
using System.Collections.Generic;
using Scriptable_Objects.Items.Scripts;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment Object", menuName = "Inventory System/Items/Equipment")]
public class EquipmentObject : ItemObject
{
    public enum defenseTypes
    {
        Necklace,
        Ring,
        NONE
    }
    [Header("Damage Properties")] 
    public int maxAttackDamage; // The maximum attack damage of the weapon
    public int minAttackDamage; // The minimum attack damage of the weapon
    public int attackRange; // The range at which the weapon can reach
    [Range(0f, 100f)]
    public float bonusDamageChance; // This controls the chance to which bonus damage to applied

    [Header("Defense Properties")] 
    public defenseTypes DefenseType; // What type of 'armour' is this item?
    public int attackDefense; // The total damage absorbed by the item (Physical)
    public int spellDefense; // The total damage absorbed by the item (Spell)
    public int healOverTimeAmount; // If applicable, how much will the player heal over time when not in combat
    public int healOverTime; // This will set how long (in seconds) will it take for healOverTimeAmount to be applied?
    
    private void Awake()
    {
        type = ItemTypes.Equipment;
    }
}
