using System.Linq;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private EProjectileDamageType damageType;
    [SerializeField] private float damageRadius;

    [SerializeField] private float _moveSpeed;

    [SerializeField] private float _delayToGravity = 1.5f;
    [SerializeField] private float _lifetime = 3;

    [Header("FX")]
    [SerializeField] private GameObject onHitFX;

    [SerializeField] private Vector3 _moveDirection;

    private Rigidbody _body;

    private Pawn _owner;
    private DamageData _damageData;

    [SerializeField] private Transform _unparent;

    private void Awake()
    {
        _body = GetComponent<Rigidbody>();

        _body.isKinematic = true;

    }

    private void Update()
    {
        GravityTimer();
        LifeTimer();
    }

    void LifeTimer()
    {
        if (_lifetime <= 0)
            Destroy(gameObject);
        else
            _lifetime -= Time.deltaTime;
    }

    public void SetDamageData(DamageData damageData)
    {
        _damageData = damageData;
    }

    public void SetOwner(Pawn newOwner)
    {
        _owner = newOwner;
    }

    public void SetProjectileDirection(Vector3 to)
    {
        _moveDirection = (to - transform.position).normalized;
    }

    public void StartMove()
    {
        _body.isKinematic = false;
        _body.AddForce(_moveDirection.normalized * _moveSpeed, ForceMode.VelocityChange);
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
        damageable.TakeDamage(_damageData);

        // Если попали в игрока то говорим об этом. В будущем добавить проверку на локального игрока
        if (_damageData == null)
            Debug.LogError("DMG null");
        if (_damageData.whoOwner == null)
            Debug.LogError("dmg data owner null");

        if (_damageData.whoOwner.GetTeam() == EPawnTeam.Player)
        {
            GlobalEvents.OnPlayerHittedDamageable?.Invoke();
        }
    }

    void AOEDamage()
    {
        var colliders = Physics.OverlapSphere(transform.position, damageRadius).ToList();

        if (colliders.Count == 0) return;

        foreach (var col in colliders)
        {
            var pawn = col.transform.root.GetComponent<IDamageable>();

            if (pawn == null) continue;
            if (pawn.GetTeam() == _owner.GetTeam()) continue;
            pawn.TakeDamage(_damageData);
        }
    }

    void CreateOnHitFX()
    {
        if (onHitFX != null)
            Instantiate(onHitFX, transform.position, Quaternion.identity);

        _unparent.SetParent(null);
        var parcticles = _unparent.GetComponentsInChildren<ParticleSystem>().ToList();
        parcticles.ForEach(p => p.Stop());


        Destroy(_unparent.gameObject, 0.5f);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;

        switch (damageType)
        {
            case EProjectileDamageType.SingleDamage:
                IDamageable damageable;
                if (other.transform.TryGetComponent<IDamageable>(out damageable))
                {
                    if (damageable.GetTeam() == _owner.GetTeam()) return;
                    Hit(damageable);
                }
                break;
            case EProjectileDamageType.AOE:
                AOEDamage();
                break;
        }



        CreateOnHitFX();
        Destroy(gameObject);
    }
}

public enum EProjectileDamageType
{
    SingleDamage,
    AOE
}