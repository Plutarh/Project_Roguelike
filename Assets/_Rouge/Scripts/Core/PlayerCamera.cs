using Cinemachine;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class PlayerCamera : MonoBehaviour
{

    [SerializeField] private float _topClamp = 70.0f;
    [SerializeField] private float _bottomClamp = -30.0f;

    [SerializeField] private float _cameraAngleOverride = 0.0f;

    [SerializeField] private bool _lockCameraPosition = false;

    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private Transform _followTarget;

    [SerializeField] private LayerMask cameraCollidersLayer;

    private InputService _inputService;
    private Cinemachine3rdPersonFollow _thirdPersonFollow;


    private const float _threshold = 0.01f;
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    public float rotationMultiplier = 5;

    private PlayerMover _player;

    private CinemachineCameraShaker _camerShaker;

    [Inject]
    public void Construct(PlayerMover player, InputService inputService)
    {
        _player = player;
        _inputService = inputService;
    }

    private void Awake()
    {
        GetComponents();
        Init();
    }


    private void LateUpdate()
    {
        if (_player == null) return;
        CameraRotation();
        // Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 5, Color.red);
    }

    void Init()
    {
        _followTarget = _player.CameraRoot;
        _virtualCamera.Follow = _followTarget;

        _thirdPersonFollow.ShoulderOffset = new Vector3(-0.5f, 0.5f, 0);
        _thirdPersonFollow.CameraDistance = 4.3f;


        _thirdPersonFollow.CameraCollisionFilter = cameraCollidersLayer;


    }

    void GetComponents()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        _thirdPersonFollow = _virtualCamera.AddCinemachineComponent<Cinemachine3rdPersonFollow>();
        _camerShaker = GetComponent<CinemachineCameraShaker>();
    }


    private void CameraRotation()
    {
        if (_inputService.GetLookInput().sqrMagnitude >= _threshold && !_lockCameraPosition)
        {
            // Тутор по персонажу от 3 лица советует не умножать на дельта тайм при клавомыше
            float deltaTimeMultiplier = _inputService.IsCurrentDeviceMouse() ? 1 : Time.deltaTime;

            _cinemachineTargetYaw += _inputService.GetLookInput().x * deltaTimeMultiplier * rotationMultiplier;
            _cinemachineTargetPitch += -_inputService.GetLookInput().y * deltaTimeMultiplier * rotationMultiplier;
        }

        // Огранчиваем повороты до 360 градусов
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, _bottomClamp, _topClamp);


        _player.CameraRoot.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + _cameraAngleOverride, _cinemachineTargetYaw, 0.0f);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void OnDestroy()
    {

    }
}
