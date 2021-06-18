using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;
using UnityEngine.PlayerLoop;

/// <summary>
/// A mage uses magic to eliminate enemies.
/// They have a medium health pool,
/// Ranged Weapons
/// </summary>
public class Mage : Enemy
{
    public Mage()
    {
        maxHp = (20 + enemyLevel);
    }
}
