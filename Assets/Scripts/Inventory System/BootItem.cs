using UnityEngine;

namespace Inventory_System
{
    [CreateAssetMenu(fileName = "New Boot Armour", menuName = "Items/Armour/Create Boot Armour")]
    public class BootItem : ArmourItem
    {
        [Header("Boot Statistics")]
        
        [Range(0, 10)] [Tooltip("The percentage increase of the players movement speed")]
        public float moveSpeedIncrease;

        [Space] [Header("Random Generation")] 
        
        [Range(-2, 50)]
        public float minMoveSpeedIncrease;

        [Range(-2, 50)]
        public float maxMoveSpeedIncrease;

        public override void RandomlyGenerateItem()
        {
            base.RandomlyGenerateItem();
            moveSpeedIncrease = Random.Range(minMoveSpeedIncrease, maxMoveSpeedIncrease);
        }
    }
}