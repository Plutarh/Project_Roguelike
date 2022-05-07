using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class PlayerHUD : MonoBehaviour
{
    Player _player;

    [SerializeField] private CanvasGroup _playerDamagedOverlay;


    Tween _playerDamagedOverlayTween;

    [Inject]
    public void Construct(Player player)
    {
        _player = player;
    }

    private void Awake()
    {
        ResetDamagedOverlay();

        _player.Health.OnHealthDecreased += ShowDamagedOverlay;
    }


    void ResetDamagedOverlay()
    {
        _playerDamagedOverlay.gameObject.SetActive(false);
        _playerDamagedOverlay.alpha = 0;
        _playerDamagedOverlay.transform.localScale = Vector3.one;
    }

    void ShowDamagedOverlay(DamageData damageData)
    {
        if (_playerDamagedOverlayTween != null)
        {
            _playerDamagedOverlayTween.Kill();
            _playerDamagedOverlayTween = null;
        }

        _playerDamagedOverlay.gameObject.SetActive(true);
        _playerDamagedOverlay.alpha = 1;
        _playerDamagedOverlay.transform.localScale = Vector3.one;

        _playerDamagedOverlayTween = _playerDamagedOverlay.transform.DOScale(Vector3.one * 5, 1f).OnComplete(() =>
        {
            _playerDamagedOverlay.DOFade(0, 0.15f).OnComplete(() =>
             {
                 _playerDamagedOverlay.gameObject.SetActive(false);
             });
        });
    }

    private void OnDestroy()
    {
        _player.Health.OnHealthDecreased -= ShowDamagedOverlay;
    }
}
