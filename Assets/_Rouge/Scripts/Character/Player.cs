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
}
