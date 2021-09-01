using System;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Inventory_System
{
    public class ItemWorld : MonoBehaviour
    {
        public static ItemWorld SpawnItemWorld(Vector3 position, Item item)
        {
            var transform = Instantiate(item.itemWorldPrefab, position, Quaternion.identity);

            var  itemWorld     = transform.GetComponent<ItemWorld>();
            var newUniqueItem = Object.Instantiate(item);
            itemWorld.SetItem(newUniqueItem);
            return itemWorld;
        }
        
        private Item           _item;

        private void Awake()
        {
            GetComponent<SpriteRenderer>();
            GetComponentInChildren<CameraFacingBillboard>().mCamera = Camera.main;
        }

        private void Update()
        {
            if (_item != null)
            {
                GetComponentInChildren<TextMeshProUGUI>().SetText($"{_item.itemName}");
            }
        }

        private void SetItem(Item item)
        {
            _item = item;
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
            Vector3 randomDirection = Random.insideUnitCircle.normalized;
            item.amount = 1; // Reset the amount back to 1, as we only drop 1 item at a time
            var     itemWorld       = SpawnItemWorld(dropPosition + randomDirection * 1f, item);
            return itemWorld;
        }
    }
}
