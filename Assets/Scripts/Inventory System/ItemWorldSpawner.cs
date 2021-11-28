using UnityEngine;

namespace Inventory_System
{
    /// <summary>
    /// Called at the start of the level, it's responsible for spawning the pickup-able objects in the world.
    /// </summary>
    public class ItemWorldSpawner : MonoBehaviour
    {
        [Tooltip("The item to spawn")]
        public Item item;

        private void Start()
        {
            // ItemWorld.SpawnItemWorld(transform.position, item);
            item.itemWorldPrefab.GetComponent<ItemWorld>().SpawnItemWorld(transform.position, item);
            Destroy(gameObject);
        }
    }
}
