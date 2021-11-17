using System;
using Enemies;
using TMPro;
using UnityEngine;
using Player;
using UnityEngine.UI;

namespace UI
{
    public class PlayerUi : MonoBehaviour
    {
        [Range(0.001f, 0.01f)]
        [SerializeField] private float fillSmoothness;

        [Header("Health Bar")]
        public Image healthBarFill;
        public TextMeshProUGUI healthBarFillText;

        [Header("Mana Bar")] public Animator        manaBarAnimator;
        public                      Image           manaBarFill;
        public                      TextMeshProUGUI manaBarFillText;

        [Header("Level Information")]
        public TextMeshProUGUI levelText;

        public TextMeshProUGUI skillPointsText;
        public TextMeshProUGUI xpFillText; // The text showing that shows: currentXP/XP to next level
        public Image           xpBarFill;

        [Header("Enemy Nameplate")] [Space] 
        
        [SerializeField] private GameObject enemyInformation;

        [SerializeField] private Image           enemyHpFill;
        [SerializeField] private TextMeshProUGUI enemyHpText;
        [SerializeField] private TextMeshProUGUI enemyNameplateText;
        [SerializeField] private TextMeshProUGUI enemyLevelText;
        private                  bool            _enemyInfoPanelOpen;
        
        [Space]
        public TextMeshProUGUI interactText;

        private Player.Player _player;
        private AbilitiesSystem _playerAbilitySystem;

        private void OnEnable()
        {
            AbilitiesSystem.OnLevelUp += UpdatePlayerLevelStats;
        }

        private void OnDisable()
        {
            AbilitiesSystem.OnLevelUp -= UpdatePlayerLevelStats;
        }

        private void UpdatePlayerLevelStats()
        {
            UpdateLevelText();
            UpdateSkillPointsText();
        }

        private void Awake()
        {
            // Get the player
            _player = FindObjectOfType<Player.Player>();
            _playerAbilitySystem = _player.GetComponent<AbilitiesSystem>();
        }

        private void Update()
        {
            UpdateHealthBar();
            UpdateXpBar();
            UpdateManaBar();
        }

        public void UpdateLevelText()
        {
            levelText.text = _playerAbilitySystem.curLevel.ToString();
        }

        public void UpdateSkillPointsText()
        {
            skillPointsText.SetText($"Skill Points: {_playerAbilitySystem.skillPoints}");
        }

        private void UpdateHealthBar()
        {
            float prevHpFill    = healthBarFill.fillAmount;
            float currentHpFill = (float)_player.CurrentHp / _player.maxHp;

            if (currentHpFill > prevHpFill) prevHpFill = Mathf.Min(prevHpFill + fillSmoothness, currentHpFill);
            else if (currentHpFill < prevHpFill)
                prevHpFill = Mathf.Max(prevHpFill - fillSmoothness, currentHpFill);

            healthBarFill.fillAmount = prevHpFill;
            healthBarFillText.SetText($"{_player.CurrentHp}/{_player.maxHp}");
        }

        private void UpdateXpBar()
        {
            float prevXpFill    = xpBarFill.fillAmount;
            float currentXpFill = (float)_playerAbilitySystem.curXp / _playerAbilitySystem.xpToNextLevel;

            if (currentXpFill > prevXpFill) prevXpFill = Mathf.Min(prevXpFill + fillSmoothness, currentXpFill);
            else if (currentXpFill < prevXpFill)
                prevXpFill = Mathf.Max(prevXpFill - fillSmoothness, currentXpFill);

            xpBarFill.fillAmount = prevXpFill;
            xpFillText.SetText($"{_playerAbilitySystem.curXp}/{_playerAbilitySystem.xpToNextLevel}");
        }

        private void UpdateManaBar()
        {
            float prevManaFill    = manaBarFill.fillAmount;
            float currentManaFill = (float)_playerAbilitySystem.CurrentMana / _playerAbilitySystem.maxMana;
            if (currentManaFill > prevManaFill) prevManaFill = Mathf.Min(prevManaFill + fillSmoothness, currentManaFill);
            else if (currentManaFill < prevManaFill)
                prevManaFill = Mathf.Max(prevManaFill - fillSmoothness, currentManaFill);
            manaBarFill.fillAmount = prevManaFill;
            manaBarFillText.SetText($"{(int)_playerAbilitySystem.CurrentMana}/{_playerAbilitySystem.maxMana}");
        }

        public void ToggleEnemyInfoPanel(bool isActive)
        {
            enemyInformation.SetActive(isActive);
        }

        public void UpdateEnemyInformationPanel(Enemy enemy)
        {
            enemyNameplateText.SetText($"{enemy.enemyName}");
            enemyLevelText.SetText($"{enemy.enemyLevel}");
            enemyHpFill.fillAmount = (float)enemy.CurHp / enemy.maxHp;
            enemyHpText.SetText($"{enemy.CurHp}/{enemy.maxHp}");
        }

        public void SetInteractText(Vector3 pos, string text)
        {
            interactText.gameObject.SetActive(true);
            interactText.text = text;

            if (Camera.main != null) interactText.transform.position = Camera.main.WorldToScreenPoint(pos + new Vector3(0, 0.5f, 0));
        }

        public void DisableInteractText()
        {
            if (interactText.gameObject.activeInHierarchy)
            {
                interactText.gameObject.SetActive(false);
            }
        }
    }
}
