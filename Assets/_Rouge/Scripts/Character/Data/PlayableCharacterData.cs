using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "New Playable Character Data", menuName = "Character/Animations", order = 51)]
public class PlayableCharacterData : ScriptableObject
{
    [SerializeField] private string _characterName;
    [SerializeField] private string _charaterDescription;
    [SerializeField] private Sprite _characterIcon;

    public CharacterAnimationData _animationData;
}


