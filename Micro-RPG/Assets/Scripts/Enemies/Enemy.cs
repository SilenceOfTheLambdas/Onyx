using System;
using System.Collections.Generic;
using AI;
using AI.States;
using Pathfinding;
using UnityEngine;
using Patrol = AI.States.Patrol;

namespace Enemies
{
    public class Enemy : MonoBehaviour
    {
        public enum EnemyType
        {
            Mage,
            Soilder,
            Warrior,
            Knight,
            Thief
        }
        /// <summary>
        /// This will specify what items this enemy can carry,
        /// the loot it can have, and the sprite set (how the enemy will look)
        /// </summary>
        public EnemyType enemyType;

        [Header("Enemy Movement")] [Range(0f, 100f)]
        public float moveSpeed = 38f; // These are set by the items the enemy carries/spawns with
        [SerializeField] private List<Transform> patrolPoints;
        
        /// <summary>
        /// This will specify the maximum amount of HP the enemy has,
        /// along with the max damage within it's class' restrictions,
        /// and Chase Range 
        /// </summary>
        public int enemyLevel = 1;
    
        // Set according to the enemy's type/tier
        private int   _curHp;
        public  int   maxHp;
        public  float chaseRange = 3;
        
        /// <summary>
        /// This will be set according the the enemies level and their level,
        /// it's calculated as follows:
        /// <code>(damage + level)</code>
        /// </summary>
        private int _xpToGive;

        /// <summary>
        /// The any force that needs to be applied to the rigidbody.
        /// </summary>
        private Vector2 _movementForce;
    
        // These options are set via weapons
        private float attackRange = 1; // The maximum range in which the enemy can hit the player
        public  int   damage      = 2; // The maximum amount of damage the enemy can do
        public  float attackRate  = 1; // How quick the enemy can attack the player

        // Is the enemy dead?
        public bool isDead = false;

        public Player _player;

        private float        _lastAttackTime;
        private StateMachine _stateMachine;

        // Components
        private Rigidbody2D _rig;
        private void Awake()
        {
            // Get the player target
            _player = FindObjectOfType<Player>();
            // Get the rigid body comp
            _rig = GetComponent<Rigidbody2D>();
            _curHp = maxHp;
            _xpToGive = damage + enemyLevel;
            _stateMachine = new StateMachine();

            var idle   = new Idle();
            var patrol = new Patrol(this, GetComponentInChildren<Animator>(), GetComponent<Seeker>() ,1f, patrolPoints);
            _stateMachine.AddTransition(idle, patrol, () => true);
            _stateMachine.SetState(patrol);
        }

        private void Update()
        {
            // Update State Machine
            _stateMachine.Tick();
            
            // if (!isDead)
            // {
            //     float playerDist = Vector2.Distance(transform.position, _player.transform.position);
            //
            //     if (playerDist <= attackRange)
            //     {
            //         // Attack the player
            //         if (Time.time - _lastAttackTime >= attackRate)
            //         {
            //             Attack();
            //         }
            //         _rig.velocity = Vector2.zero;
            //
            //     }  
            // }
        }

        private void FixedUpdate()
        {
            _stateMachine.FixedTick();
        }

        private void Chase()
        {
            Vector2 dir = (_player.transform.position - transform.position).normalized;

            _rig.velocity = dir * moveSpeed;
        }

        public void TakeDamage(int damageTaken)
        {
            _curHp -= damageTaken;

            if (_curHp <= 0) Die();
        }

        private void Die()
        {
            _player.AddXp(_xpToGive);
            isDead = true; // set the enemy to deadness :)
            Destroy(gameObject);
        }

        private void Attack()
        {
            _lastAttackTime = Time.time;
            _player.TakeDamage(damage);
        }
    }
}
