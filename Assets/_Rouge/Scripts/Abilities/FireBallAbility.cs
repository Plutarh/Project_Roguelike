using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class FireBallAbility : BaseAbility
{
    [SerializeField] private Projectile _fireBallPrefab;

    private Projectile _lastCreatedProjectile;
    private ProjectileNetworkData _lastProjectileData;

    public override void Awake()
    {
        base.Awake();
    }

    public override void PrepareExecuting(DamageData damageData)
    {

        base.PrepareExecuting(damageData);


        _damageData = damageData;


        ProjectileNetworkData projectileData = new ProjectileNetworkData();

        projectileData.damageData = _damageData;
        projectileData.createPosition = _abilityExecutePositions[_abilityPositionIndex].transform.position;
        projectileData.moveDirection = playerCharacter.PlayerMover.GetAimPoint();
        projectileData.owner = owner;
        projectileData.fromProjectilDirection = true;

        _lastCreatedProjectile = CreateProjectile(projectileData);
        _lastProjectileData = projectileData;

        foreach (var effect in _effects)
        {
            _lastCreatedProjectile.AddScriptableEffect(effect);
        }

        CmdCreateProjectile(projectileData);

        _lastCreatedProjectile.transform.SetParent(_abilityExecutePositions[_abilityPositionIndex].transform);
        _lastCreatedProjectile.transform.localPosition = Vector3.zero;
    }

    public override void Execute()
    {
        base.Execute();

        _lastCreatedProjectile.transform.SetParent(null);
        _lastCreatedProjectile.Initialize(_lastProjectileData);

        CmdThrowProjectile(_lastProjectileData);
    }

    Projectile CreateProjectile(ProjectileNetworkData projectileNetworkData)
    {
        var createdProjectile = Instantiate(_fireBallPrefab, projectileNetworkData.createPosition, Quaternion.identity);
        return createdProjectile;
    }

    [Command]
    void CmdCreateProjectile(ProjectileNetworkData projectileNetworkData)
    {
        RpcCreateProjectile(projectileNetworkData);
    }

    [ClientRpc(includeOwner = false)]
    void RpcCreateProjectile(ProjectileNetworkData projectileNetworkData)
    {
        var localProjectile = CreateProjectile(projectileNetworkData);
        localProjectile.SetupAsNetworkVisual();
        _lastCreatedProjectile = localProjectile;
    }

    [Command]
    void CmdThrowProjectile(ProjectileNetworkData projectileNetworkData)
    {
        RpcThrowProjectile(projectileNetworkData);
    }

    [ClientRpc(includeOwner = false)]
    void RpcThrowProjectile(ProjectileNetworkData projectileNetworkData)
    {
        _lastCreatedProjectile.transform.SetParent(null);
        _lastCreatedProjectile.Initialize(projectileNetworkData);
    }
}

