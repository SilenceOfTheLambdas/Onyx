using System;
using Skills;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillHotbar : MonoBehaviour
{
    [NonSerialized]  public  Skill           Skill;
    [SerializeField] private SkillsManager   skillsManager;
    [SerializeField] private Image           coolDownImage;
    [SerializeField] private TextMeshProUGUI coolDownText;

    private Image _skillSprite;

    private void Awake()
    {
        _skillSprite = transform.Find("SkillImage").GetComponent<Image>();
    }

    public void AssignSkillToHotbar(Skill skillToAssign)
    {
        Skill = skillToAssign;
        
        var activeSkill = skillsManager.activeSkills.Find(s => s.skillName.Equals(Skill.skillName));
        if (activeSkill != null)
        {
            Skill.CoolDownImage = coolDownImage;
            Skill.CoolDownText = coolDownText;
            activeSkill.CoolDownImage = coolDownImage;
            activeSkill.CoolDownText = coolDownText;
            _skillSprite.gameObject.SetActive(true);
            _skillSprite.sprite = Skill.skillSprite;
        }
    }
}
