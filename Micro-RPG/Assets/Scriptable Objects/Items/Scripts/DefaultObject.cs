using UnityEngine;

namespace Scriptable_Objects.Items.Scripts
{
    [CreateAssetMenu(fileName = "New Default Object", menuName = "Inventory System/Items/Default")]
    public class DefaultObject : ItemObject {
        public void Awake()
        {
            type = ItemTypes.Default;
        }
    }
}
