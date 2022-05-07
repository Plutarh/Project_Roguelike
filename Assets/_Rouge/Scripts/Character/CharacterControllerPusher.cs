using UnityEngine;

public class CharacterControllerPusher : MonoBehaviour
{
    CharacterController _characterController;
    [SerializeField] private Vector3 _currentVelocity;
    [SerializeField] private Vector3 _additionlVelocity;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        AdditionalMotionLogic();
    }

    // TODO если смотрят друг на друга, то стопаем движение
    public void Impact(DamageData damageData)
    {
        Vector3 dirToPush = transform.position - damageData.whoOwner.transform.position;

        _currentVelocity = _characterController.velocity;
        _additionlVelocity = damageData.combatValue * dirToPush.normalized + _currentVelocity + Vector3.up;


    }

    void AdditionalMotionLogic()
    {
        if (_characterController == null)
        {
            Debug.LogError($"Character controller NULL on {name}", this);
            return;
        }

        if (_additionlVelocity.sqrMagnitude > 0.3f)
            _characterController.Move(_additionlVelocity * Time.deltaTime);
        _additionlVelocity = Vector3.Lerp(_additionlVelocity, Vector3.zero, Time.deltaTime * 6);
    }
}
