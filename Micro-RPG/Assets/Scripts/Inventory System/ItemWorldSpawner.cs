using System;
using UnityEngine;

namespace Inventory_System
{
    public class ItemWorldSpawner : MonoBehaviour
    {
        public Item item;

        private void Start()
        {
            ItemWorld.SpawnItemWorld(transform.position, item);
            Destroy(gameObject);
        }
    }
}
