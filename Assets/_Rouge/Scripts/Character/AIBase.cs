using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBase : BaseCharacter
{
    private RagdollController ragdollController;

    public override void Awake()
    {
        base.Awake();

        ragdollController = GetComponent<RagdollController>();
    }



    public override void Death()
    {
        base.Death();
        ragdollController.EnableRagdoll();
    }
}
