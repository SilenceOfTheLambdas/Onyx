using UnityEngine;

namespace Inventory_System
{
    [CreateAssetMenu(fileName = "New Chest Armour", menuName = "Items/Armour/Create Chest Armour")]
    public class ChestItem : ArmourItem
    {
        [Header("Chest Specific Stats")]
        
        [Tooltip("The percentage of health (HP) gained when the player lands a successful hit on an enemy")]
        public int healthOnHitAmount = 0;

        [Range(0, 5)] [Tooltip("Adds some additional range to the player's weapon")]
        public int additionalWeaponRangeAmount = 0;

        [Space] [Header("Random Generation")]
        
        public int minHealthOnHitAmount;
        public int maxHealthOnHitAmount;

        [Range(0, 5)]
        public int minAdditionalWeaponRangeAmount;

        [Range(0, 5)]
        public int maxAdditionalWeaponRangeAmount;

        public override void RandomlyGenerateItem()
        {
            base.RandomlyGenerateItem();
            healthOnHitAmount = Random.Range(minHealthOnHitAmount, maxHealthOnHitAmount);
            additionalWeaponRangeAmount = Random.Range(minAdditionalWeaponRangeAmount, maxAdditionalWeaponRangeAmount);
        }
    }
}