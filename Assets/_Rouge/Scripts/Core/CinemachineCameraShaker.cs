using Cinemachine;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class CinemachineCameraShaker : MonoBehaviour
{
    [SerializeField] private float _shakingTime = 1;
    [SerializeField] private float _shakeResetTime = 0.2f;
    [SerializeField] private float _shakeSmooth;

    [Header("Player Taked Damage")]
    [SerializeField] private float _onHitFrequency;
    [SerializeField] private float _onHitAmplitude;

    [Header("Player Attack")]
    [SerializeField] private float _onAttackFrequency;
    [SerializeField] private float _onAttackAmplitude;

    private CinemachineBasicMultiChannelPerlin _cameraNoise;
    private CinemachineVirtualCamera _virtualCamera;

    private Tween _shakeAmplitudeTween;
    private Tween _shakeFrequencyTween;



    Player _player;

    [Inject]
    public void Construct(Player player)
    {
        _player = player;
    }

    private void Awake()
    {
        GetComponents();
        StopShake();

        _player.Health.OnHealthDecreased += OnPlayerHitShake;
        _player.OnAttackAnimation += OnPlayerAttackShake;
    }

    void GetComponents()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        if (_virtualCamera != null)
            _cameraNoise = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        else
            Debug.LogError("Cant get Cinemachine Perlin Noise Channel");


    }

    public void StopShake()
    {
        if (_cameraNoise == null)
        {
            Debug.LogError("Cannot stop shake, null ref");
            return;
        }

        if (_shakeAmplitudeTween != null)
        {
            _shakeAmplitudeTween.Kill();
            _shakeAmplitudeTween = null;
        }

        if (_shakeFrequencyTween != null)
        {
            _shakeFrequencyTween.Kill();
            _shakeFrequencyTween = null;
        }

        _cameraNoise.m_AmplitudeGain = 0;
        _cameraNoise.m_FrequencyGain = 0;
    }

    public void OnPlayerHitShake(DamageData damageData)
    {
        Shake(_onHitAmplitude, _onHitFrequency);
    }

    public void OnPlayerAttackShake()
    {
        Shake(_onAttackAmplitude, _onAttackFrequency);
    }

    void Shake(float amplitude, float freq)
    {
        if (_virtualCamera == null)
        {
            Debug.LogError("Cant Shake camera, null ref to virtual camera");
            return;
        }

        if (_shakeAmplitudeTween != null)
        {
            _shakeAmplitudeTween.Kill();
            _shakeAmplitudeTween = null;
        }

        _shakeAmplitudeTween = DOTween.To(() => _cameraNoise.m_AmplitudeGain, x => _cameraNoise.m_AmplitudeGain = x, amplitude, _shakeSmooth)
            .OnComplete(() =>
            {
                DOTween.To(() => _cameraNoise.m_AmplitudeGain, x => _cameraNoise.m_AmplitudeGain = x, 0, _shakeResetTime).SetDelay(_shakingTime);
            });



        if (_shakeFrequencyTween != null)
        {
            _shakeFrequencyTween.Kill();
            _shakeFrequencyTween = null;
        }

        _shakeFrequencyTween = DOTween.To(() => _cameraNoise.m_FrequencyGain, x => _cameraNoise.m_FrequencyGain = x, freq, _shakeSmooth)
            .OnComplete(() =>
            {
                DOTween.To(() => _cameraNoise.m_FrequencyGain, x => _cameraNoise.m_FrequencyGain = x, 0, _shakeResetTime).SetDelay(_shakingTime);
            });
    }

    private void OnDestroy()
    {
        _player.Health.OnHealthDecreased -= OnPlayerHitShake;
        _player.OnAttackAnimation -= OnPlayerAttackShake;
    }
}