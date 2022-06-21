using System.Collections.Generic;
using System.Linq;
using Mirror;
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

            CmdCreateHandsTrail(pos.name);
        }
    }

    [Command]
    void CmdCreateHandsTrail(string handName)
    {
        RpcCreatHandsTrail(handName);
    }

    [ClientRpc(includeOwner = false)]
    void RpcCreatHandsTrail(string handName)
    {
        var hand = playerCharacter.GetComponent<MagePlayer>().Hands.FirstOrDefault(h => h.name == handName).transform;

        if (hand == null)
        {
            Debug.LogError("Cannot find hand for client");
            return;
        }

        var trail = Instantiate(_handsTrailPrefab, hand);
        trail.transform.localPosition = Vector3.zero;
        _trailRenderers.Add(trail);
    }

    void StartDestroyHandsTrails()
    {
        DestroyTrails();
        CmdDestroyHandsTrails();
    }

    void DestroyTrails()
    {
        if (_trailRenderers.Count == 0) return;

        foreach (var trail in _trailRenderers)
        {
            if (trail == null)
            {
                Debug.LogError("Hand trail NULLED", playerCharacter);
                continue;
            }

            trail.emitting = false;
            Destroy(trail.gameObject, 0.8f);
        }

        _trailRenderers.Clear();
    }


    [Command(requiresAuthority = false)]
    void CmdDestroyHandsTrails()
    {
        RpcDestroyHandsTrails();
    }

    [ClientRpc(includeOwner = false)]
    void RpcDestroyHandsTrails()
    {
        DestroyTrails();
    }


    public override void Execute()
    {
        base.Execute();
        CreateFX();
        CastDamageShpere();
        StartDestroyHandsTrails();
    }

    void CreateFX()
    {
        if (_fx == null) return;
        var fx = Instantiate(_fx, owner.transform.position + Vector3.up * 0.4f, Quaternion.identity);

        Destroy(fx.gameObject, 3);
        CmdCreateFX(fx.transform.position, fx.transform.rotation);
    }


    [Command]
    void CmdCreateFX(Vector3 position, Quaternion rotation)
    {
        RpcCreateFX(position, rotation);
    }

    [ClientRpc(includeOwner = false)]
    void RpcCreateFX(Vector3 position, Quaternion rotation)
    {
        var fx = Instantiate(_fx, position, rotation);
        Destroy(fx.gameObject, 3);
    }

    void CastDamageShpere()
    {
        var colliders = Physics.OverlapSphere(owner.transform.position, _radius, _layer).ToList();

        foreach (var col in colliders)
        {
            var pawn = col.transform.root.GetComponent<IDamageable>();

            if (pawn == null) continue;
            if (pawn.GetTeam() == owner.GetComponent<BaseCharacter>().GetTeam()) continue;
            pawn.TakeDamage(_damageData);
            AddEffectToDamageable(pawn);
        }

    }

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
