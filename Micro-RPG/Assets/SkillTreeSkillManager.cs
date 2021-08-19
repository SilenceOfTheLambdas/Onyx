using System.Collections.Generic;
using System.Linq;
using Skills;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class SkillTreeSkillManager : MonoBehaviour
{
    public                   Skill           skill;
    [SerializeField] private SkillsManager   skillsManager;
    [SerializeField] private PlayerUi        playerUi;
    private                  Image           _overlay;
    private                  Image           _border;
    private                  Image           _skillSpriteHolder;
    private                  TextMeshProUGUI _levelText;
    private                  Button_UI       _buttonUI;
    private                  Player          _player;
    private                  Skill           _activatedSkill;
    private                  bool            _skillIsActive;

    private void Awake()
    {
        _overlay = transform.Find("overlay").GetComponent<Image>();
        _border = transform.Find("border").GetComponent<Image>();
        _skillSpriteHolder = transform.Find("skillSprite").GetComponent<Image>();
        _levelText = transform.Find("levelText").GetComponent<TextMeshProUGUI>();
        _buttonUI = GetComponent<Button_UI>();
        _player = skillsManager.GetComponent<Player>();
    }

    private void Start()
    {
        _skillSpriteHolder.sprite = skill.skillSprite;
        _levelText.SetText($"{skill.skillLevel}");
    }

    private void Update()
    {
        // We do not have the skill active
        if (skillsManager.activeSkills.Find(s => s.skillName.Equals(skill.skillName)) == null)
        {
            _overlay.color = new Color(1f, 1f, 1f, 0f);
            _border.color = new Color(0.08f, 0.08f, 0.08f);
        }
        else
        {
            _overlay.color = new Color(1f, 1f, 1f, 0f);
            _border.color = new Color(0.31f, 1f, 0.35f);
        }
        
        if (!CheckSkillRequirements())
        {
            _overlay.color = new Color(0.17f, 0.17f, 0.17f, 0.55f);
            _border.color = new Color(0.17f, 0.17f, 0.17f, 0.55f);
        }
        
        _buttonUI.ClickFunc = () =>
        {
            // If the skill is not unlocked, and we have the correct requirements
            if (skillsManager.activeSkills.Find(s => s.skillName.Equals(skill.skillName)) == null && CheckSkillRequirements())
            {
                // Unlock the skill
                var newSkill = Object.Instantiate(skill);
                skillsManager.activeSkills.Add(newSkill);
                skillsManager.skillsHotbar.First(slot => slot.Skill == null).AssignSkillToHotbar(newSkill);
                _activatedSkill = skillsManager.activeSkills.Find(s => s.skillName.Equals(skill.skillName));

                _player.skillPoints -= _activatedSkill.requiredNumberOfSkillPointsToUnlockOrUpgrade;
                playerUi.UpdateSkillPointsText();
            }
            
            // If we already have the skill unlocked, and we have the required skills to upgrade
            if (skillsManager.activeSkills.Find(s => s.skillName.Equals(skill.skillName)) != null && CheckSkillUpgradeRequirements())
            {
                // Level the skill up
                skillsManager.UpgradeSkill(_activatedSkill);
                // Update the tree UI
                _levelText.SetText($"{_activatedSkill.skillLevel}");
                _player.skillPoints -= _activatedSkill.requiredNumberOfSkillPointsToUnlockOrUpgrade;
                playerUi.UpdateSkillPointsText();
            }
        };
    }

    private bool CheckSkillRequirements()
    {
        return CheckIfPlayerHasRequiredNumberOfSkillPoints() &&
               _player.intelligence >= skill.requiredIntelligenceLevel &&
               (RequiredSkillsAreActive(skillsManager.activeSkills, skill.requiredActiveSkills)
                || skill.requiredActiveSkills.Count == 0);
    }

    private bool CheckIfPlayerHasRequiredNumberOfSkillPoints()
    {
        return _player.skillPoints >= skill.requiredNumberOfSkillPointsToUnlockOrUpgrade;
    }

    private bool CheckSkillUpgradeRequirements()
    {
        return CheckIfPlayerHasRequiredNumberOfSkillPoints() &&
               _player.intelligence >= _activatedSkill.requiredIntelligenceLevel &&
               _player.curLevel >= _activatedSkill.requiredNumberOfSkillPointsToUnlockOrUpgrade;
    }

    private static bool RequiredSkillsAreActive(List<Skill> l1, List<Skill> l2)
    {
        var query = from firstItem in l1
            join secondItem in l2
                on firstItem.skillName equals secondItem.skillName
            select firstItem;
        return query.Count() != 0;
    }
}
