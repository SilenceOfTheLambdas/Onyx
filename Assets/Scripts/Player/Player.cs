using System;
using Enemies;
using UnityEngine;
using Inventory_System;
using UI;
using UnityEngine.InputSystem;
using SuperuserUtils;
using UnityEngine.EventSystems;
using Item = Inventory_System.Item;

namespace Player
{
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

        public float CurrentMana
        {
            get => _currentMana;
            set => _currentMana = Mathf.Clamp(value, 0f, maxMana);
        }

        #endregion

        #region Fields

        [Header("Player Statistics")]
        [Tooltip("The maximum amount of health the player has, this is affected by Strength")]
        public int maxHp; // our maximum health

        [Tooltip("The maximum amount of mana the player has, this is affected by Intelligence")]
        public int maxMana;

        [Range(0, 20)] [Tooltip("The percentage of mana restored every second")]
        public int manaRegenerationPercentage = 1;

        private float _manaRegenTimer;

        private int   _curHp;
        private float _currentMana;

        [Header("Player Attributes")]
        [Tooltip(
            "Strength: for each level of strength: +strengthHpIncreaseAmount and +strengthPhysicalDamageIncreaseAmount to physical damage")]
        public int strength;

        [SerializeField] [Tooltip("The amount of HP to give the player per Strength level")]
        public int strengthHpIncreaseAmount = 3;

        [SerializeField] [Tooltip("The amount of Physical Damage to add towards attacks per Strength level")]
        public int strengthPhysicalDamageIncreaseAmount = 5;

        [Space]
        [Tooltip(
            "Intelligence: for each level: +intelligenceManaIncreaseAmount Mana and +intelligenceElementalDamageIncreaseAmount to elemental damage")]
        public int intelligence;

        [SerializeField] [Tooltip("The amount of Mana to give the player per Intelligence level")]
        public int intelligenceManaIncreaseAmount = 10;

        [SerializeField] [Tooltip("The amount of Elemental Damage to add towards Skill attacks per Intelligence level")]
        public int intelligenceElementalDamageIncreaseAmount = 8;

        [Header("Experience")] public int   skillPoints;
        public                        int   curLevel; // our current level
        public                        int   curXp; // our current experience points
        public                        int   xpToNextLevel; // xp needed to level up
        public                        float levelXpModifier; // modifier applied to 'xpToNextLevel' when we level up

        private float _lastAttackTime; // last time we attacked

        public State state = State.Normal;
        private ParticleSystem _hitEffect;
        public                   Controls       Controls;
        public                   PlayerUi       ui;
        public                   Inventory      Inventory;
        [SerializeField] private UI_Inventory   uiInventory;
        [SerializeField] private LayerMask      enemyHitableLayerMask;

        [Header("User Interface")] [SerializeField]
        private GameObject inventoryScreen; // Reference to the inventory UI

        public                   bool       inventoryOpen; // is the player's inventory open?
        [SerializeField] private GameObject skillTreeScreen;
        public                   bool       skillTreeOpen;

        private PlayerEquipmentManager _playerEquipmentManager;

        [SerializeField] private float manaRegenerationTime;
        private static readonly  int   SwordSlash = Animator.StringToHash("SwordSlash");

        private bool _didThePlayerClickOnItemBeforeMoving;

        #endregion

        private void Awake()
        {
            // get components
            ui = FindObjectOfType<PlayerUi>();
            _hitEffect = gameObject.GetComponentInChildren<ParticleSystem>();
            state = State.Normal;
            Controls = new Controls();

            // Instantiate the inventory
            Inventory = new Inventory(UseItem);
            uiInventory.SetPlayer(this);
            uiInventory.SetInventory(Inventory);

            // Set current HP and mana values
            _curHp = maxHp;
            _currentMana = maxMana;
            CurrentHp = maxHp;
            CurrentMana = maxMana;
            _playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
        }
        
        private void OnEnable() => Controls.Player.Enable();

        private void OnDisable() => Controls.Player.Disable();

        #region Object Pickup
        private void OnTriggerStay(Collider other)
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
                else
                {
                    var itemWorld = other.gameObject.GetComponent<ItemWorld>();
                    if (!itemWorld) return;

                    Inventory.AddItem(itemWorld.GetItem());
                    itemWorld.DestroySelf();
                }
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
                // Pause the game
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

