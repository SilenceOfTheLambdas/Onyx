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

        [Header("Enemy AI Parameters")] 
        [SerializeField] public float playerDetectionRadius; // The radius of the circle used to detect the player

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
        }

        private void Start()
        {
            var idle   = new Idle();
            var patrol = new Patrol(this, GetComponentInChildren<Animator>(), GetComponent<Seeker>(),1f, patrolPoints);
            var chasePlayer = new Chase(_player.gameObject, this, GetComponent<Seeker>());
            _stateMachine.AddTransition(idle, patrol, () => true);
            _stateMachine.AddAnyTransition(chasePlayer, IsPlayerWithinRange);
            _stateMachine.AddTransition(chasePlayer, patrol, () => Vector2.Distance(_rig.position, _player.transform.position) > playerDetectionRadius);
            _stateMachine.SetState(patrol);
        }

        private void Update()
        {
            // Update State Machine
            _stateMachine.Tick();
            
        }

        private void FixedUpdate()
        {
            _stateMachine.FixedTick();
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

        #region Helper

        private bool IsPlayerWithinRange() => Vector2.Distance(_rig.position, _player.gameObject.transform.position) <= playerDetectionRadius;
        
        private void OnDrawGizmosSelected()
        {
            // Draw player detection radius circle
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
        }

        #endregion
    }
}
