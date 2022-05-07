using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UIAimIcon : MonoBehaviour
{
    [SerializeField] private CanvasGroup _onHitIndicator;

    Tween _onHitTween;

    private void Awake()
    {
        HideIndicator();
    }

    public void HideIndicator()
    {
        if (_onHitIndicator == null)
        {
            Debug.LogError("On hit indicator NULLED ref", this);
            return;
        }
        _onHitIndicator.alpha = 0;
    }

    public void ShowIndicator()
    {
        if (_onHitIndicator == null)
        {
            Debug.LogError("On hit indicator NULLED ref", this);
            return;
        }

        if (_onHitTween != null)
        {
            _onHitTween.Kill();
            _onHitTween = null;

            _onHitIndicator.transform.localScale = Vector3.one;
        }

        _onHitIndicator.transform.DOScale(Vector3.one * 1.25f, 0.4f);
        _onHitIndicator.alpha = 1;

        _onHitTween = _onHitIndicator.DOFade(0, 0.4f).SetDelay(0.2f).OnComplete(() =>
        {

        });

    }
}
