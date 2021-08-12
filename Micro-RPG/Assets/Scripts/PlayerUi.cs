using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUi : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI interactText;
    public TextMeshProUGUI xpFillText; // The text showing that shows: currentXP/XP to next level
    public Image           healthBarFill;
    public Image           xpBarFill;

    private Player _player;

    private void Awake()
    {
        // Get the player
        _player = FindObjectOfType<Player>();
    }

    public void UpdateLevelText()
    {
        levelText.text = _player.curLevel.ToString();
    }

    public void UpdateHealthBar()
    {
        healthBarFill.fillAmount = (float)_player.CurrentHp / _player.maxHp;
    }

    public void UpdateXpBar()
    {
        xpBarFill.fillAmount = (float) _player.curXp / _player.xpToNextLevel;
        xpFillText.text = _player.curXp + "/" + _player.xpToNextLevel;
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
