using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FireMageDash : BaseAbility
{
    [SerializeField] private GameObject _prepareFX;
    [SerializeField] private GameObject _executeFX;
    [SerializeField] private float _executeTime = 2;
    [SerializeField] private float _dashForce = 10;

    public override void Update()
    {
        base.Update();
    }

    public override void PrepareExecuting(DamageData damageData)
    {
        base.PrepareExecuting(damageData);

        if (_executeFX != null)
        {
            Instantiate(_prepareFX, owner.transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Fire Dash has no prepare FX");
        }

    }

    public override void AddStack()
    {
        base.AddStack();
    }

    public override void Execute()
    {
        // if (!IsReady) return;
        base.Execute();

        if (_executeFX != null)
        {
            Instantiate(_executeFX, owner.transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Fire Dash has no execute FX");
        }

        StartCoroutine(IEDash());
    }

    IEnumerator IEDash()
    {

        float time = _executeTime;
        owner.BlockMovement(true);

        while (time > 0)
        {

            time -= Time.deltaTime;
            owner.CharController.Move(owner.transform.forward * _dashForce);
            yield return new WaitForEndOfFrame();
        }

        owner.BlockMovement(false);
        owner.Animator.CrossFade("Motion", 5f);


        yield break;
    }
}
