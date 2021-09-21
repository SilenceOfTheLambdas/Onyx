using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;

public class AttackPlayerEvent : MonoBehaviour
{
    private Enemy _enemy;

    private void Start()
    {
        _enemy = GetComponentInParent<Enemy>();
    }

    private void AttackPlayer()
    {
        _enemy.Attack();
    }
}
