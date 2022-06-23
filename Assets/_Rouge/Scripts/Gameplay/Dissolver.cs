using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Dissolver : NetworkBehaviour
{
    SkinnedMeshRenderer _skinnedMeshRenderer;
    MeshRenderer _meshRenderer;
    Pawn _pawn;
    Material _dissolveMaterial;

    bool _dissolving;

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

        _pawn.OnDeath += Dissolve;

        _dissolveMaterial = Resources.Load<Material>("Dissolve");
    }

    void Dissolve()
    {
        if (_dissolving)
        {
            Debug.LogError("Double call func");
            return;
        }
        // Если нет компонентов для дизолв материала, то просто удалим обьект
        if (_skinnedMeshRenderer == null && _meshRenderer == null)
        {
            Debug.LogError("No component for disolve");

            if (isServer)
                Destroy(gameObject, 3f);

            return;
        }

        StartCoroutine(IEDissolving());
    }

    void TryToDissolve()
    {
        if (isServer)
            RpcDissolve();
    }


    [ClientRpc]
    void RpcDissolve()
    {
        Dissolve();
    }

    IEnumerator IEDissolving()
    {
        _dissolving = true;

        float delay = 4;


        while (delay > 0)
        {
            delay -= Time.deltaTime;
            yield return null;
        }

        Material dissolveMaterial = default;

        if (_skinnedMeshRenderer != null)
            dissolveMaterial = ChangeMaterial(_skinnedMeshRenderer);
        else if (_meshRenderer != null)
            dissolveMaterial = ChangeMaterial(_meshRenderer);
        else
        {
            Debug.LogError("No dissolveMaterial");
            if (isServer)
            {
                Destroy(gameObject);
            }

        }


        dissolveMaterial.SetFloat("_Dissolve", 0);

        float timeToDissolve = 1.3f;
        float dissolveTimer = timeToDissolve;



        while (dissolveTimer > 0)
        {
            dissolveTimer -= Time.deltaTime;

            float dissolving = Mathf.Lerp(1, 0, dissolveTimer / timeToDissolve);

            dissolveMaterial.SetFloat("_Dissolve", dissolving);
            yield return new WaitForEndOfFrame();
        }

        if (isServer)
            Destroy(gameObject);

    }

    Material ChangeMaterial(SkinnedMeshRenderer skinMeshRenderer)
    {
        Texture mainTex = skinMeshRenderer.materials[0].GetTexture("_MainTex");

        var materials = skinMeshRenderer.materials;
        materials[0] = _dissolveMaterial;

        skinMeshRenderer.materials = materials;
        skinMeshRenderer.materials[0].SetTexture("_MainTex", mainTex);

        return skinMeshRenderer.materials[0];
    }

    Material ChangeMaterial(MeshRenderer meshRenderer)
    {
        Texture mainTex = meshRenderer.materials[0].GetTexture("_MainTex");

        meshRenderer.materials[0] = _dissolveMaterial;
        meshRenderer.materials[0].SetTexture("_MainTex", mainTex);

        return meshRenderer.materials[0];
    }



    private void OnDestroy()
    {
        _pawn.OnDeath -= Dissolve;
    }
}
