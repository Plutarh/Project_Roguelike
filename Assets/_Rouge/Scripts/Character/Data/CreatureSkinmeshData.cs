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

    [SerializeField] private List<Mesh> _skins = new List<Mesh>();
}


