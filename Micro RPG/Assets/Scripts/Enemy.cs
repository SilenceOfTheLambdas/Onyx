using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Enemy : MonoBehaviour
{
    [Header("Stats")] 
    public float moveSpeed;
    [FormerlySerializedAs("curHP")] public int curHp;
    [FormerlySerializedAs("maxHP")] public int maxHp;

    [Header("Target")] 
    public float chaseRange;
    public float attackRange;

    [Header("Attack")] 
    public int damage;
    public int xpToGive;
    public float attackRate;
    private float _lastAttackTime;
    
    private Player _player;
    
    // Components
    private Rigidbody2D _rig;

    private void Awake()
    {
        // Get the player target
        _player = FindObjectOfType<Player>();
        // Get the rigid body comp
        _rig = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float playerDist = Vector2.Distance(transform.position, _player.transform.position);

        if (playerDist <= attackRange)
        {
            // Attack the player
            if (Time.time - _lastAttackTime >= attackRate)
            {
                Attack();
            }
            _rig.velocity = Vector2.zero;

        } else if (playerDist <= chaseRange)
        {
            Chase();
        }
        else
        {
            _rig.velocity = Vector2.zero;
        }
    }

    void Chase()
    {
        Vector2 dir = (_player.transform.position - transform.position).normalized;

        _rig.velocity = dir * moveSpeed;
    }

    public void TakeDamage(int damageTaken)
    {
        curHp -= damageTaken;

        if (curHp <= 0) Die();
    }

    void Die()
    {
        _player.AddXp(xpToGive);
        Destroy(gameObject);
    }

    void Attack()
    {
        _lastAttackTime = Time.time;
        _player.TakeDamage(damage);
    }
}
