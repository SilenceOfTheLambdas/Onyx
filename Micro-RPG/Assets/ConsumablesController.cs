using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// This class is responsible for activating consumable items and using them.
/// </summary>
public class ConsumablesController : MonoBehaviour
{
    [SerializeField] private Player player;
    private                  bool   _hasHealthPot, _hasManaPot;

    // Update is called once per frame
    void Update()
    {
        // Check to see if the player has a health pot, or mana one
        if (player.inventory.ContainsItem(0))
        {
            _hasHealthPot = true;
        }

        if (player.inventory.ContainsItem(7)) _hasManaPot = true;

        if (Input.GetKeyDown(KeyCode.Alpha1) && _hasHealthPot)
        {
            UseHealthPot();
        }
        else Console.WriteLine("Player has no health pots");

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UseManaPot();
        }
    }

    /// <summary>
    /// Called when the user presses the key to activate
    /// </summary>
    public void UseHealthPot()
    {
        var healthPot = player.inventory.container.items.First(o => o.id == 0).item;
        player.inventory.RemoveItem(healthPot);
        player.curHp += 10;
    }

    public void UseManaPot()
    {
    }
}