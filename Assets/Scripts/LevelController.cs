using UnityEngine;

public class LevelController : MonoBehaviour
{
    // Values customisable in the Unity Inspector
    [Header("Number of Starting Asteroids")]
    [SerializeField]
    [Min(1)]
    private int _minStartAsteroids;
    [SerializeField]
    [Min(1)]
    private int _maxStartAsteroids;
    [SerializeField]
    [Min(0)]
    private int _startAsteroidsIncrement;

    [Header("Safety Zone Proportion of Screen")]
    [Range(0f, 0.5f)]
    [SerializeField]
    private float _safetyZoneProportion;

    [Header("References")]
    [SerializeField]
    private GameObject _player;
    [SerializeField]
    private GameObject _exclusionZone;
    [SerializeField]
    private GameObject _gameOverText;
    [SerializeField]
    private AsteroidField _asteroidField;
    [SerializeField]
    private AudioHub _audioHub;
    [SerializeField]
    private EventHub _eventHub;

    //private AsteroidField _asteroidFieldScript;
    private Player _playerScript;
    private ExclusionZone _exclusionZoneScript;

    private int _currentStartAsteroids;
    private float _safetyZoneRadius;
    private bool _gameOver = false;

    private void Awake()
    {
        _playerScript = _player.GetComponent<Player>();
        _exclusionZoneScript = _exclusionZone.GetComponent<ExclusionZone>();

        _eventHub.PlayerExploded += PlayerHitByAsteroid;
        _eventHub.AsteroidFieldCleared += LevelCleared;
        _eventHub.PlayerHasDied += PlayerIsDead;
    }

    private void PlayerIsDead()
    {
        _gameOver = true;
        _gameOverText.SetActive(true);
    }

    private void Start()
    {
        _safetyZoneRadius = ScreenUtils.Instance.Height * _safetyZoneProportion / 2.0f;
        _exclusionZoneScript.Radius = _safetyZoneRadius;
        _currentStartAsteroids = _minStartAsteroids;
        CreateSheet();
    }

    private void CreateSheet()
    {
        // The exclusion rect is centred on the players current position
        // TODO This does not account for the size of the new asteroids!  Should add 0.5 large asteroid size border all around, maybe this logic should be in the asteroid field
        _asteroidField.CreateSheet(_currentStartAsteroids,
            new Rect(new Vector2(_player.transform.position.x - _safetyZoneRadius, _player.transform.position.y - _safetyZoneRadius),
            new Vector2(_safetyZoneRadius * 2f, _safetyZoneRadius * 2f)));
    }

    private void Update()
    {
        // On death the ship is repositioned to the centre of the screen so we might need 
        // wait until the area is free from asteroids
        // TODO This assumes the spawn point is the centre of the screen!
        if (!_player.activeSelf && _exclusionZoneScript.IsSafe(Vector2.zero) && !_gameOver)
        {
            _player.SetActive(true);
        }
    }

    private void PlayerHitByAsteroid()
    {
        _player.SetActive(false);
    }

    private void LevelCleared()
    {
        // In the arcade game the first wave starts with 4 large asteroids.
        // Each subsequent wave adds two more, to a maximum of 11.
        // The game allows up to 27 asteroids on screen.
        // This implementation places no limit on the maximum number of asteroids on the screen.
        _currentStartAsteroids += _startAsteroidsIncrement;
        _currentStartAsteroids = Mathf.Min(_currentStartAsteroids, _maxStartAsteroids);
        _audioHub.ResetBeats();
        CreateSheet();
    }
}
