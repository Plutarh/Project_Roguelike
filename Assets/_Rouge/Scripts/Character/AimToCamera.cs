using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AimToCamera : MonoBehaviour
{
    [SerializeField] private MultiAimConstraint _multiAimConstraint;
    [SerializeField] private Transform _aimTarget;
    [SerializeField] private float _maximumDistanceToAim = 30;
    [SerializeField] private float _weightChangeSpeed = 5;
    [SerializeField] private float _aimSpeed = 6;
    [SerializeField] private float _forwardDistance = 5;
    [SerializeField] private bool _disableAim;

    private PlayerCharacter _playerCharacter;

    private float _dotProd;
    Camera _mainCamera;

    Vector3 _defaultPosition;
    Vector3 _targetPosition;

    float dotOffset = 0.9f;

    private void Awake()
    {
        if (_multiAimConstraint == null)
            _multiAimConstraint = GetComponent<MultiAimConstraint>();

        if (_multiAimConstraint.data.sourceObjects.Count == 0)
            Debug.LogError("No head aim", this);
        else
            _defaultPosition = _aimTarget.localPosition;

        _playerCharacter = transform.root.GetComponent<PlayerCharacter>();

        _mainCamera = Camera.main;
    }

    void Update()
    {
        if (!_playerCharacter.isLocalPlayer) return;

        Aiming();
    }

    private void LateUpdate()
    {

    }

    void Aiming()
    {
        if (_disableAim)
        {
            _multiAimConstraint.weight = 0;
            return;
        }

        if (_aimTarget == null) return;

        _targetPosition = _mainCamera.transform.position + _mainCamera.transform.forward * _forwardDistance;
        _aimTarget.position = Vector3.Lerp(_aimTarget.position, _targetPosition, Time.deltaTime * _aimSpeed);

        // Если камера и игрок смотрят в одну сторону, то заебок, крутим бошку и тд, если смотрят друг на друга, то отключаем повороты бошкой
        Vector3 rootForward = transform.root.TransformDirection(Vector3.forward);
        Vector3 toOther = _mainCamera.transform.position - transform.root.position;

        _dotProd = Vector3.Dot(toOther, rootForward);

        int targetWeight = _dotProd - dotOffset > 0 ? 0 : 1;

        _multiAimConstraint.weight = Mathf.Lerp(_multiAimConstraint.weight, targetWeight, Time.deltaTime * _weightChangeSpeed);
    }

    private void OnDrawGizmos()
    {
        if (_aimTarget != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(_aimTarget.transform.position, 0.2f);
        }
    }
}
