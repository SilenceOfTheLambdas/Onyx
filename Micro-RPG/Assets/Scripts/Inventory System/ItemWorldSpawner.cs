using UnityEngine;

namespace Inventory_System
{
    public class ItemWorldSpawner : MonoBehaviour
    {
        [Tooltip("The item to spawn")]
        public Item item;

        private void Start()
        {
            ItemWorld.SpawnItemWorld(transform.position, item);
            Destroy(gameObject);
        }
    }
}
