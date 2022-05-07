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

    }

    private void Update()
    {
        UpdateHealthBarsPositions();
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
        {
            mainCamera = Camera.main;
        }

        for (int i = 0; i < _enemiesHealthBars.Count; i++)
        {
            var healthBar = _enemiesHealthBars[i];
            if (healthBar == null) continue;
            if (healthBar.TargetPawn == null || healthBar.TargetPawn.Health.IsDead)
            {
                DestroyHealthBar(healthBar);
                continue;
            }

            Vector3 pawnScreenPoint = Camera.main.WorldToScreenPoint(healthBar.TargetPawn.transform.position);
            if (IsTargetVisible(pawnScreenPoint))
            {
                healthBar.gameObject.SetActive(true);
            }
            else
            {
                healthBar.gameObject.SetActive(false);
                continue;
            }

            Vector2 pawnViewportPoint = mainCamera.WorldToViewportPoint(healthBar.TargetPawn.HeadBone.transform.position);

            SetHealthBarSizeByCameraDistance(healthBar);


            Vector2 screenPosition = new Vector2(((pawnViewportPoint.x * _targetCanvas.sizeDelta.x) - (_targetCanvas.sizeDelta.x * 0.5f)),
                ((pawnViewportPoint.y * _targetCanvas.sizeDelta.y) - (_targetCanvas.sizeDelta.y * 0.5f)));


            healthBar.rectTransform.anchoredPosition = Vector2.Lerp(healthBar.rectTransform.anchoredPosition, screenPosition + Vector2.Scale((Vector2)healthBar.transform.localScale, _moveOffset), Time.deltaTime * _healthBarMoveSpeed);
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

    public void DestroyEnemyHealthBar(AIBase pawn)
    {
        if (pawn == null) return;
        StartCoroutine(IEDestroyPawnHealthBar(pawn));
    }

    void DestroyHealthBar(UIEnemyHealthBar targetHealthBar)
    {
        if (_enemiesHealthBars.Contains(targetHealthBar)) _enemiesHealthBars.Remove(targetHealthBar);
        targetHealthBar.HideWithDelay();
        Destroy(targetHealthBar.gameObject, 2f);
    }


    IEnumerator IEDestroyPawnHealthBar(AIBase pawn)
    {
        if (pawn == null) yield break;
        yield return new WaitForSecondsRealtime(1);
        if (_enemiesHealthBars.Any(hb => hb.TargetPawn == pawn))
        {
            var targetHealthBar = _enemiesHealthBars.FirstOrDefault(hb => hb.TargetPawn == pawn);
            if (targetHealthBar == null) yield break;
            if (_enemiesHealthBars.Contains(targetHealthBar)) _enemiesHealthBars.Remove(targetHealthBar);
            Destroy(targetHealthBar.gameObject);
        }
    }

}
