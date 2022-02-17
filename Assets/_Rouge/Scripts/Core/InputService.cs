using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public class InputService : MonoBehaviour , IInputService 
{
    [Header("Character Input Values")]
    [SerializeField] private Vector2 _move;
    [SerializeField] private Vector2 _look;
    [SerializeField] private bool _jump;
    [SerializeField] private bool _sprint;

    [Header("Movement Settings")]
    [SerializeField] private bool analogMovement;


    [Header("Mouse Cursor Settings")]
    [SerializeField] private bool cursorLocked = true;
    [SerializeField] private bool cursorInputForLook = true;

    [SerializeReference] private PlayerInput _playerInput;


    public void OnMove(InputValue value) 
    {
        MoveInput(value.Get<Vector2>());
    }

    public void OnLook(InputValue value)
    {
        if(cursorInputForLook)
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
}

