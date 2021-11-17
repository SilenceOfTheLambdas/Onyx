using System;
using System.Collections.Generic;
using Skills;
using TMPro;
using Player;
using TheKiwiCoder;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Enemies
{
    [RequireComponent(typeof(Rigidbody), typeof(NavMeshAgent))]
    public class Enemy : MonoBehaviour
    {
        public string enemyName;
        public int CurHp { get; private set; }
        
        [SerializeField] private Skill attackSkill;

        [Header("Enemy Statistics")]
        [Space]
        [Tooltip("The level of this enemy, the level is used as a multiplier for the XP given on death")]
        public int enemyLevel = 1;

        [Tooltip("The maximum amount of HP this enemy has")]
        public int maxHp;
        
        // These options are set via weapons
        public int   damage      = 2; // The maximum amount of damage the enemy can do
        public float attackRange = 1; // The maximum range in which the enemy can hit the player
        public float attackRate  = 1; // How quick the enemy can attack the player

        [Header("Enemy AI Parameters")]
        [Space]
        [Tooltip("The area in which the player can be detected")]
        public float playerDetectionRadius; // The radius of the circle used to detect the player
        public float meleeAttackRange;
        public float chaseRange;
        
        [Space] [Header("References")]
        [SerializeField] private GameObject damageNumberDisplayPrefab;

        public delegate Node.State EnemyHitEvent();
        public event EnemyHitEvent OnEnemyHit;

        /// <summary>
        /// This will be set according the the enemies level and their level,
        /// it's calculated as follows:
        /// <code>(5 + damage) + level</code>
        /// </summary>
        private int _xpToGive;

        // Is the enemy dead?
        [NonSerialized] public bool IsDead = false;

        [NonSerialized] private Player.Player _player;
        private AbilitiesSystem _playerAbilitySystem;

        private Animator _animator;

        // Private
        private                  float _damageOverTimeTimer;
        private static readonly  int   VelocityZ = Animator.StringToHash("VelocityZ");
        private static readonly  int   VelocityX = Animator.StringToHash("VelocityX");
        private static readonly  int   Speed     = Animator.StringToHash("Speed");
        [HideInInspector] public bool  wasHit;

        private void Awake()
        {
            // Get the player target
            _player = FindObjectOfType<Player.Player>();
            CurHp = maxHp;
            _xpToGive = (5 + damage) * enemyLevel;
            _animator = GetComponentInChildren<Animator>();
            _playerAbilitySystem = _player.GetComponent<AbilitiesSystem>();
        }

        private void Update()
        {
            // Animating
            var velocityZ = Vector3.Dot(GetComponent<NavMeshAgent>().velocity.normalized, transform.forward);
            var velocityX = Vector3.Dot(GetComponent<NavMeshAgent>().velocity.normalized, transform.right);

            _animator.SetFloat(VelocityZ, velocityZ, 0.1f, Time.deltaTime);
            _animator.SetFloat(VelocityX, velocityX, 0.1f, Time.deltaTime);

            _animator.SetFloat(Speed, GetComponent<NavMeshAgent>().velocity.magnitude);
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

            // If the enemy is hit by a player weapon
            if (other.CompareTag("EquippedWeapon"))
            {
                var enemyHitDetection = other.GetComponent<EnemyHitDetection>();
                if (enemyHitDetection != null)
                {
                    // Perform a range check
                    if (Vector3.Distance(transform.position, _player.gameObject.transform.position) <= _player.GetComponent<PlayerEquipmentManager>().weaponItem.weaponRange)
                        TakeDamage(enemyHitDetection.playerAbilitySystem.CalculatePhysicalDamage());
                }
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

        public void TakeDamage(int damageTaken)
        {
            OnEnemyHit?.Invoke();
            wasHit = true;
            if (GetComponent<DamageEffect>() != null) {
                GetComponent<DamageEffect>().Activate();
            }

            // Display the amount of damage the player has dealt
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
            _playerAbilitySystem.AddXp(_xpToGive);
            IsDead = true; // set the enemy to deadness :)
            GameManager.Instance.playerUi.ToggleEnemyInfoPanel(false);
            SpawnItemsOnDeath();
            
            // TODO: Change to play a death animation
            Destroy(gameObject);
        }

        /// <summary>
        /// Spawn items when this enemy is killed
        /// </summary>
        private void SpawnItemsOnDeath()
        {
            GetComponent<LootTable>().SpawnItems(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z));
        }

        /// <summary>
        /// Called by an AnimationEvent in the enemy Animator
        /// </summary>
        public void Attack()
        {
            _player.TakeDamage(damage);
        }

        #region Helper

#if UNITY_EDITOR
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
#endif
        #endregion
    }
}