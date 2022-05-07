using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAimPanel : MonoBehaviour
{
    [SerializeField] private UIAimIcon _aimIcon;
    private void Awake()
    {
        GlobalEvents.OnPlayerHittedDamageable += ShowHitIndicator;
    }

    void ShowHitIndicator()
    {
        _aimIcon.ShowIndicator();
    }

    private void OnDestroy()
    {
        GlobalEvents.OnPlayerHittedDamageable -= ShowHitIndicator;
    }
}
