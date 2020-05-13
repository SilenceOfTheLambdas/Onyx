﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Stats")]
    public int curHp;                   // our current health
    public int maxHp;                   // our maximum health
    public float moveSpeed;             // how fast we move
    public int damage;                  // damage we deal
    public float interactRange;         // range at which we can interact
    public List<Item.Items> inventory = new List<Item.Items>();

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

    [Header("Sprites")]
    public Sprite downSprite;
    public Sprite upSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;

    // components
    private Rigidbody2D _rig;
    private SpriteRenderer _sr;
    private ParticleSystem _hitEffect;
    private Controls _controls = null;
    private PlayerUi _ui;

    void Awake ()
    {
        // get components
        _rig = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _ui = FindObjectOfType<PlayerUi>();
        _controls = new Controls();
        _hitEffect = gameObject.GetComponentInChildren<ParticleSystem>();
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
        Move();
        CheckInteract();
    }

    void Move ()
    {
        var movementInput = _controls.Player.Movement.ReadValue<Vector2>();

        var movement = new Vector2
        {
            x = movementInput.x,
            y = movementInput.y
        }.normalized;

        switch (movement.x)
        {
            case -1:
                _facingDirection = Vector2.left;
                break;
            case 1:
                _facingDirection = Vector2.right;
                break;
        }

        switch (movement.y)
        {
            case -1:
                _facingDirection = Vector2.down;
                break;
            case 1:
                _facingDirection = Vector2.up;
                break;
        }
        
        UpdateSpriteDirection();

        // Update the players position
        transform.Translate(movement * (moveSpeed * Time.deltaTime));
    }

    // change player sprite depending on where we're looking
    void UpdateSpriteDirection ()
    {
        if (_facingDirection == Vector2.up)
            _sr.sprite = upSprite;
        else if (_facingDirection == Vector2.down)
            _sr.sprite = downSprite;
        else if (_facingDirection == Vector2.left)
            _sr.sprite = leftSprite;
        else if (_facingDirection == Vector2.right)
            _sr.sprite = rightSprite;
    }

    // shoot a raycast and deal damage if we hit an enemy
    public void Attack ()
    {
        // can we attack?
        if (Time.time - _lastAttackTime >= attackRate)
        {
            _lastAttackTime = Time.time;

            // shoot a raycast in the direction of where we're facing.
            RaycastHit2D hit = Physics2D.Raycast(transform.position, _facingDirection, attackRange, 1 << 8);

            if(hit.collider != null)
            {
                Debug.Log("Play");
                hit.collider.GetComponent<Enemy>().TakeDamage(damage);
                // play hit effect
                _hitEffect.transform.position = hit.collider.transform.position;
                _hitEffect.Play();
            }   
        }
    }

    // manage interacting with objects
    public void CheckInteract ()
    {
        // shoot a raycast in the direction of where we're facing.
        // 9 == the intractable layer
        var hit = Physics2D.Raycast(transform.position, _facingDirection, interactRange, 1 << 9);

        if(hit.collider != null)
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            _ui.setInteractText(hit.collider.transform.position, interactable.interactDescription);
            // If and when the player hits the attack button, the interact() method is invoked
            if (_controls.Player.Attack.ReadValue<float>() == 1)
                interactable.Interact();
        }
        else
        {
            _ui.DisableInteractText();
        }
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

    // called when our hp reaches 0
    void Die()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

    // adds a new item to our inventory
    public void AddItemToInventory(Item.Items item)
    {
        inventory.Add(item);
        _ui.UpdateInventoryText();
    }
}