using UnityEngine;

namespace Inventory_System
{
    [CreateAssetMenu(fileName = "New Helmet", menuName = "Items/Create Helmet", order = 100)]
    public class HelmetItem : Item
    {
        [Header("Helmet Statistics")] 
        
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

    }
}