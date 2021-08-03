using System;
using Enemies;
using Inventory_System;
using UnityEngine;
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
        set => _curHp = Mathf.Clamp(value, 0, maxHp);
    }

    public int CurrentMana
    {
        get => _currentMana;
        set => _currentMana = Mathf.Clamp(value, 0, maxMana);
    }

    #endregion

    [Header("Stats")] public int   maxHp;                   // our maximum health
    public                   float moveSpeed;             // how fast we move
    public                   int   damage;                  // damage we deal
    public                   float interactRange;         // range at which we can interact
    public                   int   maxMana;
    private                  int   _curHp;                   // our current health
    private                  int   _currentMana;

    [Header("Experience")]
    public int curLevel;                // our current level
    public int curXp;                   // our current experience points
    public int xpToNextLevel;           // xp needed to level up
    public float levelXpModifier;       // modifier applied to 'xpToNextLevel' when we level up

    [Header("Combat")]
    public float attackRange;           // range we can deal damage to an enemy
    public float attackRate;            // minimum time between attacks
    private float _lastAttackTime;       // last time we attacked

    private Vector2 _facingDirection;    // direction we're facing
    private State _state = State.Normal;
    
    // Movement vector
    private Vector2 _movement;

    private                  ParticleSystem _hitEffect;
    private                  Controls       _controls;
    private                  PlayerUi       _ui;
    private                  Inventory      _inventory;
    [SerializeField] private UI_Inventory   uiInventory;

    public Animator        animator; // The animation controller for the player movement etc.

    private                  bool       _inventoryOpen; // is the player's inventory open?
    [SerializeField] private GameObject inventoryScreen; // Reference to the inventory UI
    
    
    private static readonly int        Horizontal     = Animator.StringToHash("Horizontal");
    private static readonly int        Vertical       = Animator.StringToHash("Vertical");
    private static readonly int        Speed          = Animator.StringToHash("Speed");
    private static readonly int        LastDirectionX = Animator.StringToHash("LastDirectionX");
    private static readonly int        LastDirectionY = Animator.StringToHash("LastDirectionY");

    private void Awake ()
    {
        // get components
        _ui = FindObjectOfType<PlayerUi>();
        _controls = new Controls();
        _hitEffect = gameObject.GetComponentInChildren<ParticleSystem>();
        _state = State.Normal;
        
        // Instantiate the inventory
        _inventory = new Inventory(UseItem);
        uiInventory.SetPlayer(this);
        uiInventory.SetInventory(_inventory);
        
        // Set current HP and mana values
        _curHp = maxHp / 2;
        _currentMana = maxMana / 2;
        CurrentHp = maxHp / 2;
        CurrentMana = maxMana / 2;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        var itemWorld = other.GetComponent<ItemWorld>();
        if (itemWorld != null)
        {
            // If we are touching an item
            _inventory.AddItem(itemWorld.GetItem());
            itemWorld.DestroySelf();
        }
    }

    private void OnEnable() => _controls.Player.Enable();

    private void OnDisable() => _controls.Player.Disable();

    private void Start ()
    {
        _ui.UpdateHealthBar();
        _ui.UpdateLevelText();
        _ui.UpdateXpBar();
        inventoryScreen.gameObject.SetActive(false);
    }

    private void Update ()
    {
        // Update the animation params
        animator.SetFloat(Horizontal, _movement.x);
        animator.SetFloat(Vertical, _movement.y);
        animator.SetFloat(Speed, _movement.sqrMagnitude);
        
        // If the inventory is open, pause the game
        if (_inventoryOpen)
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
        if (_controls.Player.Inventory.triggered)
        {
            _inventoryOpen = !_inventoryOpen;
            inventoryScreen.SetActive(_inventoryOpen);
        }
        
        // Attacking
        Attack();
    }

    private void UseItem(Item item)
    {
        switch (item.itemType)
        {
            case Item.ItemType.Sword:
                break;
            case Item.ItemType.SpellBook:
                break;
            case Item.ItemType.HealthPotion:
                if (CurrentHp != maxHp)
                {
                    IncreaseHp(ItemDatabase.Instance.healthPotionRestoreAmount);
                    _inventory.RemoveItem(new Item { itemType = Item.ItemType.HealthPotion, amount = 1});
                }
                break;
            case Item.ItemType.ManaPotion:
                if (CurrentMana != maxMana)
                {
                    IncreaseMana(ItemDatabase.Instance.manaPotionRestoreAmount);
                    _inventory.RemoveItem(new Item { itemType = Item.ItemType.ManaPotion, amount = 1});
                }
                break;
            case Item.ItemType.Coin:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    /// <summary>
    /// This function controls the movement of the player; alongside keeping track of which direction the player is both
    /// currently facing and the last facing direction. This is used to keep the players direction saved.
    /// </summary>
    private void Move ()
    {
        var movementInput = _controls.Player.Movement.ReadValue<Vector2>();
    
        _movement = new Vector2
        {
            x = movementInput.x,
            y = movementInput.y
        }.normalized;
    
        switch (_movement.x)
        {
            case -1:
                _facingDirection = Vector2.left;
                animator.SetFloat(LastDirectionX, -1f);
                animator.SetFloat(LastDirectionY, 0f); // The opposite axis has to be reset
                break;
            case 1:
                _facingDirection = Vector2.right;
                animator.SetFloat(LastDirectionX, 1f);
                animator.SetFloat(LastDirectionY, 0f); // The opposite axis has to be reset
                break;
        }
    
        switch (_movement.y)
        {
            case -1:
                _facingDirection = Vector2.down;
                animator.SetFloat(LastDirectionY, -1f);
                animator.SetFloat(LastDirectionX, 0f); // The opposite axis has to be reset
                break;
            case 1:
                _facingDirection = Vector2.up;
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
        // Attack function
        if (!Input.GetMouseButtonDown(0)) return;
        if (!(Time.time - _lastAttackTime >= attackRate)) return;
        
        _lastAttackTime = Time.time;
        var mousePosition = CameraController.GetMouseWorldPosition();
        var attackDirection = (mousePosition - transform.position).normalized;
        _state = State.Attacking;

        // shoot a raycast in the direction of where we're facing.
        var hit = Physics2D.Raycast(gameObject.transform.position, attackDirection, attackRange, 1 << 9);

        if (hit.collider == null || hit.collider.GetComponentInParent<Enemy>() is null ||
            hit.collider.GetComponentInParent<Enemy>().isDead) return;
        
        hit.collider.GetComponentInParent<Enemy>().TakeDamage(damage);
        // play hit effect
        _hitEffect.transform.position = hit.collider.transform.position;
        _hitEffect.Play();
    }

    // called when we gain xp
    public void AddXp (int xp)
    {
        curXp += xp;

        if(curXp >= xpToNextLevel)
            LevelUp();
        
        _ui.UpdateXpBar();
        
    }

    // called when our xp reaches the max for this level
    private void LevelUp ()
    {
        curXp = 0;
        curLevel++;

        xpToNextLevel = (int)(xpToNextLevel * levelXpModifier);
        
        _ui.UpdateLevelText();
        _ui.UpdateXpBar();
    }

    // called when an enemy attacks us
    public void TakeDamage(int damageTaken)
    {
        CurrentHp -= damageTaken;

        if(CurrentHp <= 0)
            Die();
        _ui.UpdateHealthBar();
    }

    private void IncreaseHp(int amount)
    {
        // If adding this amount of HP takes us over the limit
        if (CurrentHp + amount >= maxHp)
            CurrentHp = maxHp;
        else
            CurrentHp += amount;
        _ui.UpdateHealthBar();
    }

    private void IncreaseMana(int amount)
    {
        if (CurrentMana + amount >= maxMana)
            CurrentMana = maxMana;
        else
            CurrentMana += amount;
    }

    /// <summary>
    /// Kills the player when curHP = 0.
    /// </summary>
    private static void Die()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }
}