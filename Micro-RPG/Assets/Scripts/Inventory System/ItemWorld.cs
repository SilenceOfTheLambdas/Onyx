using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Inventory_System
{
    public class ItemWorld : MonoBehaviour
    {
        public static ItemWorld SpawnItemWorld(Vector3 position, Item item)
        {
            Transform transform = Instantiate(ItemAssets.Instance.pfItemWorld, position, Quaternion.identity);

            ItemWorld itemWorld = transform.GetComponent<ItemWorld>();
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

        public void SetItem(Item item)
        {
            _item = item;
            _spriteRenderer.sprite = item.GetSprite();
            if (item.amount > 1)
            {
                _textMeshPro.SetText(item.amount.ToString());
            }
            else
            {
                _textMeshPro.SetText("");
            }
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
            ItemWorld itemWorld       = SpawnItemWorld(dropPosition + randomDirection * 1f, item);
            //itemWorld.GetComponent<Rigidbody2D>().AddForce(randomDirection * 0.1f, ForceMode2D.Impulse);
            return itemWorld;
        }
    }
}
