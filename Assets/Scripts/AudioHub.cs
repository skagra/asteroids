using UnityEngine;

public class AudioHub : MonoBehaviour
{
    public static AudioHub Instance { get; private set; }

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
    private AudioClip _beatHighAudioClip;

    [SerializeField]
    private AudioClip _beatLowAudioClip;

    [SerializeField]
    private float _beatGapMax;

    [SerializeField]
    private float _beatGapMin;

    [SerializeField]
    private int _beatSpeedUpCount;

    [SerializeField]
    private float _beatGapSpeedUpDelta;

    private AudioSource _fireAudioSource;
    private AudioSource _thrustAudioSource;
    private AudioSource _largeExplosionAudioSource;
    private AudioSource _mediumExplosionAudioSource;
    private AudioSource _smallExplosionAudioSource;
    private AudioSource _beatLowAudioSource;
    private AudioSource _beatHighAudioSource;

    private float _beatGapTimer = 0f;
    private bool _beatToggle = false;

    private float _beatGapCurrent;
    private int _beatCount = 0;
    

    public void Awake()
    {
        _beatGapCurrent = _beatGapMax;
        if (Instance==null)
        {
            Instance = this;
       
            _fireAudioSource= gameObject.AddComponent<AudioSource>();
            _fireAudioSource.clip = _fireAudioClip;
            _thrustAudioSource =gameObject.AddComponent<AudioSource>();
            _thrustAudioSource.clip = _thrustAudioClip;
            _largeExplosionAudioSource = gameObject.AddComponent<AudioSource>();
            _largeExplosionAudioSource.clip = _largeExplosionAudioClip;
            _mediumExplosionAudioSource = gameObject.AddComponent<AudioSource>();
            _mediumExplosionAudioSource.clip = _mediumExplosionAudioClip;
            _smallExplosionAudioSource = gameObject.AddComponent<AudioSource>();
            _smallExplosionAudioSource.clip= _smallExplosionAudioClip;
            _beatLowAudioSource = gameObject.AddComponent<AudioSource>();
            _beatLowAudioSource.clip = _beatLowAudioClip;
            _beatHighAudioSource = gameObject.AddComponent<AudioSource>();
            _beatHighAudioSource.clip = _beatHighAudioClip;
        }
    }

    void Start()
    {
    }

    void Update()
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

                    if (_beatCount > _beatSpeedUpCount)
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

    public void PlayFire()
    {
        if (!_fireAudioSource.isPlaying)
        {
            _fireAudioSource.Play();
        }
    }

    public void PlayThrust()
    {
        if (!_thrustAudioSource.isPlaying)
        {
            _thrustAudioSource.Play();
        }
    }

    public void PlayLargeExplosion()
    {
        if (!_largeExplosionAudioSource.isPlaying)
        {
            _largeExplosionAudioSource.Play();
        }
    }

    public void PlayMediumExplosion()
    {
        if (!_mediumExplosionAudioSource.isPlaying)
        {
            _mediumExplosionAudioSource.Play();
        }
    }

    public void PlaySmallExplosion()
    {
        if (!_smallExplosionAudioSource.isPlaying)
        {
            _smallExplosionAudioSource.Play();
        }
    }
}
