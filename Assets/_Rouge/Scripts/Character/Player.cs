using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : BaseCharacter
{
    [SerializeField] private Vector3 _input;

    
    
    public override void Start()
    {
        
    }

   
    public override void Update()
    {
        ReadInput();
        SetMoveInput(_input);
        base.Update();
    }

    void ReadInput()
    {
        _input.z = Input.GetAxis("Vertical");
        _input.x = Input.GetAxis("Horizontal");
    }

    public override void Rotation()
    {
        base.Rotation();
    }

    public override void Movement()
    {
        base.Movement();
        // TODO возможно добавить спринт
        float targetMoveSpeed = _moveSpeed;
        float animationTargetMoveSpeed = _moveSpeed;

        if (_moveInput == Vector3.zero) targetMoveSpeed = 0;

        // Берем текущую скорость движения, без учета гравитации
        float currentHorizontalSpeed = new Vector3(_characterController.velocity.x, 0f, _characterController.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = _moveInput.magnitude;
        float inputZ = _moveInput.z;

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

        _helpMotion = Mathf.Lerp(_helpMotion, targetMoveSpeed, _speedChangeRate * Time.deltaTime);
        if (_helpMotion < 0) _helpMotion = 0;

        _motion = Mathf.Lerp(-1f, 1f, _helpMotion / animationTargetMoveSpeed);

        Vector3 targetMovement = _moveInput.normalized * _currentMoveSpeed * Time.deltaTime;

        // К нашему движению добавляем вертикальное ускорение, вертикальное ускорение меняется в зависимости от прыжков,падений и тд
        targetMovement += new Vector3(0, _verticalVelocity, 0) * Time.deltaTime;

        _characterController.Move(targetMovement);
    }
}
