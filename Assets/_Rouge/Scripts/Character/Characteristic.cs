using UnityEngine;

[System.Serializable]
public class Characteristic
{
    [HideInInspector]
    public string charecteristicName;

    public float CurrentValue
    {
        get => _currentValue;
    }

    public ECharacteristicType characteristicType;
    [SerializeField] private float _currentValue;
    [SerializeField] private float _maxValue;
    [SerializeField] private float _levelIncreaseMultiplier;

    public void IncreaseMaximumValueByLevel(int level)
    {
        _maxValue += level * _levelIncreaseMultiplier;
    }

    public void RefreshCurrentValue()
    {
        _currentValue = _maxValue;
    }

    public void IncreaseCurrentValue(float increaseValue)
    {
        _currentValue += increaseValue;
    }

    public void DecreaseCurrentValue(float decreaseValue)
    {
        _currentValue -= decreaseValue;
    }


}
