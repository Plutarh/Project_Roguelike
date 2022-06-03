using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitVisualizer : MonoBehaviour
{
    SkinnedMeshRenderer _skinnedMeshRenderer;
    MeshRenderer _meshRenderer;
    Pawn _pawn;

    private void Awake()
    {
        Initialize();
    }

    void Initialize()
    {
        _skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        if (_skinnedMeshRenderer == null)
            _meshRenderer = GetComponentInChildren<MeshRenderer>();

        _pawn = transform.root.gameObject.GetComponent<Pawn>();

        if (_pawn == null)
        {
            Debug.LogError("No pawn for dissolver");
            return;
        }

        _pawn.OnDeath += VisualizeHit;
    }

    void VisualizeHit()
    {
        StartCoroutine(IEVisualizating());
    }

    IEnumerator IEVisualizating()
    {
        MaterialPropertyBlock hit = new MaterialPropertyBlock();

        Color baseColor = Color.cyan;

        if (_skinnedMeshRenderer != null)
            baseColor = _skinnedMeshRenderer.sharedMaterial.GetColor("_BaseColor");
        else if (_meshRenderer != null)
            baseColor = _meshRenderer.sharedMaterial.GetColor("_BaseColor");

        

        yield return null;
    }
}
