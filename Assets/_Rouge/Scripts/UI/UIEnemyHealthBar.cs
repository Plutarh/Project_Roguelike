using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIEnemyHealthBar : UIHealthBar
{
    public bool IsDeactivated => _deactivated;

    public RectTransform rectTransform;

    [SerializeField] private UICombatText _combatTextPrefab;

    private bool _deactivated;

    private Tween _hideTween;
    private Tween _showTween;

    public Action<UIEnemyHealthBar> OnHide;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void QuickHide()
    {
        canvasGroup.alpha = 0;
    }

    public override void UpdateBar(CombatData combatData)
    {
        if (_showTween != null)
        {
            _showTween.Kill();
            _showTween = null;
        }
        _showTween = canvasGroup.DOFade(1, 0.05f);

        base.UpdateBar(combatData);

        if (combatData is DamageData)
            ShowDamageText(combatData as DamageData);

        HideWithDelay(3);
    }

    public void Deactivate()
    {
        _deactivated = true;
    }

    void ShowDamageText(DamageData damageData)
    {
        var createdCombatText = Instantiate(_combatTextPrefab, _combatTextParent.transform);

        createdCombatText.ShowText(damageData);
    }

    public void HideWithDelay(float delay = 0)
    {
        if (_hideTween != null)
        {
            _hideTween.Kill();
            _hideTween = null;
        }

        _hideTween = canvasGroup.DOFade(0, 0.2f).SetDelay(delay).OnComplete(() =>
        {
            OnHide?.Invoke(this);
        });
    }

}
