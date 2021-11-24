using System;
using System.Collections.Generic;
using Skills;
using Player;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Skills manager is used to handle the various equipped skills the player has. This script is also responsible
/// for activating those skills.
/// </summary>
public class SkillsManager : MonoBehaviour
{
    public List<Skill> activeSkills = new List<Skill>();
    public List<SkillHotbar> skillsHotbar;
    [SerializeField] private Transform skillSpawnPoint;
    [SerializeField] private PlayerUi playerUi;
    private Player.Player _player;
    private AbilitiesSystem _playerAbilitiesSystem;
    private Controls _playerControls;
    private Camera _camera;

    private void Start()
    {
        _playerControls = GetComponent<Player.Player>().Controls;
        _camera = Camera.main;
        _player = GetComponent<Player.Player>();
        _playerAbilitiesSystem = _player.GetComponent<AbilitiesSystem>();
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
                case SkillType.Beam:
                    if (CheckForCorrectSkillInput(skill) && !skill.InCoolDown && CheckIfPlayerHasEnoughMana(skill))
                        CastBeam(skill);

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

    private bool CheckIfPlayerHasEnoughMana(Skill skill)
    {
        if (GetComponent<AbilitiesSystem>().CurrentMana - skill.manaCost >= 0)
        {
            return true;
        }
        else
        {
            playerUi.manaBarAnimator.Play("ManaWiggleAnimation");
            return false;
        }
    }

    private void ShootProjectile(Skill skill)
    {
        skill.InCoolDown = true;
        skill.HasBeenUsed = true;
        var projectile = Instantiate(skill.skillEffect, skillSpawnPoint.position, Quaternion.Euler(new Vector2(0, 0)));
        var reductionAmount = _playerAbilitiesSystem.reducedManaCostOfSkillsAmount / 100 * skill.manaCost; // TODO: Might need fixed?
        _playerAbilitiesSystem.RemoveMana(skill.manaCost - reductionAmount);

        var mRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(mRay, out var mRaycastHit, Mathf.Infinity))
        {
            var direction = -(skillSpawnPoint.transform.position - mRaycastHit.point).normalized;
            projectile.GetComponent<Rigidbody>().AddForce(direction * projectile.GetComponent<SkillProjectile>().projectileSpeed, ForceMode.Impulse);
        }

        projectile.GetComponent<SkillProjectile>().Skill = skill;

    }

    private void CastBeam(Skill skill)
    {
        var beam = Instantiate(original: skill.skillEffect, parent: skillSpawnPoint);
        var beamManager = beam.GetComponent<BeamManager>();

        beamManager.SetOriginalRotation(skillSpawnPoint.rotation);
        beamManager.Skill = skill;
        beamManager.BeamSpawnPoint = skillSpawnPoint;

        skill.InCoolDown = true;
        skill.HasBeenUsed = true;
        _playerAbilitiesSystem.RemoveMana(skill.manaCost);
    }
}