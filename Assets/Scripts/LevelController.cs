using UnityEngine;
using UnityEngine.InputSystem;

public class LevelController : MonoBehaviour
{
    private const string _INPUT_ONE_PLAYER = "One Player";
    private const string _INPUT_QUIT = "Quit";

    private const int _BACKGROUND_ASTEROIDS = 8;

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
    private Lives _lives;
    [SerializeField]
    private AudioHub _audioHub;
    [SerializeField]
    private EventHub _eventHub;
    [SerializeField]
    private GameObject _coinText;
    [SerializeField]
    private GameObject _playText;

    private ExclusionZone _exclusionZoneScript;
    private InputAction _onePlayerAction;
    private InputAction _quitAction;
    private Player _playerScript;

    private int _currentStartAsteroids;
    private float _safetyZoneRadius;
    private bool _gameOver = false;

    private void Awake()
    {
        _playerScript = _player.GetComponent<Player>();

        _safetyZoneRadius = ScreenUtils.Instance.Height * _safetyZoneProportion / 2.0f;
        _exclusionZoneScript = _exclusionZone.GetComponent<ExclusionZone>();
        _exclusionZoneScript.Radius = _safetyZoneRadius;

        _onePlayerAction = InputSystem.actions.FindAction(_INPUT_ONE_PLAYER);
        _quitAction = InputSystem.actions.FindAction(_INPUT_QUIT);

        _eventHub.OnShipExploding += OnPlayerExploding;
        _eventHub.OnShipExploded += OnPlayerExploded;
        _eventHub.OnPlayerDeath += OnPlayerDeath;
        _eventHub.OnAsteroidFieldCleared += OnAsteroidFieldCleared;
    }

    private void Start()
    {
        WaitingToPlay();
    }

    // TODO Exclusion zone not account for the size of the new asteroids!  Should add 0.5 large asteroid size border all around, maybe this logic should be in the asteroid field
    private void CreateActiveSheet()
    {
        // The exclusion rect is centred on the players current position
        _asteroidField.CreateSheet(_currentStartAsteroids,
            new Rect(new Vector2(_player.transform.position.x - _safetyZoneRadius, _player.transform.position.y - _safetyZoneRadius),
                     new Vector2(_safetyZoneRadius * 2f, _safetyZoneRadius * 2f)));
    }

    private void CreatePassiveSheet()
    {
        // The exclusion rect is centred on the players current position
        _asteroidField.CreateSheet(_BACKGROUND_ASTEROIDS, new Rect(Vector2.zero, Vector2.zero), false);
    }

    private void Update()
    {
        if (_quitAction.WasPressedThisFrame())
        {
            Application.Quit();
        }

        if (_gameOver)
        {
            if (_onePlayerAction.WasPressedThisFrame())
            {
                StartPlaying();
            }
        }
        else
        {
            // On death the ship is repositioned to the centre of the screen so we might need 
            // wait until the area is free from asteroids
            // TODO This assumes the spawn point is the centre of the screen!
            if (!_player.activeSelf && _exclusionZoneScript.IsSafe(Vector2.zero) && !_playerScript.IsExploding)
            {
                _player.SetActive(true);
            }
        }
    }

    private void StartPlaying()
    {
        _currentStartAsteroids = _minStartAsteroids;
        _gameOver = false;
        _player.SetActive(true);
        _coinText.SetActive(false);
        _playText.SetActive(false);
        _gameOverText.SetActive(false);
        _lives.ResetLives();
        CreateActiveSheet();
    }

    private void WaitingToPlay()
    {
        _gameOver = true;
        _player.SetActive(false);
        _coinText.SetActive(true);
        _playText.SetActive(true);
        CreatePassiveSheet();
    }

    // Called immediately when final ship is destroyed
    private void GameOver()
    {
        _gameOver = true;
        _coinText.SetActive(true);
        _playText.SetActive(true);
        _gameOverText.SetActive(true);
    }

    private void OnPlayerExploding()
    {
    }

    private void OnPlayerExploded()
    {
        _player.SetActive(false);
    }

    private void OnPlayerDeath()
    {
        GameOver();
    }

    // In the arcade game the first wave starts with 4 large asteroids.
    // Each subsequent wave adds two more, to a maximum of 11.
    // The game allows up to 27 asteroids on screen.
    // This implementation places no limit on the maximum number of asteroids on the screen.
    private void OnAsteroidFieldCleared()
    {
        _currentStartAsteroids += _startAsteroidsIncrement;
        _currentStartAsteroids = Mathf.Min(_currentStartAsteroids, _maxStartAsteroids);
        _audioHub.ResetBeats();
        CreateActiveSheet();
    }
}
