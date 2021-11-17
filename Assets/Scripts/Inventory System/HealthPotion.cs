using UnityEngine;

namespace Inventory_System
{
    [CreateAssetMenu(fileName = "New Health Potion", menuName = "Items/Create Health Potion", order = 100)]
    public class HealthPotion : Item
    {
        [Header("Health Potion Properties")]
        
        [Tooltip("The amount of HP to restore to the player")]
        public int restoreAmount;

        public override void RandomlyGenerateItem()
        {
            
        }
    }
}