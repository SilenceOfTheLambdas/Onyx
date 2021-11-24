using UnityEngine;

namespace Inventory_System
{
    [CreateAssetMenu(fileName = "New Boot Armour", menuName = "Items/Armour/Create Boot Armour")]
    public class BootItem : ArmourItem
    {
        [Header("Boot Statistics")] 
        
        [Range(0, 10)] [Tooltip("The percentage increase of the players movement speed")]
        public float moveSpeedIncrease;
        
        public override void RandomlyGenerateItem()
        {
            
        }
    }
}