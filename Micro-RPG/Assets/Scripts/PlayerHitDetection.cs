using System;
using System.Collections;
using System.Collections.Generic;
using Skills;
using UnityEngine;

public class PlayerHitDetection : MonoBehaviour
{
    private Player _player;

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // If we are hit by an enemy projectile skill
        if (other.CompareTag("Enemy") && other.GetComponent<SkillProjectile>())
        {
            var damageModifier = (_player.intelligenceElementalDamageIncreaseAmount * _player.intelligence);
            _player.TakeDamage(other.GetComponent<SkillProjectile>().Skill.amountOfDamage - damageModifier);
        }
    }
}
