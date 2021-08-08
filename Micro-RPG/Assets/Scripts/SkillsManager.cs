using System;
using System.Collections.Generic;
using Skills;
using UnityEngine;

/// <summary>
/// Skills manager is used to handle the various equipped skills the player has. This script is also responsible
/// for activating those skills.
/// </summary>
public class SkillsManager : MonoBehaviour
{
    public                   List<Skill> activeSkills = new List<Skill>();
    [SerializeField] private Transform   projectileSpawnPoint;
    private                  Controls    _playerControls;

    private void Start()
    {
        _playerControls = GetComponent<Player>().Controls;
    }

    private void Update()
    {
        foreach (var skill in activeSkills)
        {
            switch (skill.skillType)
            {
                case SkillType.Projectile:
                    ShootProjectile(skill);
                    break;
                case SkillType.Swing:
                    break;
                case SkillType.Buff:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }


    private void ShootProjectile(Skill skill)
    {
        if (_playerControls.Player.Skill1.triggered && !skill.InCoolDown)
        {
            skill.InCoolDown = true;
            skill.HasBeenUsed = true;
            Debug.Log(transform.forward);
            var obj = Instantiate(skill.skillEffect, projectileSpawnPoint.position, Quaternion.Euler(new Vector2(0, 0)));
            obj.GetComponent<Rigidbody2D>().velocity = GetComponent<Player>().facingDirection * 4f;
        }

        if (skill.InCoolDown)
        {
            skill.SkillTimer += Time.deltaTime;
            if (skill.SkillTimer >= skill.coolDownTime)
            {
                skill.InCoolDown = false;
                skill.SkillTimer = 0;
            }
        }
    }
}