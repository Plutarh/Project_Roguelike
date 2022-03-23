using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class BaseCharacter : Pawn
{
    [Header("Components")]
    [SerializeField] protected Animator _animator;
    [SerializeField] protected CharacterController _characterController;

    [Header("Movement")]
    [SerializeField] protected float _moveSpeed;
    [SerializeField] protected float _backwardMoveSpeed;
    [SerializeField] protected float _sprintSpeed;
    [SerializeField] protected float _speedChangeRate;


    [SerializeField] protected float _animationMotion;
    [SerializeField] protected float _gravity = -9.81f;

    [Space]
    [SerializeField] protected float _currentMoveSpeed;

    [Space]
    [SerializeField] protected Vector3 _moveInput;

    [Space]
    [SerializeField] protected LayerMask _groundLayers;
    [SerializeField] protected bool _isGrounded;
    [SerializeField] protected Transform _groundChecker;

    [Space]
    [SerializeField] protected bool _blockMovement;
    [SerializeField] protected bool _battleState;

    protected float _jumpTimeoutDelta;

    protected float _battleStateTimeout = 3;

    public override void Awake()
    {
        base.Awake();
        InitComponents();
    }

    public override void Start()
    {

    }


    public override void Update()
    {
        GroundCheck();
    }

    public void SetMoveInput(Vector3 _input)
    {
        _moveInput = _input;
    }

    public virtual void InitComponents()
    {
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
    }

    // Кастуем сферу под ногами, для нахождения земли
    public virtual void GroundCheck()
    {
        Vector3 spherePosition = Vector3.zero;

        // Если есть отдельный трансформ для граунд чека, то юзаем его, если нету, то юзаем обычный трансформ 
        if (_groundChecker != null)
            spherePosition = new Vector3(_groundChecker.position.x, _groundChecker.position.y - 0.15f, _groundChecker.position.z);
        else
            spherePosition = new Vector3(transform.position.x, transform.position.y - 0.15f, transform.position.z);

        _isGrounded = Physics.CheckSphere(spherePosition, 0.3f, _groundLayers, QueryTriggerInteraction.Ignore);


    }

    public void BlockMovement(bool state)
    {
        _blockMovement = state;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        if (_groundChecker != null)
        {
            Vector3 spherePosition = new Vector3(_groundChecker.position.x, _groundChecker.position.y - 0.15f, _groundChecker.position.z);
            Gizmos.DrawSphere(spherePosition, 0.3f);
        }

    }
}
