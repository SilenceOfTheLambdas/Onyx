using UnityEngine;

namespace Inventory_System
{
    [CreateAssetMenu(fileName = "New Helmet Armour", menuName = "Items/Armour/Create Helmet")]
    public class HelmetItem : ArmourItem
    {
        [Header("Helmet Statistics")]

        [Range(0, 20)] [Tooltip("(Percentage) The amount of mana regenerated every second whilst wearing this item")]
        public int manaRegenerationPercentage = 0;
        
        [Range(0, 20)] [Tooltip("The percentage reduction of Mana EVERY skill")]
        public int reducedManaCostOfSkillsAmount = 0;

        public override void RandomlyGenerateItem()
        {
            
        }
    }
}