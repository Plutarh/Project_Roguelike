using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Player : BaseCharacter
{
    
    private IInputService _inputService;

    [SerializeField] private float _targetRotation;

    [Range(0.0f, 0.3f)]
    [SerializeReference] float _rotationSmoothTime = 0.12f;

    [SerializeReference] private float _rotationVelocity;

    [SerializeReference] private Vector3 _targetDirection;

    public PlayerCamera playerCamera;
    public Transform cameraRoot;

    [Inject]
    public void Construct(IInputService inputService)
    {
        _inputService = inputService;
    }

    public override void Awake()
    {
        base.Awake();
      
    }

    public override void Start()
    {
        
    }

   
    public override void Update()
    {
        SetMoveInput(_inputService.GetMoveInput());
        base.Update();
    }

    public override void InitComponents()
    {
        base.InitComponents();
        _inputService = GetComponent<InputService>();
    }

  
    public override void Rotation()
    {
        base.Rotation();
     
        Vector3 inputDirection = new Vector3(_inputService.GetMoveInput().x, 0.0f, _inputService.GetMoveInput().y).normalized;
       
        if (_inputService.GetMoveInput() != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + playerCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, _rotationSmoothTime);

            // rotate to face input direction relative to camera position
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

      
        //Vector3 targetMovement = new Vector3(_inputService.GetMoveInput().x,0,_inputService.GetMoveInput().y).normalized * _currentMoveSpeed * Time.deltaTime;
        Vector3 targetMovement = _targetDirection.normalized * _currentMoveSpeed * Time.deltaTime;

        // К нашему движению добавляем вертикальное ускорение, вертикальное ускорение меняется в зависимости от прыжков,падений и тд
        targetMovement += new Vector3(0, _verticalVelocity, 0) * Time.deltaTime;

        _characterController.Move(targetMovement);

      
        // Обновляем переменную для бленд движения аниматора 
        if(targetMoveSpeed > 0)
        {
            if(_inputService.GetMoveInput().magnitude != 0)
            {
                if (forwardMovement)
                    _animationMotion = Mathf.Lerp(_animationMotion, 1, currentHorizontalSpeed / _moveSpeed);
                else
                    _animationMotion = Mathf.Lerp(_animationMotion, -1, currentHorizontalSpeed / _backwardMoveSpeed);

            }
            else _animationMotion = 0;
        }
        else
        {
            // Если таргет скорость равна нулю, то просто плавно сбавляем бленд
            _animationMotion = Mathf.Lerp(_animationMotion,0,Time.deltaTime * _speedChangeRate);
        }
    }

    public override void UpdateAnimator()
    {
        _animator.SetFloat("Motion_Y",_animationMotion);
        _animator.SetFloat("Motion_X",_inputService.GetMoveInput().x);
    }

}
