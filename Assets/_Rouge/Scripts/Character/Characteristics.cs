using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class Characteristics : MonoBehaviour
{
    public List<Characteristic> AllCharacteristics
    {
        get => _characteristics;
    }

    [SerializeField] private List<Characteristic> _characteristics = new List<Characteristic>();

    public float GetTypedValue(ECharacteristicType type)
    {
        return _characteristics.FirstOrDefault(ch => ch.characteristicType == type).CurrentValue;
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_characteristics.Count == 0)
        {
            foreach (ECharacteristicType characteristicType in Enum.GetValues(typeof(ECharacteristicType)))
            {
                Characteristic newCharacteristic = new Characteristic();
                newCharacteristic.characteristicType = characteristicType;
                newCharacteristic.charecteristicName = characteristicType.ToString();

                if (_characteristics.Any(ch => ch.characteristicType == characteristicType) == false)
                {
                    _characteristics.Add(newCharacteristic);
                    UnityEditor.EditorUtility.SetDirty(this);
                }
            }
        }
    }
#endif
}

public enum ECharacteristicType
{
    AttackSpeed,
    Damage,
    Defense,
    CritChance,
    MoveSpeed
}
