using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FireMeteorAbility : BaseAbility
{
    [SerializeField] private Projectile _meteorPrefab;

    private Projectile _meteor;

    public override void PrepareExecuting(DamageData damageData)
    {
        base.PrepareExecuting(damageData);



        _meteor = CreateProjectile(_meteorPrefab);

        _meteor.SetOwner(owner);

        _meteor.SetDamageData(damageData);



        Debug.LogError("Create meteor");
    }

    public override void Execute()
    {
        base.Execute();


        _meteor.transform.SetParent(null);
        _meteor.SetProjectileDirection(owner.GetAimPoint());
        _meteor.StartMove();
    }

    Projectile CreateProjectile(Projectile prefab)
    {
        var createdProjectile = Instantiate(prefab, owner.transform.position + Vector3.up * 14, Quaternion.identity);

        return createdProjectile;
    }
}
