using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class PlayerHUD : MonoBehaviour
{
    PlayerMover _player;

    [SerializeField] private CanvasGroup _playerDamagedOverlay;


    Tween _playerDamagedOverlayTween;
    bool _isInited;

    private void Awake()
    {
        ResetDamagedOverlay();
        GlobalEvents.OnLocalPlayerInitialized += Initialize;
    }

    void Initialize(PlayerMover player)
    {
        _player = player;
        _player.Health.OnHealthDecreased += ShowDamagedOverlay;
        _isInited = true;
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
        if (!_isInited) return;

        GlobalEvents.OnLocalPlayerInitialized -= Initialize;
        _player.Health.OnHealthDecreased -= ShowDamagedOverlay;
    }
}
