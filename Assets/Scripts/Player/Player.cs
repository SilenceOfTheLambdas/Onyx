#region Imports
using System;
using Enemies;
using UnityEngine;
using Inventory_System;
using UI;
using UnityEngine.InputSystem;
using Item = Inventory_System.Item;
#endregion

namespace Player
{
    [RequireComponent(typeof(AbilitiesSystem))]
    public class Player : MonoBehaviour
    {
        public enum State
        {
            Normal,
            Attacking
        }

        #region Properties

        public int CurrentHp
        {
            get => _curHp;
            private set => _curHp = Mathf.Clamp(value, 0, maxHp);
        }

        #endregion

        #region Fields

        [Header("Player Statistics")]
        [Tooltip("The maximum amount of health the player has, this is affected by Strength")]
        public int maxHp; // our maximum health
        private int _curHp;

        public State state = State.Normal;
        [NonSerialized]     public  Controls        Controls;
        [NonSerialized]     public  PlayerUi        ui;
        [NonSerialized]     public  Inventory       Inventory;
        [SerializeField]    private UI_Inventory    uiInventory;
        [SerializeField]    private LayerMask       enemyHitableLayerMask;

        [Header("User Interface")]
        [SerializeField]
        private GameObject inventoryScreen; // Reference to the inventory UI

        [SerializeField] private GameObject skillTreeScreen;
        [HideInInspector] public bool inventoryOpen;
        [HideInInspector] public bool skillTreeOpen;

        // Private variables
        private ParticleSystem _hitEffect;
        private AbilitiesSystem _playerAbilitySystem;
        private PlayerEquipmentManager _playerEquipmentManager;
        private float _lastAttackTime; // last time we attacked
        private bool _didThePlayerClickOnItemBeforeMoving;

        #endregion

        private void Awake()
        {
            // get components
            ui = FindObjectOfType<PlayerUi>();
            _hitEffect = gameObject.GetComponentInChildren<ParticleSystem>();
            _playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
            _playerAbilitySystem = GetComponent<AbilitiesSystem>();
            state = State.Normal;
            Controls = new Controls();

            // Instantiate the inventory
            Inventory = new Inventory(UseItem);
            uiInventory.SetPlayer(this);
            uiInventory.SetInventory(Inventory);

            // Set current HP values
            _curHp = maxHp;
            CurrentHp = maxHp;
        }

        private void OnEnable() => Controls.Player.Enable();

        private void OnDisable() => Controls.Player.Disable();

        #region Object Pickup

        private void OnTriggerEnter(Collider other)
        {
            PlayerItemPickup(other);
        }

        /// <summary>
        /// Detect if the player is within an ItemWorld's sphere collider, and if so; 
        /// perform a raycast and pickup the object if and when the player presses the left mouse button over the object.
        /// </summary>
        /// <param name="other">The collider that is touching the player</param>
        private void PlayerItemPickup(Collider other)
        {
            // If we click
            if (Mouse.current.leftButton.IsPressed())
            {
                // Perform a raycast to detect if we are hovering over an item
                if (!SuperuserUtils.SuperuserUtils.Instance.IsTheMouseHoveringOverGameObject(LayerMask.GetMask("Pickup"), out var _))
                    return;
                
                var itemWorld = other.gameObject.GetComponent<ItemWorld>();
                if (!itemWorld) return;

                Inventory.AddItem(itemWorld.GetItem());
                itemWorld.DestroySelf();
            }

            if (_didThePlayerClickOnItemBeforeMoving)
            {
                var itemWorld = other.gameObject.GetComponent<ItemWorld>();
                if (!itemWorld) return;

                Inventory.AddItem(itemWorld.GetItem());
                itemWorld.DestroySelf();
            }
        }
        #endregion

        private void Start()
        {
            ui.UpdateLevelText();
            ui.UpdateSkillPointsText();
            inventoryScreen.SetActive(false);
        }

        private void Update()
        {
            // If the inventory is open, pause the game
            if (inventoryOpen || skillTreeOpen)
            {
                // 'Pause' the game
                Time.timeScale = 0f;
            }
            else // The player can only move if the inventory is NOT open
            {
                Time.timeScale = 1f;
            }

            // Open and close the inventory screen
            if (Controls.Player.Inventory.triggered && !skillTreeOpen)
            {
                inventoryOpen = !inventoryOpen;
                inventoryScreen.SetActive(inventoryOpen);

                if (inventoryOpen)
                    CursorController.Instance.SetCursor(CursorController.CursorTypes.Default);
            }

            if (Controls.Player.SkillTree.triggered && !inventoryOpen)
            {
                skillTreeOpen = !skillTreeOpen;
                skillTreeScreen.SetActive(skillTreeOpen);

                if (skillTreeOpen)
                    CursorController.Instance.SetCursor(CursorController.CursorTypes.Default);
            }

            // Detect if player clicked on an item in the world
            if (SuperuserUtils.SuperuserUtils.Instance.IsTheMouseHoveringOverGameObject(LayerMask.GetMask("Pickup"), out var _)
                && Mouse.current.leftButton.isPressed)
            {
                _didThePlayerClickOnItemBeforeMoving = true;
            }
            if (!SuperuserUtils.SuperuserUtils.Instance.IsTheMouseHoveringOverGameObject(LayerMask.GetMask("Pickup"), out var _)
                && Mouse.current.leftButton.isPressed)
            {
                _didThePlayerClickOnItemBeforeMoving = false;
            }
            //////////////////////////////////////////////////
            
            // Attacking
            PlayerMeleeAttack();
        }

