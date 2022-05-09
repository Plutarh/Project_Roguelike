using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FireCircleAbility : BaseAbility
{
    [SerializeField] private GameObject _fx;
    [SerializeField] private float _radius = 1;

    [SerializeField] private LayerMask _layer;

    [SerializeField] private TrailRenderer _handsTrailPrefab;

    private List<TrailRenderer> _trailRenderers = new List<TrailRenderer>();

    public override void PrepareExecuting(DamageData damageData)
    {
        base.PrepareExecuting(damageData);

        CreateHandsTrail();
    }

    void CreateHandsTrail()
    {
        foreach (var pos in _abilityExecutePositions)
        {
            var createdTrail = Instantiate(_handsTrailPrefab, pos);
            createdTrail.emitting = true;

            for (int i = 0; i < createdTrail.colorGradient.colorKeys.Length; i++)
            {
                var color = createdTrail.colorGradient.colorKeys[i];
                color.color *= 2;
            }

            createdTrail.transform.localPosition = Vector3.zero;
            _trailRenderers.Add(createdTrail);
        }
    }

    void DestroyHandsTrail()
    {
        foreach (var trail in _trailRenderers)
        {
            trail.emitting = false;
            Destroy(trail.gameObject, 0.8f);
        }

        _trailRenderers.Clear();
    }

    public override void Execute()
    {
        base.Execute();
        CreateFX();
        CastDamageShpere();
        DestroyHandsTrail();
    }

    void CreateFX()
    {
        if (_fx == null) return;
        var fx = Instantiate(_fx, owner.transform.position + Vector3.up * 0.4f, Quaternion.identity);

        Destroy(fx, 3);
    }
    bool show;
    void CastDamageShpere()
    {
        var colliders = Physics.OverlapSphere(owner.transform.position, _radius, _layer).ToList();

        foreach (var col in colliders)
        {
            var pawn = col.transform.root.GetComponent<IDamageable>();

            if (pawn == null) continue;
            if (pawn.GetTeam() == owner.GetTeam()) continue;
            pawn.TakeDamage(_damageData);
        }
        show = true;
    }

    private void OnDrawGizmos()
    {
        if (show == false) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
