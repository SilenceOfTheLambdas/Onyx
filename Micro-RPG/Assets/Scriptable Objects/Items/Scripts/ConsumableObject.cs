using UnityEngine;
using UnityEngine.Serialization;

namespace Scriptable_Objects.Items.Scripts
{
    [CreateAssetMenu(fileName = "New Consumable Object", menuName = "Inventory System/Items/Consumable")]
    public class ConsumableObject : ItemObject
    {
        public enum HealthPotionTypes
        {
            None,
            Small,
            Medium,
            Large
        }

        [FormerlySerializedAs("PotionTypes")] [Header("Set Consumable Properties")] 
        public HealthPotionTypes healthPotionType;
        public int healthRestoreAmount;
        public int foodRestoreAmount;
        public int waterRestoreAmount;

        public void Awake()
        {
            type = ItemTypes.Consumable;

            switch (healthPotionType)
            {
                case HealthPotionTypes.None:
                    healthRestoreAmount = 0;
                    break;
                case HealthPotionTypes.Small:
                    healthRestoreAmount = 12;
                    break;
                case HealthPotionTypes.Medium:
                    healthRestoreAmount = 18;
                    break;
                case HealthPotionTypes.Large:
                    healthRestoreAmount = 26;
                    break;
            }
        }
    }
}
