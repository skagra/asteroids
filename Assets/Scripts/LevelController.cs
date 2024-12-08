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

    [Header("Lives")]
    [SerializeField]
    [Range(3,5)]
    private int _maxLives;

    [Header("Safety Zone Proportion of Screen")]
    [Range(0f,0.5f)]
    [SerializeField]
    private float _safetyZoneProportion;

    [Header("References")]
    [SerializeField]
    private GameObject _player;
    [SerializeField]
    private GameObject _asteroidField;
    [SerializeField]
    private GameObject _exclusionZone;

    private AsteroidField _asteroidFieldScript;
    private Player _playerScript;
    private ExclusionZone _exclusionZoneScript;

    private int _currentNumLives;
    private int _currentStartAsteroids;
    private float _safetyZoneRadius;

    private void Awake()
    {
        _currentNumLives = _maxLives;
        _playerScript=_player.GetComponent<Player>();
        _playerScript.PlayerHasHitAsteroid += PlayerHitByAsteroid;

        _asteroidFieldScript = _asteroidField.GetComponent<AsteroidField>();
        _asteroidFieldScript.LevelCleared += LevelCleared;

        _exclusionZoneScript=_exclusionZone.GetComponent<ExclusionZone>(); 
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
        // The exclusion rect is centred on the player and of dimensions 2 * the radius of the exclusion zone collider
        // TODO This does not account for the size of the new asteroids!  Should add 0.5 large asteroid size border all around, maybe this logic should be in the asteroid field
        _asteroidFieldScript.CreateSheet(_currentStartAsteroids,
            new Rect(new Vector2(_player.transform.position.x- _safetyZoneRadius, _player.transform.position.y - _safetyZoneRadius), 
            new Vector2(_safetyZoneRadius * 2f, _safetyZoneRadius * 2f)));
    }

    private void Update()
    {
        // On death the ship is repositied to the centre of the screen so we might need 
        // wait until the area is free from asteroids
        if (!_player.activeSelf && _currentNumLives > 0 && _exclusionZoneScript.IsSafe(_player.transform.position))
        {
            _player.SetActive(true);
        }
    }

    private void PlayerHitByAsteroid(GameObject player)
    {
        //Debug.Log("LOST A LIFE");
        // _numLives--;
        player.SetActive(false);
        if (_currentNumLives > 0)
        {
            _playerScript.Init();
        }
    }

    private void LevelCleared()
    {
        // In the arcade game the first wave starts with 4 large asteroids.
        // Each subsequent wave adds two more, to a maximum of 11.
        // The game allows up to 27 asteroids on screen.
        // This implementation places no limit on the maxium number of asteroids on the screen.
        _currentStartAsteroids += _startAsteroidsIncrement;
        _currentStartAsteroids = Mathf.Min(_currentStartAsteroids, _maxStartAsteroids);
        CreateSheet();
    }
}
