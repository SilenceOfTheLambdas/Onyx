﻿using System.Collections.Generic;
using AI;
using AI.States;
using Pathfinding;
using UnityEngine;
using UnityEngine.UI;
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

        [Header("Enemy AI Parameters")] 
        [SerializeField] public float playerDetectionRadius; // The radius of the circle used to detect the player

        [SerializeField] private float fov = 90f;

        [Header("Enemy HUD")] [SerializeField] private Image hpFillImage;
        
        /// <summary>
        /// This will specify the maximum amount of HP the enemy has,
        /// along with the max damage within it's class' restrictions,
        /// and Chase Range 
        /// </summary>
        public int enemyLevel = 1;
    
        // Set according to the enemy's type/tier
        private int   _curHp;
        public  int   maxHp;

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
        public float attackRange = 1; // The maximum range in which the enemy can hit the player
        public float enemyStoppingDistance;
        public int   damage     = 2; // The maximum amount of damage the enemy can do
        public float attackRate = 1; // How quick the enemy can attack the player

        // Is the enemy dead?
        public bool isDead = false;

        public Player _player;

        private float        _lastAttackTime;
        private StateMachine _stateMachine;

        // Components
        private                  Rigidbody2D _rig;

        private void Awake()
        {
            // Get the player target
            _player = FindObjectOfType<Player>();
            // Get the rigid body comp
            _rig = GetComponent<Rigidbody2D>();
            _curHp = maxHp;
            _xpToGive = damage + enemyLevel;
            _stateMachine = new StateMachine();
        }

        private void Start()
        {
            var idle   = new Idle();
            var patrol = new Patrol(this, GetComponentInChildren<Animator>(), GetComponent<Seeker>(),1f, patrolPoints);
            var chasePlayer = new Chase(_player.gameObject, this, GetComponent<Seeker>());
            var attackPlayer = new MeleeAttack(this);
            _stateMachine.AddTransition(idle, patrol, () => true);
            
            // Go from patrolling to chasing the player, is the player is in the enemies' sight
            _stateMachine.AddTransition(patrol, chasePlayer, IsPlayerInSight);
            // While the enemy is chasing the player, if they lose sight of the player, the enemy will go back to patrolling
            _stateMachine.AddTransition(chasePlayer, patrol, () => !IsPlayerInSight());
            // At any point, the enemy will attack the player if they are in sight, and within the melee attack range
            _stateMachine.AddAnyTransition(attackPlayer, () => IsPlayerInSight() && IsPlayerWithinMeleeAttackRange());
            // If the player moves outside of the enemies' melee attack range, but it still in sight, the enemy will chase the player
            _stateMachine.AddTransition(attackPlayer, chasePlayer, () => IsPlayerInSight() && !IsPlayerWithinMeleeAttackRange());
            
            // Set the default state to patrolling
            _stateMachine.SetState(patrol);
            
            // Update enemy HP HUD
            UpdateEnemyHpBarFill();
        }

        private void Update()
        {
            // Update State Machine
            _stateMachine.Tick();
            //Debug.Log($"Current State: {_stateMachine.GetCurrentState()}");
        }

        private void FixedUpdate()
        {
            _stateMachine.FixedTick();
        }

        public void TakeDamage(int damageTaken)
        {
            _curHp -= damageTaken;
            UpdateEnemyHpBarFill();
            if (_curHp <= 0) Die();
        }

        private void Die()
        {
            _player.AddXp(_xpToGive);
            isDead = true; // set the enemy to deadness :)
            Destroy(gameObject);
        }

        public void Attack()
        {
            _lastAttackTime = Time.time;
            _player.TakeDamage(damage);
        }

        #region Helper

        private bool IsPlayerInSight()
        {
            if (Vector3.Distance(transform.position, _player.transform.position) < playerDetectionRadius)
            {
                var directionToPlayer = (_player.transform.position - transform.position).normalized;
                var aimDirection      = ((Vector3.down * playerDetectionRadius) - transform.position).normalized;
                if (Vector3.Angle(aimDirection, directionToPlayer) < fov)
                {
                    return true;
                }
            }

            return false;
        }
        
        private bool IsPlayerWithinRange() => Vector2.Distance(_rig.position, _player.gameObject.transform.position) <= playerDetectionRadius 
                                              && Vector2.Distance(_rig.position, _player.gameObject.transform.position) >= enemyStoppingDistance;
        private bool IsPlayerWithinMeleeAttackRange() => Vector2.Distance(_rig.position, _player.gameObject.transform.position) <= attackRange;

        private void UpdateEnemyHpBarFill() => hpFillImage.fillAmount = (float) _curHp / maxHp;
        
        private void OnDrawGizmosSelected()
        {
            // Draw player detection radius circle
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
        }

        #endregion
    }
}
