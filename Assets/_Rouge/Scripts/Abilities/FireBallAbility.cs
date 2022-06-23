using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class FireBallAbility : BaseAbility
{
    [SerializeField] private Projectile _fireBallPrefab;
    // [SerializeField] private int _muzzleIndex;

    [SerializeField] private List<Projectile> _primaryAttackProjectilesToMove = new List<Projectile>();

    private Projectile _lastCreatedProjectile;

    public override void Awake()
    {
        base.Awake();
    }

    public override void PrepareExecuting(DamageData damageData)
    {
        if (!owner.isLocalPlayer) return;
        base.PrepareExecuting(damageData);


        _damageData = damageData;


        ProjectileNetworkData projectileData = new ProjectileNetworkData();

        projectileData.damageData = _damageData;
        projectileData.createPosition = _abilityExecutePositions[_abilityPositionIndex].transform.position;
        projectileData.moveDirection = playerCharacter.PlayerMover.GetAimPoint();
        projectileData.owner = owner;


        var createdProjectile = CreateProjectile(projectileData);

        foreach (var effect in _effects)
        {
            createdProjectile.AddScriptableEffect(effect);
        }

        CmdCreateProjectile(projectileData);


        return;


        if (_primaryAttackProjectilesToMove.Count == 0)
        {
            Debug.LogError("No bullet to spawn");
            return;
        }

        Projectile projectile = _primaryAttackProjectilesToMove.FirstOrDefault();

        projectile.transform.SetParent(_abilityExecutePositions[_abilityPositionIndex].transform);
        projectile.transform.localPosition = Vector3.zero;
    }

    public override void Execute()
    {
        return;
        if (!owner.isLocalPlayer) return;
        base.Execute();



        var projectileToMove = _primaryAttackProjectilesToMove.FirstOrDefault();

        if (projectileToMove == null)
        {
            Debug.LogError("Animation didnt create projectile - PrimaryAttack_1");
            _primaryAttackProjectilesToMove.Clear();
            return;
        }

        projectileToMove.transform.SetParent(null);

        ProjectileNetworkData projectileData = new ProjectileNetworkData();

        //projectileData.effects = _effects;
        //projectileData.damageData = _damageData;
        projectileData.createPosition = projectileToMove.transform.position;
        //projectileData.moveDirection = owner.GetComponent<BaseCharacter>().GetAimPoint();
        //projectileData.owner = owner;

        projectileToMove.Initialize(projectileData);

        _primaryAttackProjectilesToMove.RemoveAt(0);
    }

    Projectile CreateProjectile(ProjectileNetworkData projectileNetworkData)
    {
        var createdProjectile = Instantiate(_fireBallPrefab, projectileNetworkData.createPosition, Quaternion.identity);
        createdProjectile.Initialize(projectileNetworkData);
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
    }
}

