using System;
using Enemies;
using Inventory_System;
using UnityEngine;
using UnityEngine.Internal;
using Item = Inventory_System.Item;

// ReSharper disable CompareOfFloatsByEqualityOperator

public class Player : MonoBehaviour
{
    
    private enum State
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

    [Header("Player Statistics")] 
    
    [Tooltip("The maximum amount of health the player has, this is affected by Strength")]
    public int maxHp;                   // our maximum health
    
    [Tooltip("The maximum amount of mana the player has, this is affected by Intelligence")]
    public int maxMana;

    [Range(0, 20)] [Tooltip("The percentage of mana restored every second")]
    public int manaRegenerationPercentage = 1;

    private float _manaRegenTimer;
    
    [Header("Movement")]
    [Tooltip("The speed at which the player moves around the world")]
    public                   float moveSpeed;
    
    private                  int   _curHp;
    private                  float   _currentMana;
    
    [Header("Player Attributes")]
    [Tooltip("Strength: for each level of strength: +strengthHpIncreaseAmount and +strengthPhysicalDamageIncreaseAmount to physical damage")]
    public int strength;

    [SerializeField] [Tooltip("The amount of HP to give the player per Strength level")]
    public int strengthHpIncreaseAmount = 3;

    [SerializeField] [Tooltip("The amount of Physical Damage to add towards attacks per Strength level")]
    public int strengthPhysicalDamageIncreaseAmount = 5;

    [Space]
    [Tooltip("Intelligence: for each level: +intelligenceManaIncreaseAmount Mana and +intelligenceElementalDamageIncreaseAmount to elemental damage")]
    public int intelligence;
    
    [SerializeField] [Tooltip("The amount of Mana to give the player per Intelligence level")]
    public int intelligenceManaIncreaseAmount = 10;

    [SerializeField] [Tooltip("The amount of Elemental Damage to add towards Skill attacks per Intelligence level")]
    public int intelligenceElementalDamageIncreaseAmount = 8;

    [Header("Experience")] public int   skillPoints;
    public                        int   curLevel;                // our current level
    public                        int   curXp;                   // our current experience points
    public                        int   xpToNextLevel;           // xp needed to level up
    public                        float levelXpModifier;       // modifier applied to 'xpToNextLevel' when we level up

    [Space]
    [SerializeField] private Transform projectileSpawnPoint;    // Transform component that projectiles will spawn from
    private                  float     _lastAttackTime;       // last time we attacked

    public Vector2 facingDirection;    // direction we're facing
    private State _state = State.Normal;
    
    // Movement vector
    private Vector2 _movement;

    private                  ParticleSystem _hitEffect;
    public                   Controls       Controls;
    public                   PlayerUi       ui;
    public                   Inventory      Inventory;
    [SerializeField] private UI_Inventory   uiInventory;

    public Animator animator; // The animation controller for the player movement etc.

    [Header("User Interface")]
    [SerializeField] private GameObject inventoryScreen; // Reference to the inventory UI
    private                  bool       _inventoryOpen; // is the player's inventory open?
    [SerializeField] private GameObject skillTreeScreen;
    private                  bool       _skillTreeOpen;
    
    private PlayerEquipmentManager _playerEquipmentManager;

    private static readonly  int    Horizontal     = Animator.StringToHash("Horizontal");
    private static readonly  int    Vertical       = Animator.StringToHash("Vertical");
    private static readonly  int    Speed          = Animator.StringToHash("Speed");
    private static readonly  int    LastDirectionX = Animator.StringToHash("LastDirectionX");
    private static readonly  int    LastDirectionY = Animator.StringToHash("LastDirectionY");
    [SerializeField] private float manaRegenerationTime;

    private void Awake ()
    {
        // get components
        ui = FindObjectOfType<PlayerUi>();
        Controls = new Controls();
        _hitEffect = gameObject.GetComponentInChildren<ParticleSystem>();
        _state = State.Normal;
        
        // Instantiate the inventory
        Inventory = new Inventory(UseItem);
        uiInventory.SetPlayer(this);
        uiInventory.SetInventory(Inventory);

        // Set current HP and mana values
        _curHp = maxHp / 2;
        _currentMana = maxMana / 2;
        CurrentHp = maxHp / 2;
        CurrentMana = maxMana / 2;
        _playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
        
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        var itemWorld = other.GetComponent<ItemWorld>();
        if (itemWorld != null)
        {
            // If we are touching an item
            Inventory.AddItem(itemWorld.GetItem());
            itemWorld.DestroySelf();
        }
    }

    private void OnEnable() => Controls.Player.Enable();

    private void OnDisable() => Controls.Player.Disable();

    private void Start ()
    {
        ui.UpdateHealthBar();
        ui.UpdateLevelText();
        ui.UpdateSkillPointsText();
        ui.UpdateXpBar();
        ui.UpdateManaBar();
        inventoryScreen.gameObject.SetActive(false);
    }

