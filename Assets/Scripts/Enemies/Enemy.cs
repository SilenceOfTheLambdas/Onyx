using System;
using System.Collections.Generic;
using AI;
using AI.States;
using Skills;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Enemies
{
    [RequireComponent(typeof(Rigidbody), typeof(NavMeshAgent))]
    public class Enemy : MonoBehaviour
    {
        public enum EnemyType
        {
            Mage,
            Soldier,
            Warrior,
            Knight,
            Thief
        }

        /// <summary>
        /// This will specify what items this enemy can carry,
        /// the loot it can have, and the sprite set (how the enemy will look)
        /// </summary>
        public EnemyType enemyType;

        [SerializeField] private Skill attackSkill;

        [Header("Enemy Statistics")] [Space]
        
        [Tooltip("The level of this enemy, the level is used as a multiplier for the XP given on death")]
        public int enemyLevel = 1;
        
        [Tooltip("The maximum amount of HP this enemy has")]
        public  int maxHp;

        public int CurHp { get; private set; }
        
        // These options are set via weapons
        public int   damage     = 2; // The maximum amount of damage the enemy can do
        public float attackRange = 1; // The maximum range in which the enemy can hit the player
        public float attackRate = 1; // How quick the enemy can attack the player

        [SerializeField] private List<Transform> patrolPoints;
        public List<Transform> PatrolPoints => patrolPoints;

        [Header("Enemy AI Parameters")] [Space]
        
        [SerializeField] [Tooltip("The area in which the player can be detected")]
        public float playerDetectionRadius; // The radius of the circle used to detect the player
        [SerializeField] private float meleeAttackRange;
        [SerializeField] private float chaseRange;

        [SerializeField] private GameObject damageNumberDisplayPrefab;
        

        /// <summary>
        /// This will be set according the the enemies level and their level,
        /// it's calculated as follows:
        /// <code>(5 + damage) + level</code>
        /// </summary>
        private int _xpToGive;

        /// <summary>
        /// The any force that needs to be applied to the rigidbody.
        /// </summary>
        private Vector2 _movementForce;

        // Is the enemy dead?
        [NonSerialized] public bool IsDead = false;

        [NonSerialized] private Player.Player _player;

        private StateMachine _stateMachine;

        // Components
        private Rigidbody       _rig;
        private TextMeshProUGUI _currentStateText;
        
        // Timers
        private                 float _damageOverTimeTimer;
        private static readonly int   VelocityZ = Animator.StringToHash("VelocityZ");
        private static readonly int   VelocityX = Animator.StringToHash("VelocityX");

        private void Awake()
        {
            // Get the player target
            _player = FindObjectOfType<Player.Player>();
            // Get the rigid body comp
            _rig = GetComponent<Rigidbody>();
            CurHp = maxHp;
            _xpToGive = (5 + damage) * enemyLevel;
            _stateMachine = new StateMachine();
            _currentStateText = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void Start()
        {
            var wanderState  = new WanderState(this);
            var chasePlayer  = new Chase(_player.gameObject, this);
            var attackPlayer = new MeleeAttack(this, _player.gameObject);

            // Go from patrolling to chasing the player, is the player is in the enemies' sight
            // _stateMachine.AddTransition(patrol, chasePlayer, () => IsPlayerInSight() && !IsPlayerWithinAttackRange());
            
            At(wanderState, chasePlayer, CanChasePlayer());
            
            if (enemyType == EnemyType.Mage)
            {
                var attackPlayerWithFlameShot = new FlameShotAttack(this, _player.gameObject, transform.Find("SkillSpawn"), attackSkill);
                
                _stateMachine.AddAnyTransition(attackPlayerWithFlameShot, () =>
                {
                    if (IsPlayerInSight().Invoke() && IsPlayerWithinSkillAttackRange().Invoke())
                    {
                        return true;
                    }

                    return false;
                });
                
                At(attackPlayerWithFlameShot, chasePlayer, () =>
                {
                    if (!IsPlayerWithinSkillAttackRange().Invoke() && CanChasePlayer().Invoke()) return true;
                    return false;
                });

                At(attackPlayerWithFlameShot, attackPlayer, CanMeleeAttack());
            } 
            else if (enemyType != EnemyType.Mage)
                At(chasePlayer, attackPlayer, CanMeleeAttack());
            
            At(attackPlayer, chasePlayer, () => !CanMeleeAttack().Invoke() && CanChasePlayer().Invoke());
            
            At(chasePlayer, wanderState, () => !IsPlayerInSight().Invoke()); // if player is not insight
            
            _stateMachine.AddAnyTransition(wanderState, () => !IsPlayerInSight().Invoke());
            
            // Set the default state to patrolling
            _stateMachine.SetState(wanderState);

            void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);

            Func<bool> IsPlayerInSight() => () => GetComponent<FieldOfView>().canSeePlayer;

            Func<bool> IsPlayerWithinChaseRange() => () =>
                Vector3.Distance(transform.position, _player.transform.position) <= chaseRange &&
                Vector3.Distance(transform.position, _player.transform.position) > attackRange;
            
            Func<bool> IsPlayerWithinSkillAttackRange() => () =>
                Vector3.Distance(transform.position, _player.transform.position) <= attackRange &&
                Vector3.Distance(transform.position, _player.transform.position) > meleeAttackRange;

            Func<bool> IsPlayerWithinMeleeAttackRange() => () =>
                Vector3.Distance(transform.position, _player.transform.position) <= meleeAttackRange;

            Func<bool> CanChasePlayer() => () => IsPlayerInSight().Invoke() && IsPlayerWithinChaseRange().Invoke();
            Func<bool> CanMeleeAttack() => () => IsPlayerInSight().Invoke() && IsPlayerWithinMeleeAttackRange().Invoke();
        }

        private void Update()
        {
            // Update State Machine
            _stateMachine.Tick();
            _currentStateText.SetText($"{_stateMachine.GetCurrentState()}");
            // Debug.Log($"Current State: {_stateMachine.GetCurrentState()}");
            
            // Animating
            var velocityZ = Vector3.Dot(GetComponent<NavMeshAgent>().velocity.normalized, transform.forward);
            var velocityX = Vector3.Dot(GetComponent<NavMeshAgent>().velocity.normalized, transform.right);
            
            GetComponentInChildren<Animator>().SetFloat(VelocityZ, velocityZ, 0.1f, Time.deltaTime);
            GetComponentInChildren<Animator>().SetFloat(VelocityX, velocityX, 0.1f, Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            // If the enemy is hit with a projectile that IS NOT tagged enemy
            if (other.GetComponent<SkillProjectile>() && !other.gameObject.CompareTag("Enemy"))
            {
                TakeDamage(other.GetComponent<SkillProjectile>().Skill.amountOfDamage);
                transform.LookAt(other.transform);
                Destroy(other.gameObject);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            _damageOverTimeTimer += Time.deltaTime;
            if (other.GetComponent<BeamManager>() && !other.gameObject.CompareTag("Enemy"))
            {
                if (_damageOverTimeTimer >= other.GetComponent<BeamManager>().Skill.dotTimeMultiplier)
                {
                    TakeDamage(other.GetComponent<BeamManager>().Skill.amountOfDamage);
                    _damageOverTimeTimer = 0f;
                }
            }
        }

        private void FixedUpdate()
        {
            _stateMachine.FixedTick();
        }

        public void TakeDamage(int damageTaken)
        {
            // Display the amount of damage the player has dealt
            var randomXPosition     = Random.Range(transform.position.x - 0.2f, transform.position.x + 0.2f);
            var damageNumberDisplay = Instantiate(damageNumberDisplayPrefab, transform.Find("Canvas"));
            
            // Randomly set the anchored X position
            var rectTransform = damageNumberDisplay.GetComponent<RectTransform>().anchoredPosition3D;
            rectTransform = new Vector3(Random.Range(rectTransform.x - 5f, rectTransform.x + 5f), 0f, 0f);
            damageNumberDisplay.GetComponent<RectTransform>().anchoredPosition3D = rectTransform;

            damageNumberDisplay.GetComponent<TextMeshProUGUI>().SetText($"-{damageTaken}"); // Update the damage text
            CurHp -= damageTaken;
            
            GameManager.Instance.playerUi.ToggleEnemyInfoPanel(true);
            GameManager.Instance.playerUi.UpdateEnemyInformationPanel(this);
            if (CurHp <= 0) Die();
        }

        private void Die()
        {
            _player.AddXp(_xpToGive);
            IsDead = true; // set the enemy to deadness :)
            GameManager.Instance.playerUi.ToggleEnemyInfoPanel(false);
            Destroy(gameObject);
        }

        // Called by Animation event
        public void Attack()
        {
            var damageModifier = (_player.strengthPhysicalDamageIncreaseAmount * _player.strength);
            _player.TakeDamage(damage - damageModifier);
        }

        #region Helper

        private void OnDrawGizmosSelected()
        {
            var enemyPosition = transform.position;
            // Draw player detection radius circle
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(enemyPosition, playerDetectionRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(enemyPosition, attackRange);
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(enemyPosition, meleeAttackRange);
        }

        #endregion
    }
}