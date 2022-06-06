using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public class InputService : MonoBehaviour//, IInputService
{
    [Header("Character Input Values")]
    [SerializeField] private Vector2 _move;
    [SerializeField] private Vector2 _look;
    [SerializeField] private bool _jump;
    [SerializeField] private bool _sprint;

    [SerializeField] private bool _fire;
    [SerializeField] private bool _secondaryFire;
    [SerializeField] private bool _utility;
    [SerializeField] private bool _ultimate;

    [Header("Movement Settings")]
    [SerializeField] private bool analogMovement;


    [Header("Mouse Cursor Settings")]
    [SerializeField] private bool cursorLocked = true;
    [SerializeField] private bool cursorInputForLook = true;

    [SerializeReference] private PlayerInput _playerInput;

    [SerializeField] private Vector3 _mousePosition;


    public Action<EAttackType> OnAttackButtonClicked;


    public void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>().normalized);
    }

    public void OnLook(InputValue value)
    {
        if (cursorInputForLook)
        {
            LookInput(value.Get<Vector2>());
        }
    }

    public void OnJump(InputValue value)
    {
        JumpInput(value.isPressed);
    }

    public void OnSprint(InputValue value)
    {
        SprintInput(value.isPressed);
    }

    public void MoveInput(Vector2 newMoveDirection)
    {
        _move = newMoveDirection;
    }

    public void LookInput(Vector2 newLookDirection)
    {
        _look = newLookDirection;
    }

    public void JumpInput(bool newJumpState)
    {
        _jump = newJumpState;
    }

    public void SprintInput(bool newSprintState)
    {
        _sprint = newSprintState;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }

    public void OnMousePosition(InputValue value)
    {
        _mousePosition = value.Get<Vector2>();
    }

    public void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }

    public Vector2 GetMoveInput()
    {
        return _move;
    }

    public Vector2 GetLookInput()
    {
        return _look;
    }

    public Vector3 GetMousePosition()
    {
        return _mousePosition;
    }

    public bool GetSprint()
    {
        return _sprint;
    }

    public bool GetJump()
    {
        return _jump;
    }

    public void ResetJump()
    {
        _jump = false;
    }

    public bool IsCurrentDeviceMouse()
    {
        return _playerInput.currentControlScheme == "KeyboardMouse";
    }

    public void OnFire(InputValue value)
    {
        _fire = value.isPressed;
        OnAttackButtonClicked?.Invoke(EAttackType.Primary);
    }

    public bool GetFire()
    {
        return _fire;
    }

    public void ResetFire()
    {
        _fire = false;
    }

    public void OnSecondaryFire(InputValue value)
    {
        _secondaryFire = value.isPressed;
        OnAttackButtonClicked?.Invoke(EAttackType.Secondary);
    }

    public bool GetSecondaryFire()
    {
        return _secondaryFire;
    }

    public void ResetSecondaryFire()
    {
        _secondaryFire = false;
    }

    public void OnUtility(InputValue value)
    {
        _utility = value.isPressed;
        OnAttackButtonClicked?.Invoke(EAttackType.Utility);
    }

    public bool GetUtility()
    {
        return _utility;
    }

    public void ResetUtility()
    {
        _utility = false;
    }

    public void OnUltimate(InputValue value)
    {
        _ultimate = value.isPressed;
        OnAttackButtonClicked?.Invoke(EAttackType.Ultimate);
    }

    public bool GetUltimate()
    {
        return _ultimate;
    }

    public void ResetUltimate()
    {
        _ultimate = false;
    }

}

