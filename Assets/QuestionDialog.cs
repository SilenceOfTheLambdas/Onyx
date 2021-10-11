using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using SuperuserUtils;

public class QuestionDialog : GenericSingletonClass<QuestionDialog>
{
    [SerializeField] private TextMeshProUGUI _questionText;
    [SerializeField] private Button _yesBtn;
    [SerializeField] private Button _noBtn;

    private void Start()
    {
        Hide();
    }

    public void ShowQuestion(string questionText, Action yesAction, Action noAction)
    {
        gameObject.SetActive(true);

        _questionText.text = questionText;
        _yesBtn.onClick.AddListener(() =>
        {
            Hide();
            yesAction();
        });
        _noBtn.onClick.AddListener(() =>
        {
            Hide();
            noAction();
        });
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
