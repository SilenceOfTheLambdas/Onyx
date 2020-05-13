using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Items
    {
        Gold_Coin,
        Sword,
        HP_Potion,
        Hat,
        Gloves
    }

    public Items selectedItem;

    public void PickupItem()
    {
        FindObjectOfType<Player>().AddItemToInventory(selectedItem);
        Destroy(gameObject);
    }
}
