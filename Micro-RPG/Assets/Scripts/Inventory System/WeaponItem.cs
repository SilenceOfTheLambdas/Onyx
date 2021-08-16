using UnityEngine;

namespace Inventory_System
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Items/Create Weapon", order = 100)]
    public class WeaponItem : Item
    {
        [Header("Weapon Statistics")]
        
        [Tooltip("The amount of damage this weapon deals to an enemy when hit")]
        public int damage;
        
        [Tooltip("The maximum range this weapon can be used")]
        public int weaponRange;
    }
}