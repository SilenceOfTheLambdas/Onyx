using System;
using System.Collections.Generic;
using Skills;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Skills manager is used to handle the various equipped skills the player has. This script is also responsible
/// for activating those skills.
/// </summary>
public class SkillsManager : MonoBehaviour
{
    public                   List<Skill>       activeSkills = new List<Skill>();
    public                   List<SkillHotbar> skillsHotbar;
    [SerializeField] private Transform         projectileSpawnPoint;
    private                  Player            _player;
    private                  Controls          _playerControls;
    private                  Camera            _camera;

    private void Start()
    {
        _playerControls = GetComponent<Player>().Controls;
        _camera = Camera.main;
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        foreach (var skill in activeSkills)
        {
            switch (skill.skillType)
            {
                case SkillType.Projectile:
                    if (CheckForCorrectSkillInput(skill) && !skill.InCoolDown && CheckIfPlayerHasEnoughMana(skill))
                        ShootProjectile(skill);
                    
                    if (skill.InCoolDown)
                    {
                        skill.SkillTimer += Time.deltaTime;
                        var textTimer = (int)(skill.coolDownTime - skill.SkillTimer) + 1;
                        skill.CoolDownText.SetText(textTimer.ToString());
                        skill.CoolDownImage.fillAmount = 1 - (skill.SkillTimer / skill.coolDownTime);
                        if (skill.SkillTimer >= skill.coolDownTime)
                        {
                            skill.InCoolDown = false;
                            skill.SkillTimer = 0;
                            skill.CoolDownText.SetText("");
                            skill.CoolDownImage.fillAmount = 0;
                        }
                    }
                    break;
                case SkillType.Swing:
                    if (CheckForCorrectSkillInput(skill) && !skill.InCoolDown && CheckIfPlayerHasEnoughMana(skill))
                        Debug.Log("Action!");
                    
                    if (skill.InCoolDown)
                    {
                        skill.SkillTimer += Time.deltaTime;
                        var textTimer = (int)(skill.coolDownTime - skill.SkillTimer) + 1;
                        skill.CoolDownText.SetText(textTimer.ToString());
                        skill.CoolDownImage.fillAmount = 1 - (skill.SkillTimer / skill.coolDownTime);
                        if (skill.SkillTimer >= skill.coolDownTime)
                        {
                            skill.InCoolDown = false;
                            skill.SkillTimer = 0;
                            skill.CoolDownText.SetText("");
                            skill.CoolDownImage.fillAmount = 0;
                        }
                    }
                    break;
                case SkillType.Buff:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    
    public void UpgradeSkill(Skill skill, int amount = 1)
    {
        // First check to see if the player has the skill equipped
        if (activeSkills.Contains(skill))
        {
            var activeSkill = activeSkills.Find(active => active.name == skill.name);
            // Check to see if the skill is NOT already at max level
            if (activeSkill.skillLevel == activeSkill.maximumSkillLevel) return;
            activeSkill.skillLevel += amount;

        }
    }

    private bool CheckForCorrectSkillInput(Skill skill)
    {
        var input = _playerControls.Player.Skill1;
        
        if (activeSkills.FindIndex(s => s.skillName.Equals(skill.skillName)) == 0)
            input = _playerControls.Player.Skill1;
        if (activeSkills.FindIndex(s => s.skillName.Equals(skill.skillName)) == 1)
            input = _playerControls.Player.Skill2;
        if (activeSkills.FindIndex(s => s.skillName.Equals(skill.skillName)) == 2)
            input = _playerControls.Player.Skill3;
        if (activeSkills.FindIndex(s => s.skillName.Equals(skill.skillName)) == 4)
            input = _playerControls.Player.Skill4;
        if (activeSkills.FindIndex(s => s.skillName.Equals(skill.skillName)) == 5)
            input = _playerControls.Player.Skill5;

        return input.triggered;
    }

    private bool CheckIfPlayerHasEnoughMana(Skill skill) => GetComponent<Player>().CurrentMana - skill.manaCost >= 0;

    private void ShootProjectile(Skill skill)
    {
        // Computer where the mouse is
        var mousePos = new Vector2
        {
            x = Input.mousePosition.x,
            y = Input.mousePosition.y
        };
        var position = transform.position;
        var direction = -(new Vector3(position.x, position.y) - _camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y))).normalized; // negate it
        
        skill.InCoolDown = true;
        skill.HasBeenUsed = true;
        var projectile = Instantiate(skill.skillEffect, projectileSpawnPoint.position, Quaternion.Euler(new Vector2(0, 0)));
        _player.RemoveMana(skill.manaCost);
        projectile.GetComponent<Rigidbody2D>().AddForce(direction * projectile.GetComponent<SkillProjectile>().projectileSpeed, ForceMode2D.Impulse);
        projectile.GetComponent<SkillProjectile>().Skill = skill;
        
    }
}