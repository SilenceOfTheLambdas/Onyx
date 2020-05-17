using UnityEngine;

namespace Scriptable_Objects.Items.Scripts
{
    public enum ItemTypes
    {
        Consumable,
        Equipment,
        Default
    }
    public abstract class ItemObject : ScriptableObject
    {
        public GameObject prefab;
        public ItemTypes type;
        [TextArea(15, 20)]
        public string description;
    }
}
