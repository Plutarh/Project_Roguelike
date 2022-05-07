using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UICombatText : MonoBehaviour
{
    [SerializeField] private Color _baseDamageColor;
    [SerializeField] private Color _critDamageColor;
    [SerializeField] private Color _healingColor;

    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TextMeshProUGUI _text;

    [SerializeField] private RectTransform _rectTransform;

    public void ShowText(CombatData combatData)
    {
        _text.text = combatData.combatValue.ToString();

        if (combatData is DamageData)
        {
            var cd = combatData as DamageData;


            Color textColor = cd.isCritical ? _critDamageColor : _baseDamageColor;

            _text.color = textColor;
        }
        else if (combatData is HealData)
        {
            var hd = combatData as HealData;
            _text.color = _healingColor;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        Vector2 randomPosition = Random.insideUnitCircle * 100;


        _rectTransform.DOAnchorPos(randomPosition, 2.5f).SetEase(Ease.OutBack);
        _canvasGroup.DOFade(0, 0.7f).SetDelay(0.6f);
    }
}
