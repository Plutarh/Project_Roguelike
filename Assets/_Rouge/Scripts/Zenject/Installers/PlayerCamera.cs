using Cinemachine;
using UnityEngine;
using Zenject;

public class PlayerCamera : MonoBehaviour
{
    [SerializeReference] private CinemachineVirtualCamera virtualCamera;

  
    [SerializeReference] private Transform _followTarget;

    private void Awake() 
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        Init();
    }

    public void Construct(Player player)
    {
        _followTarget = player.transform;
    }

    void Init()
    {
        virtualCamera.Follow = _followTarget;
        
    }
}