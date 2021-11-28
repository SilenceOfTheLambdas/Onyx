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
            internal set => _curHp = Mathf.Clamp(value, 0, maxHp);
        }
        
        public int GoldCoinAmount { get; private set; }

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
        private                 ParticleSystem         _hitEffect;
        private                 AbilitiesSystem        _playerAbilitySystem;
        private                 PlayerEquipmentManager _playerEquipmentManager;
        private                 float                  _lastAttackTime; // last time we attacked
        private                 bool                   _didThePlayerClickOnItemBeforeMoving;
        private static readonly int                    SwordSlash  = Animator.StringToHash("SwordSlash");
        private static readonly int                    IsAttacking = Animator.StringToHash("isAttacking");

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

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Pickup"))
            {
                var itemWorld = other.gameObject.GetComponentInParent<ItemWorld>();
                if (itemWorld)
                {
                    GoldCoinAmount += 1;
                    itemWorld.DestroySelf();
                }
            }
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
                
                // Check if item is a gold coin
                if (itemWorld.GetItem() as Coin)
                    return;
                Inventory.AddItem(itemWorld.GetItem());
                itemWorld.DestroySelf();
            }

            if (_didThePlayerClickOnItemBeforeMoving)
            {
                var itemWorld = other.gameObject.GetComponent<ItemWorld>();
                if (!itemWorld) return;
                
                // Check if item is a gold coin
                if (itemWorld.GetItem() as Coin)
                    return;
                
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
                if (!inventoryOpen)
                    uiInventory.hoverInterface.SetActive(false);
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
                if (_playerAbilitySystem.curLevel >= weaponItem.levelRequirement
                    && _playerAbilitySystem.strength >= weaponItem.strengthRequirement
                    && _playerAbilitySystem.intelligence >= weaponItem.intelligenceRequirement)
                {
                    if (!_playerEquipmentManager.hasWeaponEquipped)
                    {
                        _playerEquipmentManager.EquipWeapon(weaponItem);
                        Inventory.RemoveItem(item);
                    }
                    else
                    {
                        _playerEquipmentManager.UnEquip(_playerEquipmentManager.weaponItem);
                        _playerEquipmentManager.EquipWeapon(weaponItem); // Equip new weapon
                        Inventory.RemoveItem(item);
                    }
                }
            }

            if (item is ArmourItem armourItem)
            {
                // first check item requirements
                if (_playerAbilitySystem.strength >= armourItem.strengthRequirement
                    && _playerAbilitySystem.intelligence >= armourItem.intelligenceRequirement
                    && _playerAbilitySystem.curLevel >= armourItem.levelRequirement)
                {
                    switch (armourItem)
                    {
                        // ## Helmet Item ##
                        case HelmetItem when _playerEquipmentManager.head != null:
                        case ChestItem when _playerEquipmentManager.chest != null:
                        case BootItem when _playerEquipmentManager.boots != null:
                            return;
                        default:
                            _playerEquipmentManager.EquipArmour(armourItem);
                            Inventory.RemoveItem(item);
                            break;
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

                _playerAbilitySystem.AttemptToUseManaPotion(Inventory, manaPotion);
            }

            uiInventory.hoverInterface.SetActive(false);
        }

        private void PlayerMeleeAttack()
        {
            // Melee
            if (Mouse.current.rightButton.IsPressed() && !(GetComponent<Player>().inventoryOpen || GetComponent<Player>().skillTreeOpen)
                && state != State.Attacking)
            {
                // Check to see if the player has a weapon Equipped
                if (!_playerEquipmentManager.hasWeaponEquipped) return;

                // Check for attack rate timer
                if (!(Time.time - _lastAttackTime >= _playerEquipmentManager.weaponItem.attackRate)) return;
                _lastAttackTime = Time.time;
                
                // If everything is good, play the attack animation
                GetComponent<Animator>().SetTrigger(SwordSlash);
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

            CurrentHp -= (damageTaken - (damageModifier / 100) * damageTaken);

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