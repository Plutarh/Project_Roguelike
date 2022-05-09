using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/BaseAbility")]
public class AbilityScriptable : ScriptableObject
{
    public AbilityData abilityData;

    public BaseAbility abilityComponent;
}
