using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    public Pawn TargetPawn
    {
        get => _pawn;
    }

    [SerializeField] protected CanvasGroup canvasGroup;
    [SerializeField] private Image _healthImage;
    [SerializeField] private Image _damageImage;
    [SerializeField] protected GameObject _combatTextParent;

    protected Pawn _pawn;

    public void SetPawn(Pawn pawn)
    {
        _pawn = pawn;

        _pawn.Health.OnHealthDecreased += UpdateBar;
    }

    public void ResetBar()
    {
        _damageImage.fillAmount = 1;
        _healthImage.fillAmount = 1;
    }

    public virtual void UpdateBar(CombatData damageData)
    {

        float targetValue = _pawn.Health.GetHealth01();
        _healthImage.DOFillAmount(targetValue, 0.1f).OnComplete(() =>
        {
            _damageImage.DOFillAmount(targetValue, 0.2f);
        });
    }

    private void OnDestroy()
    {
        _pawn.Health.OnHealthDecreased -= UpdateBar;
    }
}
