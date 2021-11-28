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
        
        [Space] [Header("Random Helmet Statistics")]

        [Range(0, 20)] [InspectorName("Min Mana Regen")]
        public int minManaRegenAmount;
        
        [Range(0, 20)] [InspectorName("Max Mana Regen")]
        public int maxManaRegenAmount;

        [Range(0, 20)] [InspectorName("Min Skill Cost Reduction")]
        public int minReducedManaCostOfSkillsAmount;
        
        [Range(0, 20)] [InspectorName("Max Skill Cost Reduction")]
        public int maxReducedManaCostOfSkillsAmount;
        
        public override void RandomlyGenerateItem()
        {
            base.RandomlyGenerateItem();
            manaRegenerationPercentage = Random.Range(minManaRegenAmount, maxManaRegenAmount);
            reducedManaCostOfSkillsAmount =
                Random.Range(minReducedManaCostOfSkillsAmount, maxReducedManaCostOfSkillsAmount);
        }
    }
}