using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUi : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI inventoryText;
    public TextMeshProUGUI interactText;
    public Image healthBarFill;
    public Image xpBarFill;

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
        healthBarFill.fillAmount = (float)_player.curHp / _player.maxHp;
    }

    public void UpdateXpBar()
    {
        xpBarFill.fillAmount = (float) _player.curXp / _player.xpToNextLevel;
    }

    public void setInteractText(Vector3 pos, string text)
    {
        interactText.gameObject.SetActive(true);
        interactText.text = text;

        if (Camera.main != null) interactText.transform.position = Camera.main.WorldToScreenPoint(pos + Vector3.up);
    }

    public void DisableInteractText()
    {
        if (interactText.gameObject.activeInHierarchy)
        {
            interactText.gameObject.SetActive(false);
        }
    }

    // public void UpdateInventoryText()
    // {
    //     inventoryText.text = "";
    //
    //     foreach (var item in _player.inventory)
    //     {
    //         inventoryText.text += item.ToString().Replace('_', ' ') + "\n";
    //     }
    // }
}
