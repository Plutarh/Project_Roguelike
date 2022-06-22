using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class FireMeteorAbility : BaseAbility
{
    [SerializeField] private Projectile _meteorPrefab;

    private Projectile _meteor;


    public override void PrepareExecuting(DamageData damageData)
    {
        base.PrepareExecuting(damageData);

        _damageData = damageData;
        TryCreateMeteor();
    }

    public override void Execute()
    {
        base.Execute();
        ProjectileNetworkData projectileNetworkData = new ProjectileNetworkData();

        projectileNetworkData.createPosition = owner.transform.position + Vector3.up * 14;
        projectileNetworkData.damageData = _damageData;
        projectileNetworkData.moveDirection = playerCharacter.PlayerMover.GetAimDirection();
        projectileNetworkData.owner = owner;

        TryToLaunchMeteor(projectileNetworkData);
    }

    Projectile CreateProjectile(Projectile prefab)
    {
        var createdProjectile = Instantiate(prefab, owner.transform.position + Vector3.up * 14, Quaternion.identity);

        return createdProjectile;
    }

    void TryCreateMeteor()
    {
        CreateMeteor();
        CmdCreateMeteor();
    }

    void CreateMeteor()
    {
        _meteor = CreateProjectile(_meteorPrefab);
        _meteor.SetOwner(owner);
        _meteor.SetDamageData(_damageData);
    }

    [Command]
    void CmdCreateMeteor()
    {
        RpcCreateMeteor();
    }

    [ClientRpc(includeOwner = false)]
    void RpcCreateMeteor()
    {
        CreateMeteor();
    }

    void TryToLaunchMeteor(ProjectileNetworkData projectileNetworkData)
    {
        LaunchMeteor(projectileNetworkData);
        CmdLaunchMeteor(projectileNetworkData);
    }

    void LaunchMeteor(ProjectileNetworkData projectileNetworkData)
    {
        if (_meteor == null)
        {
            Debug.LogError("Meteor didnt created", playerCharacter);
            return;
        }
        _meteor.transform.SetParent(null);
        _meteor.Initialize(projectileNetworkData);
    }

    [Command]
    void CmdLaunchMeteor(ProjectileNetworkData projectileNetworkData)
    {
        RpcLauncMeteor(projectileNetworkData);
    }

    [ClientRpc(includeOwner = false)]
    void RpcLauncMeteor(ProjectileNetworkData projectileNetworkData)
    {
        LaunchMeteor(projectileNetworkData);

        if (_meteor != null) _meteor.SetupAsNetworkVisual();
    }
}
