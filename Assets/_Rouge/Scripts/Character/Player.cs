using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
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
    [SerializeField] private float _rotationSmoothTime = 0.12f;
    [SerializeField] private float _rotationVelocity;
    [SerializeField] private Vector3 _targetDirection;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Transform _cameraRoot;


    [Header("Combat")]
    [SerializeField] private float _comboAttackTimeout = 0.8f;
    [SerializeField] private float _lastPrimaryAttackTime = 0;
    [SerializeField] private string _currentCombatName;
    public int currentPrimaryAttackIndex = 0;

    [Header("Other")]
    public AvatarMask attackMask;
    public CharacterAnimationData characterAnimationData;

    public List<AnimationClipData> combatAnimationsQueue = new List<AnimationClipData>();



    [Inject]
    public void Construct(IInputService inputService)
    {
        _inputService = inputService;
    }

    public override void Awake()
    {
        base.Awake();
        _mainCamera = Camera.main;

        InputEvents.OnFireClicked += IncreasePrimaryAttackClickCount;
        InputEvents.OnAttackButtonClicked += OnAttackButtonClicked;

        ResetPrimaryAttack();
        _animator.SetLayerWeight(1, 1);
    }

    public override void Start()
    {

        foreach (AvatarMaskBodyPart bodyPart in Enum.GetValues(typeof(AvatarMaskBodyPart)))
        {

        }
    }

    public override void Update()
    {
        base.Update();

        PrimaryAttackResetTimer();
        TryToJump();
        SetMoveInput(_inputService.GetMoveInput());
        TryToPrimaryAttack();

    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void InitComponents()
    {
        base.InitComponents();
        _inputService = GetComponent<InputService>();
    }

    void OnAttackButtonClicked(EAttackType type)
    {
        switch (type)
        {
            case EAttackType.Primary:
                break;
            case EAttackType.Secondary:
                break;
            case EAttackType.Utility:
                break;
            case EAttackType.Ultimate:
                break;
        }
    }

    void IncreasePrimaryAttackClickCount()
    {
        TryToPrimaryAttack();
    }

    float GetAttackLayerAnimationTime()
    {
        if (_animator.GetCurrentAnimatorStateInfo(1).IsName(_currentCombatName))
            return _animator.GetCurrentAnimatorStateInfo(1).normalizedTime;
        else
            return 0;
    }

    void PrimaryAttackResetTimer()
    {
        if (Time.time - _lastPrimaryAttackTime > _comboAttackTimeout)
        {
            if (currentPrimaryAttackIndex >= 0)
            {
                //_animator.SetLayerWeight(1, 0);
            }

            ResetPrimaryAttack();
        }
    }

    void TryToPrimaryAttack()
    {
        if (_inputService.GetFire() == false) return;
        PrimaryAttack();
    }

    void PrimaryAttack()
    {
        _inputService.ResetFire();

        if (currentPrimaryAttackIndex < 0)
        {
            currentPrimaryAttackIndex = 0;
        }
        else
        {
            if (GetAttackLayerAnimationTime() > 0.65f)
                currentPrimaryAttackIndex++;
            else
                return;
        }


        _lastPrimaryAttackTime = Time.time;

        AnimationClipData nextAnimationClip = new AnimationClipData();

        nextAnimationClip = characterAnimationData.GetAnimationClip(EAttackType.Primary, ref currentPrimaryAttackIndex);

        _comboAttackTimeout = nextAnimationClip.GetTimerToNextCombo;

        // BlockMovement(nextAnimationClip.IsStopMovement);
        // attackMask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.Root, nextAnimationClip.IsAllowRootRotation);
        // attackMask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.LeftLeg, nextAnimationClip.IsAllowRootRotation);
        // attackMask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.RightLeg, nextAnimationClip.IsAllowRootRotation);


        if (nextAnimationClip.IsAllowRootRotation)
        {
            //_animator.SetLayerWeight(1, 0);
        }
        else
        {
            //_animator.SetLayerWeight(1, 1);
        }



        _animator.CrossFade(nextAnimationClip.GetAnimationName, nextAnimationClip.GetCrossFadeTime, nextAnimationClip.IsAllowRootRotation ? 0 : 1);
        _currentCombatName = nextAnimationClip.GetAnimationName;


    }

    void ResetPrimaryAttack()
    {
        currentPrimaryAttackIndex = -1;

        // BlockMovement(false);
        // attackMask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.Root, false);
        // attackMask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.LeftLeg, false);
        // attackMask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.RightLeg, false);
    }

    void ChangeAttackLayerWeightSmooth(float targetWeight)
    {
        float currentWeight = _animator.GetLayerWeight(1);

        DOTween.To(() => currentWeight, x => currentWeight = x, targetWeight, 1).OnUpdate(() =>
        {
            _animator.SetLayerWeight(1, currentWeight);
        });
    }



    public override void Rotation()
    {
        if (_blockMovement) return;

        base.Rotation();

        Vector3 inputDirection = new Vector3(_inputService.GetMoveInput().x, 0.0f, _inputService.GetMoveInput().y).normalized;

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
        if (_blockMovement) return;

        base.Movement();

        // Сначала проверяем бежит ли игрок вперед или назад
        bool forwardMovement = _inputService.GetMoveInput().y > 0 ? true : false;

        float targetMoveSpeed = 0;

        if (forwardMovement)
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
        Vector3 targetDir = new Vector3(_inputService.GetMoveInput().x, 0, _inputService.GetMoveInput().y);
        targetDir = _mainCamera.transform.TransformDirection(targetDir);
        targetDir = Vector3.ProjectOnPlane(targetDir, Vector3.up);

        Vector3 targetMovement = targetDir.normalized * _currentMoveSpeed * Time.deltaTime;

        // К нашему движению добавляем вертикальное ускорение, вертикальное ускорение меняется в зависимости от прыжков,падений и тд
        targetMovement += new Vector3(0, _verticalVelocity, 0) * Time.deltaTime;

        _characterController.Move(targetMovement);

        // Обновляем переменную для бленд движения аниматора 
        if (targetMoveSpeed > 0)
        {
            if (_inputService.GetMoveInput().magnitude != 0)
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
            _animationMotion = Mathf.Lerp(_animationMotion, 0, Time.deltaTime * _speedChangeRate);
        }
    }

    public override void TryToJump()
    {
        if (_inputService.GetJump() == false) return;
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

            if (_fallTimeoutDelta > 0)
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

        _animator.SetBool("Land", _isGrounded);
    }

    public override void UpdateAnimator()
    {
        _animator.SetFloat("Motion_Y", _animationMotion);
        _animator.SetFloat("Motion_X", _inputService.GetMoveInput().x);
    }


    private void OnDestroy()
    {
        InputEvents.OnFireClicked -= IncreasePrimaryAttackClickCount;
        InputEvents.OnAttackButtonClicked -= OnAttackButtonClicked;
    }
}

