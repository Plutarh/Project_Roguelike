using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FireBallAbility : BaseAbility
{
    [SerializeField] private Projectile _fireBallPrefab;
    [SerializeField] private int _muzzleIndex;

    [SerializeField] private List<Projectile> _primaryAttackProjectilesToMove = new List<Projectile>();

    public override void PrepareExecuting()
    {
        base.PrepareExecuting();

        _muzzleIndex = _abilityPositionIndex;

        var projectile = CreateProjectile(_fireBallPrefab);

        projectile.SetOwner(owner);
        _primaryAttackProjectilesToMove.Add(projectile);


        projectile.transform.SetParent(_abilityExecutePositions[_muzzleIndex].transform);
        projectile.transform.localPosition = Vector3.zero;
    }

    public override void Execute()
    {
        base.Execute();
        Debug.LogError("Primary abil");

        var firstProjectile = _primaryAttackProjectilesToMove.First();

        if (firstProjectile == null)
        {
            Debug.LogError("Animation didnt create projectile - PrimaryAttack_1");
            return;
        }

        _primaryAttackProjectilesToMove.RemoveAt(0);

        firstProjectile.transform.SetParent(null);
        firstProjectile.SetProjectileDirection(owner.GetAimPoint());
        firstProjectile.StartMove();
    }

    Projectile CreateProjectile(Projectile prefab)
    {
        var createdProjectile = Instantiate(prefab, _abilityExecutePositions[_muzzleIndex].transform.position, Quaternion.identity);

        return createdProjectile;
    }
}
