using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory_System
{
    public class UI_Inventory : MonoBehaviour
    {
        private Inventory _inventory;
        private Transform itemSlotContainer;
        private Transform itemSlotTemplate;
        private Player    _player;

        private void Awake()
        {
            itemSlotContainer = transform.Find("itemSlotContainer");
            itemSlotTemplate = itemSlotContainer.Find("itemSlotTemplate");
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
            foreach (Transform child in itemSlotContainer)
            {
                if (child == itemSlotTemplate) continue;
                Destroy(child.gameObject);
            }
            var x                = 0;
            var y                = 0;
            var itemSlotCellSize = 80f;
            
            foreach (var item in _inventory.GetItemList())
            {
                RectTransform itemSlotRectTransform =
                    Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
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
                    Item duplicateItem = new Item {itemType = item.itemType, amount = item.amount};
                    _inventory.RemoveItem(item);
                    ItemWorld.DropItem(_player.transform.position, duplicateItem);
                };
                
                itemSlotRectTransform.anchoredPosition = new Vector2(x * itemSlotCellSize, y * itemSlotCellSize);
                
                Image image = itemSlotRectTransform.Find("image").GetComponent<Image>();
                image.sprite = item.GetSprite();

                TextMeshProUGUI uiText = itemSlotRectTransform.Find("amountText").GetComponent<TextMeshProUGUI>();
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
