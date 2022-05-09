using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FireCircleAbility : BaseAbility
{
    [SerializeField] private GameObject _fx;
    [SerializeField] private float _radius = 1;

    [SerializeField] private LayerMask _layer;

    public override void PrepareExecuting(DamageData damageData)
    {
        base.PrepareExecuting(damageData);


    }

    public override void Execute()
    {
        base.Execute();
        CreateFX();
        CastDamageShpere();
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
