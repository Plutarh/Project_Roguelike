using Cinemachine;
using UnityEngine;
using Zenject;

public class PlayerCamera : MonoBehaviour
{
   
    [SerializeField] private float _topClamp = 70.0f;
    [SerializeField] private float _bottomClamp = -30.0f;

    [SerializeField] private float _cameraAngleOverride = 0.0f;

    [SerializeField] private bool _lockCameraPosition = false;

    [SerializeReference] private CinemachineVirtualCamera _virtualCamera;
    [SerializeReference] private Transform _followTarget;

    private IInputService _inputService;
    private Cinemachine3rdPersonFollow _thirdPersonFollow;

    private const float _threshold = 0.01f;
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    public float multiplyer = 5;

    Player _player;

    private void Awake() 
    {
        Init();
    }

    [Inject]
    public void Construct(Player player, IInputService inputService)
    {
        _player = player;
        _inputService = inputService;
    }

    private void LateUpdate() 
    {
        CameraRotation();
    }

    void Init()
    {
        GetComponents();

        _followTarget = _player.cameraRoot.transform;
        _virtualCamera.Follow = _followTarget;
        
        _thirdPersonFollow.ShoulderOffset = new Vector3(-0.5f,0,0);
        _thirdPersonFollow.CameraDistance = 3;

        _player.playerCamera = this;
    }

    void GetComponents()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        _thirdPersonFollow = _virtualCamera.AddCinemachineComponent<Cinemachine3rdPersonFollow>();
    }

    private void CameraRotation()
    {
        if (_inputService.GetLookInput().sqrMagnitude >= _threshold && !_lockCameraPosition)
        {
            // Тутор по персонажу от 3 лица советует не умножать на дельта тайм при клавомыше
            float deltaTimeMultiplier = _inputService.IsCurrentDeviceMouse() ? 1 : Time.deltaTime;

            _cinemachineTargetYaw += _inputService.GetLookInput().x * deltaTimeMultiplier * multiplyer;
            _cinemachineTargetPitch += -_inputService.GetLookInput().y * deltaTimeMultiplier * multiplyer;
        }

        // Огранчиваем повороты до 360 градусов
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, _bottomClamp, _topClamp);


        _player.cameraRoot.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + _cameraAngleOverride, _cinemachineTargetYaw, 0.0f);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}