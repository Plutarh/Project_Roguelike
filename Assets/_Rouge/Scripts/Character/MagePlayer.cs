using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagePlayer : MonoBehaviour
{
    [SerializeField] private List<Transform> _projectileMuzzles = new List<Transform>();

    [SerializeField] private Projectile _primaryAttackProjectilePrefab;

    Player _player;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    public void AnimStartPrimaryAttack_1()
    {
        var projectile = CreateProjectile(_primaryAttackProjectilePrefab);

        projectile.SetMoveDirection(_player.GetAimDirection());
    }

    public void AnimStartPrimaryAttack_2()
    {
        var projectile = CreateProjectile(_primaryAttackProjectilePrefab);

        projectile.SetMoveDirection(_player.GetAimDirection());
    }

    Projectile CreateProjectile(Projectile prefab)
    {
        var createdProjectile = Instantiate(prefab, _projectileMuzzles[0].transform.position, Quaternion.identity);

        return createdProjectile;
    }

}
