using System;
using Inventory_System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Item = Inventory_System.Item;

// ReSharper disable CompareOfFloatsByEqualityOperator

public class Player : MonoBehaviour
{
    
    private enum State
    {
        Normal,
        Attacking
    }

    [Header("Stats")]
    public int curHp;                   // our current health
    public int maxHp;                   // our maximum health
    public float moveSpeed;             // how fast we move
    public int damage;                  // damage we deal
    public float interactRange;         // range at which we can interact

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

    [Header("Components")]
    // components
    private Rigidbody2D _rig;
    private                  SpriteRenderer _sr;
    private                  ParticleSystem _hitEffect;
    private                  Controls       _controls;
    private                  PlayerUi       _ui;
    private                  Inventory      _inventory;
    [SerializeField] private UI_Inventory   uiInventory;

    public Animator animator; // The animation controller for the player movement etc.
    public CameraController cameraController;
    public TextMeshProUGUI interactText;

    private                 bool       _inventoryOpen; // is the player's inventory open?
    private                 bool       _enemyInventoryOpen; // Is the enemies inventory open?
    private static readonly int        Horizontal     = Animator.StringToHash("Horizontal");
    private static readonly int        Vertical       = Animator.StringToHash("Vertical");
    private static readonly int        Speed          = Animator.StringToHash("Speed");
    private static readonly int        LastDirectionX = Animator.StringToHash("LastDirectionX");
    private static readonly int        LastDirectionY = Animator.StringToHash("LastDirectionY");
    
    void Awake ()
    {
        // get components
        _rig = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _ui = FindObjectOfType<PlayerUi>();
        _controls = new Controls();
        _hitEffect = gameObject.GetComponentInChildren<ParticleSystem>();
        _state = State.Normal;
        
        // Instantiate the inventory
        _inventory = new Inventory(UseItem);
        uiInventory.SetPlayer(this);
        uiInventory.SetInventory(_inventory);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        ItemWorld itemWorld = other.GetComponent<ItemWorld>();
        if (itemWorld != null)
        {
            // If we are touching an item
            _inventory.AddItem(itemWorld.GetItem());
            itemWorld.DestroySelf();
        }
    }

    private void OnEnable() => _controls.Player.Enable();

    private void OnDisable() => _controls.Player.Disable();

    void Start ()
    {
        _ui.UpdateHealthBar();
        _ui.UpdateLevelText();
        _ui.UpdateXpBar();
    }

    void Update ()
    {
        // Update the animation params
        animator.SetFloat(Horizontal, _movement.x);
        animator.SetFloat(Vertical, _movement.y);
        animator.SetFloat(Speed, _movement.sqrMagnitude);
        Move();

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
                Debug.Log("Used Health Potion");
                _inventory.RemoveItem(new Item { itemType = Item.ItemType.HealthPotion, amount = 1});
                break;
            case Item.ItemType.ManaPotion:
                Debug.Log("Used Mana Potion");
                _inventory.RemoveItem(new Item { itemType = Item.ItemType.ManaPotion, amount = 1});
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
    void Move ()
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

        if (hit.collider == null || hit.collider.GetComponent<Enemy>() is null ||
            hit.collider.GetComponent<Enemy>().isDead) return;
        
        hit.collider.GetComponent<Enemy>().TakeDamage(damage);
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
    void LevelUp ()
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
        curHp -= damageTaken;

        if(curHp <= 0)
            Die();
        _ui.UpdateHealthBar();
    }

    /// <summary>
    /// Kills the player when curHP = 0.
    /// </summary>
    void Die()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

    private void OnApplicationQuit()
    {
        
    }
}