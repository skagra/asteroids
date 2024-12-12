using UnityEngine;

public class AudioHub : MonoBehaviour
{
    [Header("General Settings")]
    public bool _isSoundEnabled;

    [Header("Sounds")]
    [SerializeField]
    private AudioClip _fireAudioClip;
    [SerializeField]
    private AudioClip _thrustAudioClip;
    [SerializeField]
    private AudioClip _largeExplosionAudioClip;
    [SerializeField]
    private AudioClip _mediumExplosionAudioClip;
    [SerializeField]
    private AudioClip _smallExplosionAudioClip;
    [SerializeField]
    private AudioClip _extraShipAudioClip;
    [SerializeField]
    private AudioClip _beatHighAudioClip;
    [SerializeField]
    private AudioClip _beatLowAudioClip;

    [Header("Beat Rhythm")]
    [SerializeField]
    [Range(0.1f, 1.0f)]
    private float _beatGapMin;
    [SerializeField]
    [Range(0.1f, 1.0f)]
    private float _beatGapMax;
    [SerializeField]
    [Range(5, 20)]
    private int _beatsBeforeSpeedUp;
    [SerializeField]
    [Range(0.1f, 0.5f)]
    private float _beatGapSpeedUpDelta;

    private AudioSource _fireAudioSource;
    private AudioSource _thrustAudioSource;
    private AudioSource _largeExplosionAudioSource;
    private AudioSource _mediumExplosionAudioSource;
    private AudioSource _smallExplosionAudioSource;
    private AudioSource _beatLowAudioSource;
    private AudioSource _beatHighAudioSource;
    private AudioSource _extraShipAudioSource;

    private float _beatGapTimer = 0f;
    private bool _beatToggle = false;

    private float _beatGapCurrent;
    private int _beatCount = 0;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        _beatGapCurrent = _beatGapMax;

        _fireAudioSource = gameObject.AddComponent<AudioSource>();
        _fireAudioSource.clip = _fireAudioClip;
        _thrustAudioSource = gameObject.AddComponent<AudioSource>();
        _thrustAudioSource.clip = _thrustAudioClip;
        _largeExplosionAudioSource = gameObject.AddComponent<AudioSource>();
        _largeExplosionAudioSource.clip = _largeExplosionAudioClip;
        _mediumExplosionAudioSource = gameObject.AddComponent<AudioSource>();
        _mediumExplosionAudioSource.clip = _mediumExplosionAudioClip;
        _smallExplosionAudioSource = gameObject.AddComponent<AudioSource>();
        _smallExplosionAudioSource.clip = _smallExplosionAudioClip;
        _beatLowAudioSource = gameObject.AddComponent<AudioSource>();
        _beatLowAudioSource.clip = _beatLowAudioClip;
        _beatHighAudioSource = gameObject.AddComponent<AudioSource>();
        _beatHighAudioSource.clip = _beatHighAudioClip;
        _extraShipAudioSource = gameObject.AddComponent<AudioSource>();
        _extraShipAudioSource.clip = _extraShipAudioClip;
    }

    private void Update()
    {
        if (_isSoundEnabled)
        {
            if (!_beatLowAudioSource.isPlaying && !_beatHighAudioSource.isPlaying)
            {
                _beatGapTimer += Time.deltaTime;

                if (_beatGapTimer > _beatGapCurrent)
                {
                    _beatToggle = !_beatToggle;

                    if (_beatToggle)
                    {
                        _beatLowAudioSource.Play();
                    }
                    else
                    {
                        _beatHighAudioSource.Play();
                    }

                    if (_beatGapCurrent > _beatGapMin)
                    {
                        _beatCount++;

                        if (_beatCount > _beatsBeforeSpeedUp)
                        {
                            _beatCount = 0;
                            _beatGapCurrent -= _beatGapSpeedUpDelta;
                        }
                    }
                }
            }
            else
            {
                _beatGapTimer = 0f;
            }
        }
    }

    public void ResetBeats()
    {
        _beatGapCurrent = _beatGapMax;
        _beatCount = 0;
    }

    public void PlayFire()
    {
        if (_isSoundEnabled && !_fireAudioSource.isPlaying)
        {
            _fireAudioSource.Play();
        }
    }

    public void PlayThrust()
    {
        if (_isSoundEnabled && !_thrustAudioSource.isPlaying)
        {
            _thrustAudioSource.Play();
        }
    }

    public void PlayLargeExplosion()
    {
        if (_isSoundEnabled)
        {
            _largeExplosionAudioSource.Play();
        }
    }

    public void PlayMediumExplosion()
    {
        if (_isSoundEnabled)
        {
            _mediumExplosionAudioSource.Play();
        }
    }

    public void PlaySmallExplosion()
    {
        if (_isSoundEnabled)
        {
            _smallExplosionAudioSource.Play();
        }
    }

    public void PlayExtraShip()
    {
        if (_isSoundEnabled && !_extraShipAudioSource.isPlaying)
        {
            _extraShipAudioSource.Play();
        }
    }
}
