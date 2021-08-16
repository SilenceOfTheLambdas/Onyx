using UnityEngine;

namespace Inventory_System
{
    public class ItemAssets : MonoBehaviour
    {
        public static ItemAssets Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public Transform pfItemWorld;
        
        public Sprite woodenSwordSprite;
        public Sprite spellBookSprite;
        public Sprite healthPotionSprite;
        public Sprite manaPotionSprite;
        public Sprite coinSprite;
    }
}
