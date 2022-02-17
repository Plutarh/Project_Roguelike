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

    [Space]
    [SerializeField] protected float _jumpHeight;
    [SerializeField] protected float _jumpTimeout = 0.5f;
    [SerializeField] protected float _verticalVelocity;
    [SerializeField] protected float _gravity = -9.81f;

    [Space]
    [SerializeField] protected float _currentMoveSpeed;
    
    [Space]
    [SerializeField] protected Vector3 _moveInput;

    [Space]
    [SerializeField] protected LayerMask _groundLayers;
    [SerializeField] protected bool _isGrounded;
    [SerializeField] protected Transform _groundChecker;

    protected float _jumpTimeoutDelta;

   

    public override void Awake()
    {
        InitComponents();
    }

    public override void Start()
    {

    }


    public override void Update()
    {
        GroundCheck();
        Rotation();
        JumpTimer();
        Gravity();
        Movement();
        UpdateAnimator();
     
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
        if(_groundChecker != null)
            spherePosition = new Vector3(_groundChecker.position.x,_groundChecker.position.y - 0.15f,_groundChecker.position.z);
        else
            spherePosition = new Vector3(transform.position.x, transform.position.y - 0.15f, transform.position.z);

        _isGrounded = Physics.CheckSphere(spherePosition,0.3f,_groundLayers,QueryTriggerInteraction.Ignore);

        
    }

    
    public virtual void Movement()
    {
       
    }

    void Gravity()
    {
        // не уходим в бесконечность
        if(_isGrounded )
        {
            _animator.SetBool("FreeFall", false);
            if(_verticalVelocity < 0)
                _verticalVelocity = -2f;
        }
        else
        {
            _animator.SetBool("FreeFall",true);
        }

        _verticalVelocity += _gravity * Time.deltaTime;
    }

    public virtual void TryToJump()
    {
        if (_jumpTimeoutDelta > 0 || !_isGrounded) return;
        Jump();
    }

    public void Jump()
    {
        _verticalVelocity = Mathf.Sqrt(_jumpHeight * -2f * _gravity);

        //_animator.SetTrigger("Jump");
    }

    void JumpTimer()
    {
        if(_jumpTimeoutDelta < 0) return;
        _jumpTimeoutDelta -= Time.deltaTime;
    }

    // Виртуальный метод, потому что повороты игрока зависят от камеры и инпута, ИИ юзают навмеш агента
    public virtual void Rotation()
    {
        
    }

    public virtual void UpdateAnimator()
    {
        //_animator.SetFloat("Motion",_animationMotion);
    }
    
    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.green;

        if(_groundChecker != null)
        {
            Vector3 spherePosition = new Vector3(_groundChecker.position.x, _groundChecker.position.y - 0.15f, _groundChecker.position.z);
            Gizmos.DrawSphere(spherePosition, 0.3f);
        }
       
    }
}
