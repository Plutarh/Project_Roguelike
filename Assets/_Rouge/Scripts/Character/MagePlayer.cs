using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MagePlayer : PlayerCharacter
{
    [SerializeField] private List<Transform> _hands = new List<Transform>();

    public override void Awake()
    {
        base.Awake();

        _primaryAbility.SetAbilityExecutePositions(_hands);
        _secondaryAbility.SetAbilityExecutePositions(_hands);
    }

    public override void AnimPreparePrimaryAttack_1()
    {
        base.AnimPreparePrimaryAttack_1();


    }

    public override void AnimStartPrimaryAttack_1()
    {
        base.AnimStartPrimaryAttack_1();
    }

    public override void AnimStartSecondaryAttack_1()
    {
        base.AnimStartSecondaryAttack_1();
    }

    public override void AnimPrepareSecondaryAttack_1()
    {
        base.AnimPrepareSecondaryAttack_1();
    }

    public override void AnimPrepareUltimateAttack_1()
    {
        base.AnimPrepareUltimateAttack_1();
    }

    public override void AnimStartUltimateAttack_1()
    {
        base.AnimStartUltimateAttack_1();
    }

    public override void AnimPrepareUtilitySkill_1()
    {
        base.AnimPrepareUtilitySkill_1();
    }

    public override void AnimStartUtilitySkill_1()
    {
        base.AnimStartUtilitySkill_1();
    }

}
