using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;


public class HeadAim : MonoBehaviour
{
    [SerializeField] MultiAimConstraint multiAimConstraint;
    [SerializeField] private Transform aimTarget;
    [SerializeField] private Transform headAim;
    [SerializeField] private float maximumDistanceToAim = 30;
    [SerializeField] private float weightChangeSpeed = 5;
    [SerializeField] private float headRotationSpeed = 6;
 
    Vector3 defaultPosition;

    Vector3 targetPosition;

    float dotOffset = 0.9f;

    private void Awake() 
    {
        if(multiAimConstraint == null)
            multiAimConstraint = GetComponentInChildren<MultiAimConstraint>();

        if(multiAimConstraint.data.sourceObjects.Count == 0)
            Debug.LogError("No head aim",this);
        else
            defaultPosition = headAim.localPosition;    
    }

    void Update()
    {
        MoveAim();
    }

    private void LateUpdate() 
    {
        Aiming();
    }
  
    public void SetNewAimTarget(Transform target)
    {
        aimTarget = target;
    }

    void Aiming()
    {
        if(aimTarget != null)
        {
            float distance = Vector3.Distance(transform.position,aimTarget.position);

            if(distance > 100)
                targetPosition = defaultPosition;
            else
                headAim.position = aimTarget.position;
        }
        else
        {
            headAim.localPosition = defaultPosition;
        }
    }

    public float dotProd;

    void MoveAim()
    {
        targetPosition = Camera.main.transform.position + Camera.main.transform.forward * 5;
        headAim.position = Vector3.Lerp(headAim.position,targetPosition ,Time.deltaTime * headRotationSpeed);

        // Если камера и игрок смотрят в одну сторону, то заебок, крутим бошку и тд, если смотрят друг на друга, то отключаем повороты бошкой
        Vector3 rootForward = transform.root.TransformDirection(Vector3.forward);
        Vector3 toOther = Camera.main.transform.position - transform.root.position;

        dotProd = Vector3.Dot(toOther,rootForward);

        int targetWeight = dotProd - dotOffset > 0 ? 0 : 1;

        multiAimConstraint.weight = Mathf.Lerp(multiAimConstraint.weight, targetWeight, Time.deltaTime * weightChangeSpeed);
    }

    private void OnDrawGizmos() 
    {
        if(headAim != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(headAim.transform.position,0.2f);
        }
    }
}
