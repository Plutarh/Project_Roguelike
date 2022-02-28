using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Zenject;

public class Player : BaseCharacter
{
    public Transform GetCameraRoot
    {
        get => _cameraRoot;
    }

    private IInputService _inputService;

    [SerializeField] private float _targetRotation;

    [Range(0.0f, 0.3f)]
    [SerializeReference] float _rotationSmoothTime = 0.12f;

    [SerializeReference] private float _rotationVelocity;

    [SerializeReference] private Vector3 _targetDirection;

    [SerializeField] private  Camera _mainCamera;
    [SerializeField] private Transform _cameraRoot;

    
    private float _comboAttackDelta;
    private float _comboAttackTimeout = 0.8f;

    [SerializeField] private int _fireClickCount;

    public AvatarMask attackMask;


    public CharacterAnimationData characterAnimationData;
  

    [Inject]
    public void Construct(IInputService inputService)
    {
        _inputService = inputService;
        
    }

    public override void Awake()
    {
        base.Awake();
        _mainCamera = Camera.main;

        InputEvents.OnFireClicked += IncreaseFireClickCount;
    }

    public override void Start()
    {
      
        foreach (AvatarMaskBodyPart bodyPart in Enum.GetValues(typeof(AvatarMaskBodyPart)))
        {
            
        }
    }
    
