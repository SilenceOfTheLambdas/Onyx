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
    private                  Camera      _camera;

    private void Start()
    {
        _playerControls = GetComponent<Player>().Controls;
        _camera = Camera.main;
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
            // Computer where the mouse is
            var mousePos     = new Vector2
            {
                x = Input.mousePosition.x,
                y = Input.mousePosition.y
            };
            var position = transform.position;
            var direction = -(new Vector3(position.x, position.y) - _camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y))).normalized; // negate it
            
            skill.InCoolDown = true;
            skill.HasBeenUsed = true;
            var projectile = Instantiate(skill.skillEffect, projectileSpawnPoint.position, Quaternion.Euler(new Vector2(0, 0)));
            projectile.GetComponent<Rigidbody2D>().AddForce(direction * projectile.GetComponent<SkillProjectile>().projectileSpeed, ForceMode2D.Impulse);
            projectile.GetComponent<SkillProjectile>().Skill = skill;
        }

        if (skill.InCoolDown)
        {
            skill.SkillTimer += Time.deltaTime;
            int textTimer = (int)(skill.coolDownTime - skill.SkillTimer) + 1;
            skill.coolDownText.SetText(textTimer.ToString());
            skill.coolDownImage.fillAmount = 1 - (skill.SkillTimer / skill.coolDownTime);
            if (skill.SkillTimer >= skill.coolDownTime)
            {
                skill.InCoolDown = false;
                skill.SkillTimer = 0;
                skill.coolDownText.SetText("");
                skill.coolDownImage.fillAmount = 0;
            }
        }
    }
}