            // Mana Regen
            _manaRegenTimer += Time.deltaTime;
            if (_manaRegenTimer >= manaRegenerationTime)
            {
                _manaRegenTimer = 0;
                ManaRegeneration();
            }

            // Detect if player clicked on an item in the world
            if (SuperuserUtils.SuperuserUtils.Instance.IsTheMouseHoveringOverGameObject(LayerMask.GetMask("Pickup"), out var _)
                && Mouse.current.leftButton.isPressed)
            {
                Debug.Log("Mouse is hovering over item");
                _didThePlayerClickOnItemBeforeMoving = true;
            }
            if (!SuperuserUtils.SuperuserUtils.Instance.IsTheMouseHoveringOverGameObject(LayerMask.GetMask("Pickup"), out var _)
                && Mouse.current.leftButton.isPressed)
            {
                Debug.Log("Mouse is NOT hovering over item");
                _didThePlayerClickOnItemBeforeMoving = false;
            }
            //////////////////////////////////////////////////

            // Attacking
            Attack();
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
                    if (strength >= helmetItem.strengthRequirement &&
                        intelligence >= helmetItem.intelligenceRequirement)
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
                    if (strength >= chestItem.strengthRequirement && intelligence >= chestItem.intelligenceRequirement)
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
                if (CurrentMana < maxMana)
                {
                    IncreaseMana(manaPotion.restoreAmount);
                    Inventory.RemoveItem(item);
                }
            }

            uiInventory.hoverInterface.SetActive(false);
        }
        
        private void Attack()
        {
            // Check to see if the player has a weapon Equipped
            if (!_playerEquipmentManager.hasWeaponEquipped) return;

            // Check if the player is hovering over an enemy
            if (!SuperuserUtils.SuperuserUtils.Instance.IsTheMouseHoveringOverGameObject(enemyHitableLayerMask, out var _)) return;

            // Check for attack rate timer
            if (!(Time.time - _lastAttackTime >= _playerEquipmentManager.weaponItem.attackRate)) return;
            _lastAttackTime = Time.time;

            // Melee
            if (Mouse.current.rightButton.IsPressed() && !(GetComponent<Player>().inventoryOpen || GetComponent<Player>().skillTreeOpen))
            {
                // First we play the sword slash animation
                GetComponent<Animator>().SetBool("isAttack", true);
                /*GetComponent<Animator>().SetTrigger(SwordSlash);*/
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
                        enemy.TakeDamage(CalculatePhysicalDamage());
                    }
                }
            }
            else
            {
                state = State.Normal;
            }
        }

        public void StopAttackAnimation()
        {
            GetComponent<Animator>().SetBool("isAttack", false);
        }

        private int CalculatePhysicalDamage()
        {
            var helmetItem = _playerEquipmentManager.head;
            var dmg        = _playerEquipmentManager.weaponItem.damage; // the weapons damage is used as a base
            dmg += (strengthPhysicalDamageIncreaseAmount * strength);
            return dmg;
        }

        // called when we gain xp
        public void AddXp(int xp)
        {
            curXp += xp;

            if (curXp >= xpToNextLevel)
            {
                LevelUp(xp);
            }
        }

        // called when our xp reaches the max for this level
        private void LevelUp(int xp)
        {
            curLevel++;
            skillPoints += 1;
            curXp = 0;
            if (xp > xpToNextLevel)
                curXp += xp - xpToNextLevel;
            if (xp < xpToNextLevel)
                curXp += xpToNextLevel - xp;
            xpToNextLevel = (int)(xpToNextLevel * levelXpModifier);

            ui.UpdateLevelText();
            ui.UpdateSkillPointsText();
        }

        // called when an enemy attacks us
        public void TakeDamage(int damageTaken)
        {
            CurrentHp -= damageTaken;

            if (CurrentHp <= 0)
                Die();
        }

        public void RemoveMana(int amountOfManaToTake)
        {
            var startMana = _currentMana;
            CurrentMana -= amountOfManaToTake;
        }

        private void IncreaseHp(int amount)
        {
            // If adding this amount of HP takes us over the limit
            if (CurrentHp + amount >= maxHp)
                CurrentHp = maxHp;
            else
                CurrentHp += amount;
        }

        private void IncreaseMana(float amount)
        {
            if (CurrentMana + amount >= maxMana)
                CurrentMana = maxMana;
            else
                CurrentMana += amount;
        }

        private void ManaRegeneration()
        {
            IncreaseMana((manaRegenerationPercentage / 100f) * maxMana);
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