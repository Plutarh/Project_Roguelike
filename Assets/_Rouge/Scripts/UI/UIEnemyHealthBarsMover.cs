using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class UIEnemyHealthBarsMover : MonoBehaviour
{
    [SerializeField]
    private List<UIEnemyHealthBar> _enemiesHealthBars = new List<UIEnemyHealthBar>();

    [SerializeField]
    private float _healthBarMoveSpeed = 5;

    [SerializeField]
    private Vector2 _moveOffset;

    [SerializeField]
    private RectTransform _targetCanvas;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        UpdateHealthBarsPositions();
    }

    private void Update()
    {

    }

    public void AddNewHealthBar(UIEnemyHealthBar newHealthBar)
    {
        if (_enemiesHealthBars.Contains(newHealthBar))
            return;

        _enemiesHealthBars.Add(newHealthBar);
    }

    void UpdateHealthBarsPositions()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        for (int i = 0; i < _enemiesHealthBars.Count; i++)
        {
            var healthBar = _enemiesHealthBars[i];
            if (healthBar == null || healthBar.TargetPawn == null)
                continue;

            if (healthBar.TargetPawn.Health.IsDead)
                DisableEnemyHealthBar(healthBar);

            Vector3 pawnScreenPoint = Camera.main.WorldToScreenPoint(healthBar.TargetPawn.transform.position);

            if (IsTargetVisible(pawnScreenPoint))
                healthBar.gameObject.SetActive(true);
            else
                healthBar.gameObject.SetActive(false);


            Vector2 pawnViewportPoint = mainCamera.WorldToViewportPoint(healthBar.TargetPawn.HeadBone.transform.position);

            SetHealthBarSizeByCameraDistance(healthBar);

            Vector2 screenPosition = new Vector2(((pawnViewportPoint.x * _targetCanvas.sizeDelta.x) - (_targetCanvas.sizeDelta.x * 0.5f)),
                ((pawnViewportPoint.y * _targetCanvas.sizeDelta.y) - (_targetCanvas.sizeDelta.y * 0.5f)));

            healthBar.rectTransform.anchoredPosition = Vector2.Lerp(healthBar.rectTransform.anchoredPosition
                , screenPosition + Vector2.Scale((Vector2)healthBar.transform.localScale, _moveOffset)
                , Time.deltaTime * _healthBarMoveSpeed);
        }
    }

    bool IsTargetVisible(Vector3 screenPosition)
    {
        bool isTargetVisible = screenPosition.z > 0 &&
            screenPosition.x > 0 &&
            screenPosition.x < Screen.width &&
            screenPosition.y > 0 &&
            screenPosition.y < Screen.height;

        return isTargetVisible;
    }

    void SetHealthBarSizeByCameraDistance(UIEnemyHealthBar healthBar)
    {
        float distance = Vector3.Distance(healthBar.TargetPawn.transform.position, mainCamera.transform.position);
        float lerpScale = Mathf.InverseLerp(5, 50, distance);
        healthBar.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.3f, lerpScale);
    }

    public void DestroyAllBars()
    {
        _enemiesHealthBars.ForEach(hb => Destroy(hb.gameObject));
        _enemiesHealthBars.Clear();
    }


    void DisableEnemyHealthBar(UIEnemyHealthBar healthBar)
    {
        if (healthBar.IsDeactivated) return;
        healthBar.Deactivate();
        healthBar.HideWithDelay(1);
        healthBar.OnHide += DestroyEnemyHealthBar;
    }

    void DestroyEnemyHealthBar(UIEnemyHealthBar healthBar)
    {
        if (_enemiesHealthBars.Contains(healthBar))
            _enemiesHealthBars.Remove(healthBar);

        healthBar.OnHide -= DestroyEnemyHealthBar;
        Destroy(healthBar.gameObject);
    }

}
