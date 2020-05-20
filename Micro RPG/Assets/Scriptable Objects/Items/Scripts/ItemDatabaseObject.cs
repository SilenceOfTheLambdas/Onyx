using System.Collections.Generic;
using UnityEngine;

namespace Scriptable_Objects.Items.Scripts
{
    [CreateAssetMenu(fileName = "New Item Database", menuName = "Inventory System/Items/Database")]
    public class ItemDatabaseObject : ScriptableObject, ISerializationCallbackReceiver
    {
        public ItemObject[] items;
        public Dictionary<int, ItemObject> getItem = new Dictionary<int, ItemObject>();
    
        public void OnBeforeSerialize()
        {
            getItem = new Dictionary<int, ItemObject>();
        }

        public void OnAfterDeserialize()
        {
            for (int i = 0; i < items.Length; i++)
            {
                items[i].Id = i;
                getItem.Add(i, items[i]);
            }
        }
    }
}
