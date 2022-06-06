using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class BoneRendererHelper : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private Transform rootBone;
    BoneRenderer boneRenderer;
    private void OnValidate()
    {
        if (boneRenderer == null)
            boneRenderer = GetComponent<BoneRenderer>();

        if (rootBone != null)
        {
            if (boneRenderer.transforms.Length == 0)
            {
                var allBones = rootBone.GetComponentsInChildren<Transform>();

                boneRenderer.transforms = allBones;
            }
        }
    }
#endif
}