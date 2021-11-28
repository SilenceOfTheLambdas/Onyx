using TMPro;
using UnityEngine;

namespace Inventory_System
{
    public class ItemWorld : MonoBehaviour
    {
        [SerializeField] private GameObject commonItemEffect;
        [SerializeField] private GameObject rareItemEffect;
        [SerializeField] private GameObject epicItemEffect;
        [SerializeField] private GameObject uniqueItemEffect;

        private float _timer;
        public void SpawnItemWorld(Vector3 position, Item item, int amount = 1)
        {
            for (var i = 0; i < amount; i++)
            {
                var worldObject = Instantiate(item.itemWorldPrefab, position, Quaternion.identity);
                worldObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                var itemWorld     = worldObject.GetComponent<ItemWorld>();

                var newUniqueItem = Instantiate(item);
                newUniqueItem.name = item.name;
                switch (newUniqueItem)
                {
                    // Randomly Generate Item Stats
                    case WeaponItem { randomlyGenerateStats: true } weaponItem:
                        weaponItem.RandomlyGenerateItem();
                        break;
                    case HelmetItem { randomlyGenerateStats: true } helmetItem:
                        helmetItem.RandomlyGenerateItem();
                        break;
                    case ChestItem { randomlyGenerateStats: true } chestItem:
                        chestItem.RandomlyGenerateItem();
                        break;
                    case BootItem { randomlyGenerateStats: true } bootItem:
                        bootItem.RandomlyGenerateItem();
                        break;
                }

                var itemEffectParent = worldObject.transform.Find("ItemEffect");
                switch (newUniqueItem.itemRarity)
                {
                    case ItemRarity.Common:
                        Instantiate(commonItemEffect, parent: itemEffectParent);
                        break;
                    case ItemRarity.Rare:
                        Instantiate(rareItemEffect, parent: itemEffectParent);
                        break;
                    case ItemRarity.Epic:
                        Instantiate(epicItemEffect, parent: itemEffectParent);
                        break;
                    case ItemRarity.Unique:
                        Instantiate(uniqueItemEffect, parent: itemEffectParent);
                        break;
                }

                itemWorld.SetItem(newUniqueItem);
            }
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
