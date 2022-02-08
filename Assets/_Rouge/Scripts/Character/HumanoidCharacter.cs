using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class HumanoidCharacter : Pawn
{
    [SerializeField] private Animator _animator;
    [SerializeField] private CharacterController _characterController;
    
    public override void Start()
    {
        
    }

   
    public override void Update()
    {
        
    }
}
