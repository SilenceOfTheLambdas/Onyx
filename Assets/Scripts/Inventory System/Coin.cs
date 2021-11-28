using UnityEngine;

namespace Inventory_System
{
    /// <summary>
    /// This class is simply used to hold the coin currency, atm it's empty but I may want to create
    /// different types of currency.
    /// </summary>
    [CreateAssetMenu(fileName = "New Coin", menuName = "Items/Create Coin", order = 100)]
    public class Coin : Item
    {
    }
}