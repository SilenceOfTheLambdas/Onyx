using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory_System
{
    public class UI_Inventory : MonoBehaviour
    {
        private Inventory _inventory;
        private Transform _itemSlotContainer;
        private Transform _itemSlotTemplate;
        private Player    _player;

        private void Awake()
        {
            _itemSlotContainer = transform.Find("itemSlotContainer");
            _itemSlotTemplate = _itemSlotContainer.Find("itemSlotTemplate");
        }

        public void SetPlayer(Player player)
        {
            _player = player;
        }
        
        public void SetInventory(Inventory inventory)
        {
            _inventory = inventory;
            inventory.OnItemListChanged += Inventory_OnItemListChanged;
            RefreshInventoryItems();
        }

        private void Inventory_OnItemListChanged(object sender, System.EventArgs e)
        {
            RefreshInventoryItems();
        }

        private void RefreshInventoryItems()
        {
            foreach (Transform child in _itemSlotContainer)
            {
                if (child == _itemSlotTemplate) continue;
                Destroy(child.gameObject);
            }
            var x                = 0;
            var y                = 0;
            var itemSlotCellSize = 80f;
            
            foreach (var item in _inventory.GetItemList())
            {
                var itemSlotRectTransform =
                    Instantiate(_itemSlotTemplate, _itemSlotContainer).GetComponent<RectTransform>();
                itemSlotRectTransform.gameObject.SetActive(true);
                
                // TODO: Might want to use a grid component instead
                
                // Action when left-clicking on an item
                itemSlotRectTransform.GetComponent<Button_UI>().ClickFunc = () =>
                {
                    // Use item
                    _inventory.UseItem(item);
                };
                
                // Action when right-clicking
                itemSlotRectTransform.GetComponent<Button_UI>().MouseRightClickFunc = () =>
                {
                    // Drop Item
                    var duplicateItem = item;
                    _inventory.RemoveItem(item);
                    ItemWorld.DropItem(_player.transform.position, duplicateItem);
                };
                
                itemSlotRectTransform.anchoredPosition = new Vector2(x * itemSlotCellSize, y * itemSlotCellSize);
                
                var image = itemSlotRectTransform.Find("image").GetComponent<Image>();
                image.sprite = item.GetSprite();

                var uiText = itemSlotRectTransform.Find("amountText").GetComponent<TextMeshProUGUI>();
                uiText.SetText(item.amount > 1 ? item.amount.ToString() : "");
                x++;
                if (x > 4)
                {
                    x = 0;
                    y--;
                }
            }
        }
    }
}
