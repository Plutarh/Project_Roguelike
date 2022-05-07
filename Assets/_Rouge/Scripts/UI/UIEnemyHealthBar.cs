using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIEnemyHealthBar : UIHealthBar
{
    public RectTransform rectTransform;


    Tween hideTween;
    Tween showTween;

    [SerializeField] private UICombatText _combatTextPrefab;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }


    public override void UpdateBar(CombatData combatData)
    {
        if (showTween != null)
        {
            showTween.Kill();
            showTween = null;
        }
        showTween = canvasGroup.DOFade(1, 0.05f);

        base.UpdateBar(combatData);

        if (combatData is DamageData)
            ShowDamageText(combatData as DamageData);
        HideWithDelay(3);
    }

    void ShowDamageText(DamageData damageData)
    {
        var createdCombatText = Instantiate(_combatTextPrefab, _combatTextParent.transform);

        createdCombatText.ShowText(damageData);
    }

    public void HideWithDelay(float delay = 0)
    {
        if (hideTween != null)
        {
            hideTween.Kill();
            hideTween = null;
        }

        hideTween = canvasGroup.DOFade(0, 0.2f).SetDelay(delay);
    }
}
