using System.Collections;
using System.Collections.Generic;
using Scriptable_Objects.Inventory.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

/// <summary>
/// A dynamic interface is used mainly for interfaces that change. For example; a player inventory would be dynamic,
/// but something like an equipment system would be a static interface.
/// </summary>
public class DynamicInterface : UserInterface
{
    public GameObject inventoryPrefab;
    [FormerlySerializedAs("X_START")] public int xStart;
    [FormerlySerializedAs("Y_START")] public int yStart;
    [FormerlySerializedAs("X_SPACE_BETWEEN_ITEM")] public int xSpaceBetweenItem;
    [FormerlySerializedAs("NUMBER_OF_COLUMN")] public int numberOfColumn;
    [FormerlySerializedAs("Y_SPACE_BETWEEN_ITEMS")] public int ySpaceBetweenItems;
    public override void CreateSlots()
    {
        itemsDisplayed = new Dictionary<GameObject, InventorySlot>();
        for (int i = 0; i < inventory.container.Items.Length; i++)
        {
            var obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, transform);
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);

            AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
            AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });
            AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj); });
            AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });
            AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });


            itemsDisplayed.Add(obj, inventory.container.Items[i]);
        }
    }
    
    private Vector3 GetPosition(int i)
    {
        return new Vector3(xStart + (xSpaceBetweenItem * (i % numberOfColumn)), yStart + (-ySpaceBetweenItems * (i / numberOfColumn)), 0f);
    }
}
