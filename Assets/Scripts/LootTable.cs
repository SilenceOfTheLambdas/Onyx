using System;
using System.Collections.Generic;
using Inventory_System;
using UnityEngine;
using Random = UnityEngine.Random;

public class LootTable : MonoBehaviour
{
    [SerializeField] private List<LootItem> dropTable;

    public void SpawnItems(Vector3 dropPosition)
    {
        // Calculate the chance each item will drop
        foreach (var lootItem in dropTable)
        {
            var spawn = Random.Range(0.1f, 100f);
            if (spawn <= lootItem.spawnChance)
                DropItem(lootItem, dropPosition,Random.Range(lootItem.minSpawnAmount, lootItem.maxSpawnAmount));
        }
    }

    private void DropItem(LootItem lootItem, Vector3 dropPosition, int amount)
    {
        var randomDirection = Random.insideUnitSphere.normalized;
        lootItem.item.itemWorldPrefab.GetComponent<ItemWorld>()
            .SpawnItemWorld(dropPosition + randomDirection * 1f, lootItem.item, amount);
        // ItemWorld.SpawnItemWorld(dropPosition + randomDirection * 1f, lootItem.item, amount);
    }
}

[Serializable]
public class LootItem
{
    [Tooltip("The item that could spawn")]
    public Item item;

    [Tooltip("The chance this item will spawn at least the minimum spawn amount")] [Range(0.1f, 100f)]
    public float spawnChance;

    [Tooltip("The minimum spawn amount of this item")]
    public int minSpawnAmount;

    [Tooltip("The maximum spawn amount of this item")]
    public int maxSpawnAmount;
}
