using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAbilityFrame : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _keyNameText;
    [SerializeField] private Image _keyIcon;

    [SerializeField] private Image _abilityIcon;
    [SerializeField] private Image _cooldownProgress;

    public void SetCooldownProgress(float value)
    {
        _cooldownProgress.fillAmount = value;
    }

    public void SetAbilityIcon(Sprite icon)
    {
        _abilityIcon.sprite = icon;
    }

    public void SetKeyName(string keyName)
    {
        _keyIcon.gameObject.SetActive(false);
        _keyNameText.text = keyName;
        _keyNameText.gameObject.SetActive(true);
    }

    public void SetKeyIcon(Sprite icon)
    {
        _keyNameText.gameObject.SetActive(false);
        _keyIcon.sprite = icon;
        _keyIcon.gameObject.SetActive(true);
    }
}
