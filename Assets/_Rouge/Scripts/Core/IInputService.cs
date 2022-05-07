// #if ENABLE_INPUT_SYSTEM
// #endif

using System;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInputService
{
    void OnMove(InputValue value);
    void OnLook(InputValue value);
    void OnJump(InputValue value);
    void OnSprint(InputValue value);
    void MoveInput(Vector2 newMoveDirection);
    void LookInput(Vector2 newLookDirection);
    void JumpInput(bool newJumpState);
    void SprintInput(bool newSprintState);
    void SetCursorState(bool newState);
    void OnFire(InputValue value);


    Vector2 GetMoveInput();
    Vector2 GetLookInput();

    bool IsCurrentDeviceMouse();

    bool GetJump();
    void ResetJump();

    bool GetFire();
    void ResetFire();

    bool GetSprint();

}

