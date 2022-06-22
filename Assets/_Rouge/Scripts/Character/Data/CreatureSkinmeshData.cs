using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Creature Skinmesh data", menuName = "Character/Creature Skin", order = 52)]
public class CreatureSkinmeshData : ScriptableObject
{
    public Mesh RandomSkinMesh
    {
        get
        {
            if (_skins.Count == 0)
            {
                Debug.LogError("Skins count equal 0", this);
                return null;
            }
            return _skins[Random.Range(0, _skins.Count)];
        }
    }

    public Mesh GetSkinMeshByIndex(int index)
    {
        if (_skins.Count == 0 || index >= _skins.Count)
        {
            Debug.LogError("Skins count equal 0 or out of range", this);
            return null;
        }
        return _skins[index];
    }

    public List<Mesh> Skins => _skins;


    [SerializeField] private List<Mesh> _skins = new List<Mesh>();
}


