using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AimToCamera : MonoBehaviour
{
    [SerializeField] MultiAimConstraint multiAimConstraint;
    [SerializeField] private Transform aimTarget;
    [SerializeField] private float maximumDistanceToAim = 30;
    [SerializeField] private float weightChangeSpeed = 5;
    [SerializeField] private float aimSpeed = 6;
    [SerializeField] private float forwardDistance = 5;

    Camera mainCamera;
    Vector3 defaultPosition;

    Vector3 targetPosition;

    float dotOffset = 0.9f;

    private void Awake()
    {
        if (multiAimConstraint == null)
            multiAimConstraint = GetComponent<MultiAimConstraint>();

        if (multiAimConstraint.data.sourceObjects.Count == 0)
            Debug.LogError("No head aim", this);
        else
            defaultPosition = aimTarget.localPosition;

        mainCamera = Camera.main;
    }

    void Update()
    {
        Aiming();
    }

    private void LateUpdate()
    {
        
    }

   

    // void Aiming()
    // {
    //     if (aimTarget != null)
    //     {
    //         float distance = Vector3.Distance(transform.position, aimTarget.position);

    //         if (distance > 100)
    //             targetPosition = defaultPosition;
    //         else
    //             aimTarget.position = aimTarget.position;
    //     }
    //     else
    //     {
    //         aimTarget.localPosition = defaultPosition;
    //     }
    // }

    public float dotProd;

    void Aiming()
    {
        if(aimTarget == null) return;
        
        targetPosition = mainCamera.transform.position + mainCamera.transform.forward * forwardDistance;
        aimTarget.position = Vector3.Lerp(aimTarget.position, targetPosition, Time.deltaTime * aimSpeed);

        // Если камера и игрок смотрят в одну сторону, то заебок, крутим бошку и тд, если смотрят друг на друга, то отключаем повороты бошкой
        Vector3 rootForward = transform.root.TransformDirection(Vector3.forward);
        Vector3 toOther = mainCamera.transform.position - transform.root.position;

        dotProd = Vector3.Dot(toOther, rootForward);

        int targetWeight = dotProd - dotOffset > 0 ? 0 : 1;

        multiAimConstraint.weight = Mathf.Lerp(multiAimConstraint.weight, targetWeight, Time.deltaTime * weightChangeSpeed);
    }

    private void OnDrawGizmos()
    {
        if (aimTarget != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(aimTarget.transform.position, 0.2f);
        }
    }
}
