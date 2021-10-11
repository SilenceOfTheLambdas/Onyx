using System;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Inventory_System
{
    public class ItemWorld : MonoBehaviour
    {
        private float _timer;
        public static ItemWorld SpawnItemWorld(Vector3 position, Item item)
        {
            var worldObject = Instantiate(item.itemWorldPrefab, position, Quaternion.identity);
            worldObject.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            var  itemWorld     = worldObject.GetComponent<ItemWorld>();
            var newUniqueItem = Instantiate(item);
            newUniqueItem.name = item.name;
            itemWorld.SetItem(newUniqueItem);
            return itemWorld;
        }

        private Item _item;

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

            // When an item is spawned, it is given the layer Ignore Raycast, while it has this layermask
            // a timer will increment, and add the pickup layermask to this object after 1.2 seconds has passed
            if (gameObject.layer.Equals(LayerMask.NameToLayer("Ignore Raycast")))
            {
                _timer += Time.deltaTime;
                if (_timer >= 1.5f)
                {
                    gameObject.layer = LayerMask.NameToLayer("Pickup");
                    _timer = 0;
                }
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
    }
}
