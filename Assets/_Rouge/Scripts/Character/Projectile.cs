using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private DamageData damageData;
    [SerializeField] private float _moveSpeed;

    [SerializeField] private float _delayToGravity = 1.5f;
    [SerializeField] private float _lifetime = 3;

    [Header("FX")]
    [SerializeField] private GameObject onHitFX;

    [SerializeField] private Vector3 _moveDirection;

    private Rigidbody _body;

    private Pawn _owner;

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
        // TODO remove DEBUG
        damageData = new DamageData();
        damageData.combatValue = 25;
        damageData.whoOwner = _owner;

        damageable.TakeDamage(damageData);

        if (onHitFX != null)
            Instantiate(onHitFX, transform.position, Quaternion.identity);

        Destroy(gameObject);

        // Если попали в игрока то говорим об этом. В будущем добавить проверку на локального игрока
        if (damageData.whoOwner.GetTeam() == EPawnTeam.Player)
        {
            GlobalEvents.OnPlayerHittedDamageable?.Invoke();
        }

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;

        Debug.Log($"Hit with {other.transform.name}");
        IDamageable damageable;
        if (other.transform.TryGetComponent<IDamageable>(out damageable))
        {
            if (damageable.GetTeam() == _owner.GetTeam()) return;
            Hit(damageable);
        }
    }
}