    private void Update ()
    {
        // Update the animation params
        animator.SetFloat(Horizontal, _movement.x);
        animator.SetFloat(Vertical, _movement.y);
        animator.SetFloat(Speed, _movement.sqrMagnitude);
        
        // If the inventory is open, pause the game
        if (_inventoryOpen || _skillTreeOpen)
        {
            // Pause the game
            Time.timeScale = 0f;
        }
        else // The player can only move if the inventory is NOT open
        {
            Time.timeScale = 1f;
            Move();
        }

        // Open and close the inventory screen
        if (Controls.Player.Inventory.triggered && !_skillTreeOpen)
        {
            _inventoryOpen = !_inventoryOpen;
            inventoryScreen.SetActive(_inventoryOpen);
        }

        if (Controls.Player.SkillTree.triggered && !_inventoryOpen)
        {
            _skillTreeOpen = !_skillTreeOpen;
            skillTreeScreen.SetActive(_skillTreeOpen);
        }
        
        // Mana Regen
        _manaRegenTimer += Time.deltaTime;
        if (_manaRegenTimer >= manaRegenerationTime)
        {
            _manaRegenTimer = 0;
            ManaRegeneration();
        }
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
                if (strength >= helmetItem.strengthRequirement && intelligence >= helmetItem.intelligenceRequirement)
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
    
    /// <summary>
    /// This function controls the movement of the player; alongside keeping track of which direction the player is both
    /// currently facing and the last facing direction. This is used to keep the players direction saved.
    /// </summary>
    private void Move ()
    {
        var movementInput = Controls.Player.Movement.ReadValue<Vector2>();
    
        _movement = new Vector2
        {
            x = movementInput.x,
            y = movementInput.y
        }.normalized;
    
        switch (_movement.x)
        {
            case -1:
                facingDirection = Vector2.left;
                animator.SetFloat(LastDirectionX, -1f);
                animator.SetFloat(LastDirectionY, 0f); // The opposite axis has to be reset
                break;
            case 1:
                facingDirection = Vector2.right;
                animator.SetFloat(LastDirectionX, 1f);
                animator.SetFloat(LastDirectionY, 0f); // The opposite axis has to be reset
                break;
        }
    
        switch (_movement.y)
        {
            case -1:
                facingDirection = Vector2.down;
                animator.SetFloat(LastDirectionY, -1f);
                animator.SetFloat(LastDirectionX, 0f); // The opposite axis has to be reset
                break;
            case 1:
                facingDirection = Vector2.up;
                animator.SetFloat(LastDirectionY, 1f);
                animator.SetFloat(LastDirectionX, 0f); // The opposite axis has to be reset
                break;
        }
        // Update the players position
        transform.Translate(_movement * (moveSpeed * Time.deltaTime));
    }

    /// <summary>
    /// Shoots a 2D raycast object, if this object hits an enemy it will take damage.
    /// </summary>
    private void Attack ()
    {
        // Check to see if the player has a weapon Equipped
        if (!_playerEquipmentManager.hasWeaponEquipped) return;
        if (!Input.GetMouseButtonDown(0)) return;
        if (!(Time.time - _lastAttackTime >= _playerEquipmentManager.weaponItem.attackRate)) return;
        
        _lastAttackTime = Time.time;
        var mousePosition = CameraController.GetMouseWorldPosition();
        var attackDirection = (mousePosition - transform.position).normalized;
        _state = State.Attacking;

        // shoot a raycast in the direction of where we're facing.
        var hit = Physics2D.Raycast(gameObject.transform.position, attackDirection, _playerEquipmentManager.weaponItem.weaponRange, 1 << 9);

        if (hit.collider == null || hit.collider.GetComponentInParent<Enemy>() is null ||
            hit.collider.GetComponentInParent<Enemy>().isDead) return;
        
        hit.collider.GetComponentInParent<Enemy>().TakeDamage(CalculatePhysicalDamage());
        // play hit effect
        _hitEffect.transform.position = hit.collider.transform.position;
        _hitEffect.Play();
    }

    private int CalculatePhysicalDamage()
    {
        var helmetItem = _playerEquipmentManager.head;
        var dmg        = _playerEquipmentManager.weaponItem.damage; // the weapons damage is used as a base
        dmg += (strengthPhysicalDamageIncreaseAmount * strength);
        return dmg;
    }

    // called when we gain xp
    public void AddXp (int xp)
    {
        curXp += xp;

        if (curXp >= xpToNextLevel)
        {
            LevelUp(xp);
        }
        
        ui.UpdateXpBar();
    }

    // called when our xp reaches the max for this level
    private void LevelUp (int xp)
    {
        curLevel++;
        skillPoints += 1;
        curXp = 0;
        curXp += xpToNextLevel - xp;
        xpToNextLevel = (int)(xpToNextLevel * levelXpModifier);
        
        ui.UpdateLevelText();
        ui.UpdateSkillPointsText();
        ui.UpdateXpBar();
    }

    // called when an enemy attacks us
    public void TakeDamage(int damageTaken)
    {
        CurrentHp -= damageTaken;

        if(CurrentHp <= 0)
            Die();
        ui.UpdateHealthBar();
    }

    public void RemoveMana(int amountOfManaToTake)
    {
        CurrentMana -= amountOfManaToTake;
        ui.UpdateManaBar();
    }

    private void IncreaseHp(int amount)
    {
        // If adding this amount of HP takes us over the limit
        if (CurrentHp + amount >= maxHp)
            CurrentHp = maxHp;
        else
            CurrentHp += amount;
        ui.UpdateHealthBar();
    }

    private void IncreaseMana(float amount)
    {
        if (CurrentMana + amount >= maxMana)
            CurrentMana = maxMana;
        else
            CurrentMana += amount;
        ui.UpdateManaBar();
    }

    private void ManaRegeneration()
    {
        IncreaseMana(((float)manaRegenerationPercentage / 100f) * (float)maxMana);
    }

    /// <summary>
    /// Kills the player when curHP = 0.
    /// </summary>
    private static void Die()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }
}