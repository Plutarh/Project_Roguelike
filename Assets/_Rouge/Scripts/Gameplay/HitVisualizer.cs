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

        _pawn.Health.OnHealthDecreased += VisualizeHit;
    }

    void VisualizeHit(DamageData damageData)
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

        float colorMultiplier = 2.2f;

        float timeIn = 0.08f;
        float timerIn = timeIn;
        Color visualColor = baseColor;

        while (timerIn > 0)
        {
            timerIn -= Time.deltaTime;
            visualColor = Color.Lerp(baseColor * colorMultiplier, baseColor, timerIn / timeIn);
            hit.SetColor("_BaseColor", visualColor);

            if (_skinnedMeshRenderer != null)
                _skinnedMeshRenderer.SetPropertyBlock(hit);
            else if (_meshRenderer != null)
                _meshRenderer.SetPropertyBlock(hit);

            yield return new WaitForEndOfFrame();
        }

        float timeOut = 0.1f;
        float timerOut = timeOut;

        while (timerOut > 0)
        {
            timerOut -= Time.deltaTime;
            visualColor = Color.Lerp(baseColor, baseColor * colorMultiplier, timerOut / timeOut);
            hit.SetColor("_BaseColor", visualColor);

            if (_skinnedMeshRenderer != null)
                _skinnedMeshRenderer.SetPropertyBlock(hit);
            else if (_meshRenderer != null)
                _meshRenderer.SetPropertyBlock(hit);

            yield return new WaitForEndOfFrame();
        }


    }

    private void OnDestroy()
    {
        _pawn.Health.OnHealthDecreased -= VisualizeHit;
    }
}
