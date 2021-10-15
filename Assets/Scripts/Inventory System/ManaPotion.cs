using UnityEngine;

namespace Inventory_System
{
    [CreateAssetMenu(fileName = "New Mana Potion", menuName = "Items/Create Mana Potion", order = 100)]
    public class ManaPotion : Item
    {
        [Header("Mana Potion Properties")]
        [Tooltip("The amount of mana restored when used")]
        public int restoreAmount;
    }
}