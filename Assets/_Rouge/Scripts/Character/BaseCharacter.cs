using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class BaseCharacter : Pawn
{   
    [Header("Components")]
    [SerializeField] private Animator _animator;
    [SerializeField] private CharacterController _characterController;

    [Header("Movement")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _sprintSpeed;
    [SerializeField] private float _speedChangeRate;

    [SerializeField] private float _motion;

    [Space]
    [SerializeField] private float _jumpHeight;
    [SerializeField] private float _jumpTimeout = 0.5f;
    [SerializeField] private float _verticalVelocity;
    [SerializeField] private float _gravity = -9.81f;

    [Space]
    [SerializeField] private float _currentMoveSpeed;
    
    [Space]
    [SerializeField] private Vector3 _moveInput;

    [Space]
    [SerializeField] private LayerMask _groundLayers;
    [SerializeField] private bool _isGrounded;
    [SerializeField] private Transform _groundChecker;

    private float _jumpTimeoutDelta;

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
        Movement();
        Rotation();
        JumpTimer();
        Gravity();
        UpdateAnimator();
    }

    public void SetMoveInput(Vector3 _input)
    {
        _moveInput = _input;
    }

    void InitComponents()
    {
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
    }

    // Кастуем сферу под ногами, для нахождения земли
    void GroundCheck()
    {
        Vector3 spherePosition = Vector3.zero;

        // Если есть отдельный трансформ для граунд чека, то юзаем его, если нету, то юзаем обычный трансформ 
        if(_groundChecker != null)
            spherePosition = new Vector3(_groundChecker.position.x,_groundChecker.position.y - 0.15f,_groundChecker.position.z);
        else
            spherePosition = new Vector3(transform.position.x, transform.position.y - 0.15f, transform.position.z);

        _isGrounded = Physics.CheckSphere(spherePosition,0.3f,_groundLayers,QueryTriggerInteraction.Ignore);
    }

    void Movement()
    {
        // TODO возможно добавить спринт
        float targetMoveSpeed = _moveSpeed;

        if(_moveInput == Vector3.zero) targetMoveSpeed = 0;

        // Берем текущую скорость движения, без учета гравитации
        float currentHorizontalSpeed = new Vector3(_characterController.velocity.x,0f,_characterController.velocity.z).magnitude;

        _currentMoveSpeed = Mathf.Lerp(currentHorizontalSpeed,targetMoveSpeed,Time.deltaTime * _speedChangeRate);

        _motion = Mathf.Lerp(0, 1, _currentMoveSpeed / _moveSpeed);
        

        Vector3 targetMovement = _moveInput.normalized * _currentMoveSpeed * Time.deltaTime;
      
        // К нашему движению добавляем вертикальное ускорение, вертикальное ускорение меняется в зависимости от прыжков,падений и тд
        targetMovement += new Vector3(0,_verticalVelocity,0) * Time.deltaTime;

        _characterController.Move(targetMovement);
       
    }

    void Gravity()
    {
        // не уходим в бесконечность
        if(_isGrounded && _verticalVelocity < 0)
            _verticalVelocity = -2f;
        else
            _verticalVelocity += _gravity * Time.deltaTime;
        
    }

    public void TryToJump()
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

    void UpdateAnimator()
    {
        _animator.SetFloat("Motion",_motion);
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
