using Player;
using TMPro;
using UnityEngine;

public class PlayerStatsUI : MonoBehaviour
{
    private Player.Player   _player;
    private AbilitiesSystem _playerAbilities;
    
    [Header("UI Elements")]
    
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI coinAmount;
    [SerializeField] private TextMeshProUGUI skillPointsAmount;
    [SerializeField] private TextMeshProUGUI playerLevel;
    [SerializeField] private TextMeshProUGUI playerIntelligence;
    [SerializeField] private TextMeshProUGUI playerStrength;
    [SerializeField] private TextMeshProUGUI physicalResistanceAmount;
    [SerializeField] private TextMeshProUGUI elementalResistanceAmount;
    [SerializeField] private TextMeshProUGUI manaRegenerationAmount;
    [SerializeField] private TextMeshProUGUI physicalDamageAmount;
    [SerializeField] private TextMeshProUGUI elementalDamageAmount;

    private void Start()
    {
        _player = GameManager.Instance.player;
        _playerAbilities = _player.GetComponent<AbilitiesSystem>();
        
        // player name should not change during game
        playerName.SetText("Callum");
    }

    private void Update()
    {
        coinAmount.SetText($"{_player.GoldCoinAmount}");
        skillPointsAmount.SetText($"{_playerAbilities.skillPoints}");
        playerLevel.SetText($"{_playerAbilities.curLevel}");
        playerIntelligence.SetText($"{_playerAbilities.intelligence}");
        playerStrength.SetText($"{_playerAbilities.strength}");
        physicalResistanceAmount.SetText($"{_playerAbilities.strengthPhysicalDamageIncreaseAmount}%");
        elementalResistanceAmount.SetText($"{_playerAbilities.intelligenceElementalDamageIncreaseAmount}%");
        manaRegenerationAmount.SetText($"{_playerAbilities.manaRegenerationPercentage}%");
        physicalDamageAmount.SetText($"{_playerAbilities.CalculatePhysicalDamage()}");
        elementalDamageAmount.SetText($"{_playerAbilities.CalculateElementalDamage()}");
    }
}
