using System;
using Scriptable_Objects.Inventory.Scripts;
using Scriptable_Objects.Items.Scripts;
using TMPro;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

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
    private State _state;
    
    // Movement vector
    private Vector2 _movement;

    // components
    private Rigidbody2D _rig;
    private SpriteRenderer _sr;
    private ParticleSystem _hitEffect;
    private Controls _controls;
    private PlayerUi _ui;
    
    [Header("Components")]
    public Animator animator; // The animation controller for the player movement etc.
    [FormerlySerializedAs("_cameraController")] public CameraController cameraController;
    public TextMeshProUGUI interactText;

    public MouseItem mouseItem = new MouseItem();
    public InventoryObject inventory;
    public GameObject inventoryCanvas; // The UI for the inventory
    public GameObject enemyInventoryCanvas; // The UI for the enemy inventory
    private bool _inventoryOpen; // is the player's inventory open?
    private bool _enemyInventoryOpen; // Is the enemies inventory open?
    private static readonly int Horizontal = Animator.StringToHash("Horizontal");
    private static readonly int Vertical = Animator.StringToHash("Vertical");
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int LastDirectionX = Animator.StringToHash("LastDirectionX");
    private static readonly int LastDirectionY = Animator.StringToHash("LastDirectionY");
    
    void Awake ()
    {
        // get components
        _rig = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _ui = FindObjectOfType<PlayerUi>();
        _controls = new Controls();
        _hitEffect = gameObject.GetComponentInChildren<ParticleSystem>();
        _state = State.Normal;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        var item = other.GetComponent<GroundItem>();
        if (item)
        {
            Item _item = new Item(item.item);
            inventory.AddItem(_item, 1);
            Destroy(other.gameObject);
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
        
        if (_controls.Player.Inventory.triggered)
        {
            // Check for input to then open the inventory GUI
            _inventoryOpen = inventoryCanvas.activeSelf; // First check to see of the inventory has been closed
            if (!_inventoryOpen)
            {
                inventoryCanvas.SetActive(true);
                _inventoryOpen = true;
            }
            else
            {
                inventoryCanvas.SetActive(false);
                _inventoryOpen = false;
            }
        }
        
        // Only move when inventory screen is NOT open
        if (!inventoryCanvas.activeSelf) Move();
        CheckInteract();
        
        // Attacking
        Attack();
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

    /// <summary>
    /// Manages the interaction of objects in the scene.
    /// </summary>
    public void CheckInteract ()
    {
        
        if (cameraController.GetGroundItemOnCursor(interactRange) != null)
        {
            _ui.SetInteractText(cameraController.GetGroundItemOnCursor(interactRange).transform.position,
                cameraController.GetGroundItemOnCursor(interactRange).item.name);
        } else if (cameraController.GetGroundItemOnCursor(interactRange) == null)
            _ui.DisableInteractText();
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
        inventory.container.Items = new InventorySlot[28];
    }
}