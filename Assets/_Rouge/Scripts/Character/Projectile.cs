using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    public bool IsInited => _isInited;
    public bool IsNetworkVisual => _isNetworkVisual;

    [SerializeField] private EProjectileDamageType damageType;
    [SerializeField] private float damageRadius;

    public bool immortal;

    [SerializeField] private float _moveSpeed;

    [SerializeField] private float _delayToGravity = 1.5f;
    [SerializeField] private float _lifetime = 3;

    [Header("FX")]
    [SerializeField] private GameObject onHitFX;

    [SerializeField] private Vector3 _moveDirection;

    [SerializeField] private List<ScriptableEffect> _effectsOnHit = new List<ScriptableEffect>();

    private Rigidbody _body;

    private NetworkIdentity _owner;
    private DamageData _damageData;

    [SerializeField] private Transform _unparent;

    [SerializeField] private List<IDamageable> _damagedTargets = new List<IDamageable>();

    private bool _isInited;
    private bool _isNetworkVisual;

    private void Awake()
    {
        _body = GetComponent<Rigidbody>();

        _body.isKinematic = true;
        //Debug.Log("Bullet created");
    }

    private void Update()
    {
        GravityTimer();
        LifeTimer();
    }

    public void Initialize(ProjectileNetworkData networkData)
    {
        _isInited = true;

        _damageData = networkData.damageData;
        _owner = networkData.owner;

        if (networkData.fromProjectilDirection)
            _moveDirection = GetProjectileDirection(networkData.moveDirection);
        else
            _moveDirection = networkData.moveDirection;

        StartMove();
    }

    public void SetupAsNetworkVisual()
    {
        _isNetworkVisual = true;
    }

    void LifeTimer()
    {
        if (_lifetime <= 0)
            NetworkServer.Destroy(gameObject);
        else
            _lifetime -= Time.deltaTime;
    }

    public void AddScriptableEffect(ScriptableEffect newEffect)
    {
        _effectsOnHit.Add(newEffect);
    }

    public void SetDamageData(DamageData damageData)
    {
        _damageData = damageData;
    }

    public void SetOwner(NetworkIdentity newOwner)
    {
        _owner = newOwner;
    }

    public Vector3 GetProjectileDirection(Vector3 to)
    {
        return (to - transform.position).normalized;
    }

    public void StartMove()
    {
        _body.isKinematic = false;
        _body.AddForce(_moveDirection.normalized * _moveSpeed, ForceMode.VelocityChange);

        transform.rotation = Quaternion.LookRotation(_moveDirection.normalized, Vector3.up);
    }

    void GravityTimer()
    {
        if (_delayToGravity <= 0)
            _body.useGravity = true;
        else
            _delayToGravity -= Time.deltaTime;
    }

    void Hit(IDamageable damageable)
    {
        if (_isNetworkVisual) return;

        damageable.TakeDamage(_damageData);

        // Вешаем эффекты которые были на пульке
        foreach (var effect in _effectsOnHit)
        {
            damageable.AddEffect(effect.InitializeEffect(damageable.GetNetworkIdentity(), _damageData));
            NetworkEffectsSync.get.SyncEffect(_owner.netId, effect.effectName, damageable.GetNetworkIdentity().netId);
        }



        if (_damageData == null)
            Debug.LogError("DMG null");
        if (_damageData.whoOwner == null)
            Debug.LogError("dmg data owner null");

        if (_damageData.whoOwner.GetComponent<Pawn>().GetTeam() == EPawnTeam.Player)
        {
            GlobalEvents.OnPlayerHittedDamageable?.Invoke();
        }
    }



    void AOEDamage()
    {
        if (_isNetworkVisual) return;

        var colliders = Physics.OverlapSphere(transform.position, damageRadius).ToList();

        if (colliders.Count == 0) return;

        var ownerPawn = _owner.GetComponent<Pawn>();
        foreach (var col in colliders)
        {
            var pawn = col.transform.root.GetComponent<IDamageable>();

            if (pawn == null) continue;
            if (_damagedTargets.Contains(pawn)) continue;
            if (pawn.GetTeam() == ownerPawn.GetTeam()) continue;

            Hit(pawn);
            _damagedTargets.Add(pawn);
        }
    }

    void CreateOnHitFX()
    {
        if (onHitFX == null)
        {
            Debug.LogError("Projectile had no FX", this);
            return;
        }

        var fx = Instantiate(onHitFX, transform.position, Quaternion.identity);

        _unparent.SetParent(null);
        var parcticles = _unparent.GetComponentsInChildren<ParticleSystem>().ToList();
        parcticles.ForEach(p => p.Stop());

        //NetworkServer.Spawn(fx.gameObject);

        Destroy(_unparent.gameObject, 0.5f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsInited) return;

        if (other == null) return;

        var ownerPawn = _owner.GetComponent<Pawn>();


        switch (damageType)
        {
            case EProjectileDamageType.SingleDamage:
                var damageable = other.transform.gameObject.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    if (_owner == null) Debug.LogError("Projectile owner null", this);
                    if (damageable.GetTeam() == ownerPawn.GetTeam())
                        return;

                    Hit(damageable);
                }
                break;
            case EProjectileDamageType.AOE:
                AOEDamage();
                break;
        }



        CreateOnHitFX();

        if (!immortal)
        {
            //Destroy(gameObject);
            NetworkServer.Destroy(gameObject);
        }
    }
}

public enum EProjectileDamageType
{
    SingleDamage,
    AOE
}

public struct ProjectileNetworkData
{
    public NetworkIdentity owner;
    public NetworkIdentity projectileToSpawn;
    public DamageData damageData;
    public Vector3 moveDirection;
    public Vector3 createPosition;
    public bool fromProjectilDirection;
}
