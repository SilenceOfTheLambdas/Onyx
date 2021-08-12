using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Inventory_System
{
    public class ItemWorld : MonoBehaviour
    {
        public static ItemWorld SpawnItemWorld(Vector3 position, Item item)
        {
            var transform = Instantiate(ItemAssets.Instance.pfItemWorld, position, Quaternion.identity);

            var itemWorld = transform.GetComponent<ItemWorld>();
            itemWorld.SetItem(item);
            
            return itemWorld;
        }
        
        private Item           _item;
        private SpriteRenderer _spriteRenderer;
        private TextMeshPro    _textMeshPro;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _textMeshPro = transform.Find("Text").GetComponent<TextMeshPro>();
        }

        private void SetItem(Item item)
        {
            _item = item;
            _spriteRenderer.sprite = item.GetSprite();
            _textMeshPro.SetText(item.amount > 1 ? item.amount.ToString() : "");
        }

        public Item GetItem()
        {
            return _item;
        }

        public void DestroySelf()
        {
            Destroy(gameObject);
        }

        public static ItemWorld DropItem(Vector3 dropPosition, Item item)
        {
            Vector3   randomDirection = Random.insideUnitCircle.normalized;
            var itemWorld       = SpawnItemWorld(dropPosition + randomDirection * 1f, item);
            return itemWorld;
        }
    }
}