        private void UseItem(Item item)
        {
            if (item is WeaponItem weaponItem)
            {
                if (!_playerEquipmentManager.hasWeaponEquipped)
                {
                    _playerEquipmentManager.EquipWeapon(weaponItem);
                    Inventory.RemoveItem(item);
                }
            }

            if (item is HelmetItem helmetItem)
            {
                if (_playerEquipmentManager.head == null)
                {
                    // Check to see if we have the correct requirements
                    if (_playerAbilitySystem.strength >= helmetItem.strengthRequirement &&
                        _playerAbilitySystem.intelligence >= helmetItem.intelligenceRequirement)
                    {
                        _playerEquipmentManager.EquipHelmet(helmetItem);
                        Inventory.RemoveItem(item);
                    }
                }
            }

            if (item is ChestItem chestItem)
            {
                if (_playerEquipmentManager.chest == null)
                {
                    if (_playerAbilitySystem.strength >= chestItem.strengthRequirement && _playerAbilitySystem.intelligence >= chestItem.intelligenceRequirement)
                    {
                        _playerEquipmentManager.EquipChest(chestItem);
                        Inventory.RemoveItem(item);
                    }
                }
            }

            if (item is HealthPotion healthPotion)
            {
                if (CurrentHp != maxHp)
                {
                    IncreaseHp(healthPotion.restoreAmount);
                    Inventory.RemoveItem(item);
                }
            }

            if (item is ManaPotion manaPotion)
            {

                _playerAbilitySystem.AttempToUseManaPotion(Inventory, manaPotion);
            }

            uiInventory.hoverInterface.SetActive(false);
        }

        private void PlayerMeleeAttack()
        {
            // Check to see if the player has a weapon Equipped
            if (!_playerEquipmentManager.hasWeaponEquipped) return;

            // Check for attack rate timer
            if (!(Time.time - _lastAttackTime >= _playerEquipmentManager.weaponItem.attackRate)) return;
            _lastAttackTime = Time.time;

            // Melee
            if (Mouse.current.rightButton.IsPressed() && !(GetComponent<Player>().inventoryOpen || GetComponent<Player>().skillTreeOpen))
            {
                // First we play the sword slash animation
                GetComponent<Animator>().SetBool("isAttack", true);
                GetComponent<Animator>().SetTrigger("SwordSlash");
            }
        }

        /// <summary>
        /// Called by the PlayerSwordSlash animation event. Does a raycast to check if we have hit an enemy, and if so, deal damage to that enemy.
        /// </summary>
        public void CheckAndDealDamageToEnemy()
        {
            if (SuperuserUtils.SuperuserUtils.Instance.IsTheMouseHoveringOverGameObject(enemyHitableLayerMask, out var o))
            {
                if (o != null)
                {
                    var enemy = o.GetComponent<Enemy>();

                    // Check if enemy is dead
                    if (enemy.IsDead) return;

                    // Check weapon range
                    if (Vector3.Distance(transform.position, enemy.transform.position) <=
                        _playerEquipmentManager.weaponItem.weaponRange)
                    {

                        // Stop moving
                        GetComponent<PlayerMovement>().navMeshAgent.ResetPath();

                        state = State.Attacking;
                        enemy.TakeDamage(_playerAbilitySystem.CalculatePhysicalDamage());
                    }
                }
            }
            else
            {
                state = State.Normal;
            }
        }

        /// <summary>
        /// Called by sword swing animation event, enables the box collider on the weapon
        /// </summary>
        public void ActivateEquippedWeaponHitbox()
        {
            if (_playerEquipmentManager.hasWeaponEquipped)
            {
                var equippedWeapon = GameObject.FindGameObjectWithTag("EquippedWeapon");
                equippedWeapon.GetComponent<BoxCollider>().enabled = true;
            }
        }

        /// <summary>
        /// Called by sword swing animation event, disables the box collider on the weapon
        /// </summary>
        public void DeactivateEquippedWeaponHitbox()
        {
            if (_playerEquipmentManager.hasWeaponEquipped)
            {
                var equippedWeapon = GameObject.FindGameObjectWithTag("EquippedWeapon");
                equippedWeapon.GetComponent<BoxCollider>().enabled = false;
            }
        }

        public void StopAttackAnimation()
        {
            state = State.Normal;
        }

        public void SetAttackingState()
        {
            state = State.Attacking;
        }

        /// <summary>
        /// Inflict damage upon the <see cref="Player"/>, specifying the BASE amount of damage to do.
        /// It will then use this value and factor in the player's stats to get the final damage value.
        /// </summary>
        /// <param name="damageTaken">The base amount of damage to inflict (i.e the enemy damage)</param>
        public void TakeDamage(int damageTaken)
        {
            var damageModifier = (_playerAbilitySystem.strengthPhysicalDamageIncreaseAmount * _playerAbilitySystem.strength);
            CurrentHp -= damageTaken - damageModifier;

            if (CurrentHp <= 0)
                Die();
        }

        private void IncreaseHp(int amount)
        {
            // If adding this amount of HP takes us over the limit
            if (CurrentHp + amount >= maxHp)
                CurrentHp = maxHp;
            else
                CurrentHp += amount;
        }

        /// <summary>
        /// Kills the player when curHP = 0.
        /// </summary>
        private static void Die()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Level1");
        }
    }
}