    public override void Update()
    {
        TryToJump();
        SetMoveInput(_inputService.GetMoveInput());
        TryToPrimaryAttack();
        base.Update();

        FireClickTimer();

    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    float lastFireClickedTime = 0;

    void IncreaseFireClickCount()
    {
        lastFireClickedTime = Time.time;
        _fireClickCount++;

        _fireClickCount = Mathf.Clamp(_fireClickCount,0,3);

        TryToPrimaryAttack();
    }

    float GetAttackLayerAnimationTime()
    {
        return _animator.GetCurrentAnimatorStateInfo(1).normalizedTime;
    }

    void FireClickTimer()
    {
        if(Time.time - lastFireClickedTime > _comboAttackTimeout)
        {
            _fireClickCount = 0;
            
            _animator.SetBool("Primary Attack 2", false);
            _animator.SetBool("Primary Attack 3", false);
        }
    }

    void TryToPrimaryAttack()
    {
        if(_inputService.GetFire() == false) return;
        PrimaryAttack();
    }

    public List<AnimationClipData> combatAnimationsQueue = new List<AnimationClipData>();

    void PrimaryAttack()
    {
        _inputService.ResetFire();

        combatAnimationsQueue.Add(characterAnimationData.GetCombatAnimations.FirstOrDefault(ca => combatAnimationsQueue.Where(caq => combatAnimationsQueue.Contains(ca) == false) ))
        //_animator.SetLayerWeight(1,1);
        //_animator.SetTrigger("Melee Attack 1");
        //attackMask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.Root,false);
        //StartCoroutine(IEResetAttack());
        
    }

    IEnumerator IEResetAttack()
    {
        yield return new WaitForEndOfFrame();
      
        _animator.ResetTrigger("Primary Attack 1");
    }

    public override void InitComponents()
    {
        base.InitComponents();
        _inputService = GetComponent<InputService>();
    }

    public override void Rotation()
    {
        base.Rotation();
     
        Vector3 inputDirection = new Vector3(_inputService.GetMoveInput().x, 0.0f,_inputService.GetMoveInput().y).normalized;
       
        if (_inputService.GetMoveInput() != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;

            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, _rotationSmoothTime);

            // Поворачиваем перса под угол камеры
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        _targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
    }

    public override void Movement()
    {
        base.Movement();

        // Сначала проверяем бежит ли игрок вперед или назад
        bool forwardMovement = _inputService.GetMoveInput().y > 0 ? true : false;

        float targetMoveSpeed = 0;

        if(forwardMovement)
        {
            // TODO добавить спринт скорость в зависимости от инпут кнопки спринта
            targetMoveSpeed = _moveSpeed;
        }
        else
        {
            targetMoveSpeed = _backwardMoveSpeed;
        }
      
        if (_inputService.GetMoveInput() == Vector2.zero) targetMoveSpeed = 0;
       
       
        // Берем текущую скорость движения, без учета гравитации
        float currentHorizontalSpeed = new Vector3(_characterController.velocity.x, 0f, _characterController.velocity.z).magnitude;

        float speedOffset = 0.1f;

        // Для стиков на геймпаде
        float inputMagnitude = _inputService.GetMoveInput().magnitude;

        if (currentHorizontalSpeed < targetMoveSpeed - speedOffset || currentHorizontalSpeed > targetMoveSpeed + speedOffset)
        {
            _currentMoveSpeed = Mathf.Lerp(currentHorizontalSpeed, targetMoveSpeed * inputMagnitude, _speedChangeRate * Time.deltaTime);

            // Округляем, до более нормальных чисел
            _currentMoveSpeed = Mathf.Round(_currentMoveSpeed * 1000f) / 1000f;
        }
        else
        {
            _currentMoveSpeed = targetMoveSpeed;
        }

        // Игрок будет двигаться по форварду камеры
        Vector3 targetDir = new Vector3(_inputService.GetMoveInput().x,0,_inputService.GetMoveInput().y);
        targetDir = _mainCamera.transform.TransformDirection(targetDir);
        targetDir = Vector3.ProjectOnPlane(targetDir,Vector3.up);
        
        Vector3 targetMovement = targetDir.normalized * _currentMoveSpeed * Time.deltaTime;

        // К нашему движению добавляем вертикальное ускорение, вертикальное ускорение меняется в зависимости от прыжков,падений и тд
        targetMovement += new Vector3(0, _verticalVelocity, 0) * Time.deltaTime;

        _characterController.Move(targetMovement);
      
        // Обновляем переменную для бленд движения аниматора 
        if(targetMoveSpeed > 0)
        {
            if(_inputService.GetMoveInput().magnitude != 0)
            {
                _animationMotion = Mathf.Lerp(_animationMotion, 1, currentHorizontalSpeed / _moveSpeed);
                // if (forwardMovement)
                //     _animationMotion = Mathf.Lerp(_animationMotion, 1, currentHorizontalSpeed / _moveSpeed);
                // else
                //     _animationMotion = Mathf.Lerp(_animationMotion, -1, currentHorizontalSpeed / _backwardMoveSpeed);

            }
            else _animationMotion = 0;
        }
        else
        {
            // Если таргет скорость равна нулю, то просто плавно сбавляем бленд
            _animationMotion = Mathf.Lerp(_animationMotion,0,Time.deltaTime * _speedChangeRate);
        }
    }

    public override void TryToJump()
    {
        if(_inputService.GetJump() == false) return;
        if (_jumpTimeoutDelta > 0 || !_isGrounded) return;

        Jump();
    }

    public override void Jump()
    {
        base.Jump();

        _animator.SetBool("Jump", true);
        _verticalVelocity = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
    }

    public override void Gravity()
    {
        base.Gravity();

       
        if (_isGrounded)
        {
            _animator.SetBool("FreeFall", false);
            _animator.SetBool("Jump", false);

            _fallTimeoutDelta = _fallTimeout;

            // не уходим в бесконечность
            if (_verticalVelocity < 0)
                _verticalVelocity = -2f;

            // Откатываем кд прыжка
            if (_jumpTimeoutDelta >= 0.0f)
                _jumpTimeoutDelta -= Time.deltaTime;
        }
        else
        {
            _jumpTimeoutDelta = _jumpTimeout;

            if(_fallTimeoutDelta > 0)
                _fallTimeoutDelta -= Time.deltaTime;
            else
                _animator.SetBool("FreeFall", true);

            _inputService.ResetJump();
        }

        _verticalVelocity += _gravity * Time.deltaTime;
    }

    public override void GroundCheck()
    {
        base.GroundCheck();

        _animator.SetBool("Land",_isGrounded);
    }

    public override void UpdateAnimator()
    {
        _animator.SetFloat("Motion_Y",_animationMotion);
        _animator.SetFloat("Motion_X",_inputService.GetMoveInput().x);
       
    }


    private void OnDestroy() 
    {
        InputEvents.OnFireClicked -= IncreaseFireClickCount;
    }
}

