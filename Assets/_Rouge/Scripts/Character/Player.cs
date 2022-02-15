using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputService))]
public class Player : BaseCharacter
{
    [SerializeField] private Vector2 _input;

    
    
    private InputService _inputService;

    public override void Awake()
    {
        base.Awake();
      
    }

    public override void Start()
    {
        
    }

   
    public override void Update()
    {
        ReadInput();
        SetMoveInput(_input);
        base.Update();
    }

    public override void InitComponents()
    {
        base.InitComponents();
        _inputService = GetComponent<InputService>();
    }

    void ReadInput()
    {
        _input = _inputService.move;
    }

    public override void Rotation()
    {
        base.Rotation();
    }

    public override void Movement()
    {
        base.Movement();

        // Сначала проверяем бежит ли игрок вперед или назад
        bool forwardMovement = _input.y > 0 ? true : false;

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
      
        if (_input == Vector2.zero) targetMoveSpeed = 0;
       
       
        // Берем текущую скорость движения, без учета гравитации
        float currentHorizontalSpeed = new Vector3(_characterController.velocity.x, 0f, _characterController.velocity.z).magnitude;

        float speedOffset = 0.1f;

        // Для стиков на геймпаде
        float inputMagnitude = _input.magnitude;

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

      
        Vector3 targetMovement = new Vector3(_input.x,0,_input.y).normalized * _currentMoveSpeed * Time.deltaTime;

        // К нашему движению добавляем вертикальное ускорение, вертикальное ускорение меняется в зависимости от прыжков,падений и тд
        targetMovement += new Vector3(0, _verticalVelocity, 0) * Time.deltaTime;

        _characterController.Move(targetMovement);

      
        // Обновляем переменную для бленд движения аниматора 
        if(targetMoveSpeed > 0)
        {
            if(_input.y != 0)
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
        _animator.SetFloat("Motion_X",_input.x);
    }

}
