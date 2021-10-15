using System.Collections.Generic;
using System.Linq;
using Skills;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Player;
using Object = UnityEngine.Object;

public class SkillTreeSkillManager : MonoBehaviour
{
    public Skill skill;
    [SerializeField] private SkillsManager skillsManager;
    [SerializeField] private PlayerUi playerUi;
    [SerializeField] private GameObject skillInfoHover;

    [Header("User Interface")]
    [SerializeField]
    private Vector2 hoverOverlayPositionOffset;
    private Image _overlay;
    private Image _border;
    private Image _skillSpriteHolder;
    private TextMeshProUGUI _levelText;
    private TextMeshProUGUI _skillName, _skillDescription, _skillLevel, _skillType, _skillStats, _skillRequirements;

    private Button_UI _buttonUI;
    private AbilitiesSystem _playerAbilitySystem;
    private Skill _activatedSkill;

    private void Awake()
    {
        _overlay = transform.Find("overlay").GetComponent<Image>();
        _border = transform.Find("border").GetComponent<Image>();
        _skillSpriteHolder = transform.Find("skillSprite").GetComponent<Image>();
        _levelText = transform.Find("levelText").GetComponent<TextMeshProUGUI>();
        _buttonUI = GetComponent<Button_UI>();
        _playerAbilitySystem = skillsManager.GetComponent<AbilitiesSystem>();

        // Setting Hover UI Stuff
        _skillName = skillInfoHover.transform.Find("skillName").GetComponent<TextMeshProUGUI>();
        _skillDescription = skillInfoHover.transform.Find("skillDescription").GetComponent<TextMeshProUGUI>();
        _skillLevel = skillInfoHover.transform.Find("skillLevel").GetComponent<TextMeshProUGUI>();
        _skillType = skillInfoHover.transform.Find("skillType").GetComponent<TextMeshProUGUI>();
        _skillStats = skillInfoHover.transform.Find("skillStats").GetComponent<TextMeshProUGUI>();
        _skillRequirements = skillInfoHover.transform.Find("skillRequirements").GetComponent<TextMeshProUGUI>();
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
                _playerAbilitySystem.skillPoints -= _activatedSkill.requiredNumberOfSkillPointsToUnlockOrUpgrade;
                playerUi.UpdateSkillPointsText();
            }


        };

        // If we already have the skill unlocked, and we have the required skills to upgrade
        if (skillsManager.activeSkills.Find(s => s.skillName.Equals(skill.skillName)) != null && CheckSkillUpgradeRequirements())
        {
            _buttonUI.ClickFunc = () =>
            {
                // Level the skill up
                skillsManager.UpgradeSkill(_activatedSkill);
                // Update the tree UI
                _levelText.SetText($"{_activatedSkill.skillLevel}");
                _playerAbilitySystem.skillPoints -= _activatedSkill.requiredNumberOfSkillPointsToUnlockOrUpgrade;
                playerUi.UpdateSkillPointsText();
            };
        }

        _buttonUI.MouseOverOnceTooltipFunc = () =>
        {
            var currentSkill = _activatedSkill != null ? _activatedSkill : skill;
            skillInfoHover.SetActive(true);
            skillInfoHover.transform.position = Mouse.current.position.ReadValue() + hoverOverlayPositionOffset;
            CursorController.Instance.SetCursor(CursorController.CursorTypes.Default);
            _skillName.SetText($"{currentSkill.skillName}");
            _skillDescription.SetText($"{currentSkill.description}");

            if (skillsManager.activeSkills.Find(s => s.skillName.Equals(skill.skillName)) != null &&
                CheckSkillUpgradeRequirements())
            {
                _skillLevel.SetText($"Skill Level: {currentSkill.skillLevel} -> {currentSkill.skillLevel + 1}");
            }
            else
            {
                _skillLevel.SetText($"Skill Level: {currentSkill.skillLevel}");
            }
            _skillType.SetText($"Skill Type: {currentSkill.skillType}");

            // Skill Stats
            _skillStats.SetText($"Skill Damage: {currentSkill.amountOfDamage}\n" +
                                $"Cool Down: {currentSkill.coolDownTime}");
            // Requirements
            _skillRequirements.SetText(
                $"Intelligence: {currentSkill.requiredIntelligenceLevel}   Mana: {currentSkill.manaCost}");
        };

        _buttonUI.MouseOutOnceTooltipFunc = () =>
        {
            skillInfoHover.SetActive(false);
        };
    }

    private bool CheckSkillRequirements()
    {
        return CheckIfPlayerHasRequiredNumberOfSkillPoints() &&
               _playerAbilitySystem.intelligence >= skill.requiredIntelligenceLevel &&
               (RequiredSkillsAreActive(skillsManager.activeSkills, skill.requiredActiveSkills)
                || skill.requiredActiveSkills.Count == 0);
    }

    private bool CheckIfPlayerHasRequiredNumberOfSkillPoints()
    {
        return _playerAbilitySystem.skillPoints >= skill.requiredNumberOfSkillPointsToUnlockOrUpgrade;
    }

    private bool CheckSkillUpgradeRequirements()
    {
        return CheckIfPlayerHasRequiredNumberOfSkillPoints() &&
               _playerAbilitySystem.intelligence >= skill.requiredIntelligenceLevel &&
               _playerAbilitySystem.curLevel >= skill.requiredNumberOfSkillPointsToUnlockOrUpgrade;
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
