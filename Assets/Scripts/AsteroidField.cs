using System.Collections.Generic;
using UnityEngine;
using AsteroidSize = Asteroid.AsteroidSize;

// The first wave starts with 4 large asteroids. Each subsequent wave adds two more, to a maximum of 11. The game allows up to 27 asteroids on scree
public class AsteroidField : MonoBehaviour
{
    public delegate void Notify();
    public event Notify LevelCleared;

    private class AsteroidDetails
    {
        public Asteroid AsteroidScript { get; set; }

        public GameObject Asteroid { get; set; }
    }

    // Values customisable in the Unity Inspector
    [Header("Large Asteroid Prefabs")]
    [SerializeField]
    private GameObject _fullSizedAsteroidPrefab1;
    [SerializeField]
    private GameObject _fullSizedAsteroidPrefab2;
    [SerializeField]
    private GameObject _fullSizedAsteroidPrefab3;
    [SerializeField]
    private GameObject _fullSizedAsteroidPrefab4;

    [Header("Asteroid Speed Range")]
    [SerializeField]
    private float _minSpeed;
    [SerializeField]
    private float _maxSpeed;

    private GameObject[] _asteroidPrefabs;
    private readonly List<AsteroidDetails> _activeAsteroids = new();

    private void Awake()
    {
        _asteroidPrefabs = new GameObject[] { _fullSizedAsteroidPrefab1, _fullSizedAsteroidPrefab2, 
            _fullSizedAsteroidPrefab3, _fullSizedAsteroidPrefab4 };
    }

    private void Start()
    {
        CreateSheet(4); // ToDo
    }

    private void CreateSheet(int numAsteroids)
    {
        for (var i = 0; i < numAsteroids; i++)
        {
            var asteroidDetails = CreateRandomAsteroid(AsteroidSize.Large);
            _activeAsteroids.Add(asteroidDetails);
            asteroidDetails.Asteroid.SetActive(true);
        }
    }

    private AsteroidDetails CreateRandomAsteroid(AsteroidSize size)
    {
        var newAsteroidAngle = Random.Range(0f, 2f * Mathf.PI); 
        var newAsteroidSpeed = Random.Range(_minSpeed, _maxSpeed);
        var newAsteroidLinearVelocity = newAsteroidSpeed * 
            new Vector2(Mathf.Cos(newAsteroidAngle), Mathf.Sin(newAsteroidAngle)); 

        var newAsteroidPosition = new Vector3(Random.Range(ScreenUtils.MinScreenX, ScreenUtils.MaxScreenX),
            Random.Range(ScreenUtils.MinScreenY, ScreenUtils.MaxScreenY), 0);

        var prefab = _asteroidPrefabs[Random.Range(0, _asteroidPrefabs.Length - 1)];
        var asteroid = Instantiate(prefab);

        var asteroidScript = asteroid.GetComponent<Asteroid>();
        
        asteroidScript.AsteroidHitByMissile += AsteroidHit;

        asteroidScript.Size = size;
        asteroidScript.Velocity = newAsteroidLinearVelocity;
        asteroidScript.Position = newAsteroidPosition;

        return new AsteroidDetails { Asteroid = asteroid, AsteroidScript= asteroidScript };
    }

    private AsteroidDetails CreateSplitAsteroid(AsteroidDetails existingAsteroid)
    {
        var newAsteroidSize = existingAsteroid.AsteroidScript.Size switch
        {
            AsteroidSize.Large => AsteroidSize.Medium,
            AsteroidSize.Medium => AsteroidSize.Small,
            _ => throw new System.NotImplementedException()
        };

        var asteroid = CreateRandomAsteroid(newAsteroidSize);

        asteroid.AsteroidScript.Position = existingAsteroid.Asteroid.transform.position;
        asteroid.Asteroid.SetActive(true);

        return asteroid;
    } 

    private void SplitAsteroid(AsteroidDetails asteroid)
    {
        switch (asteroid.AsteroidScript.Size) { 
            case AsteroidSize.Large:
                AudioHub.Instance.PlayLargeExplosion();
                break;
            case AsteroidSize.Medium:
                AudioHub.Instance.PlayMediumExplosion();
                break;
            case AsteroidSize.Small:
                AudioHub.Instance.PlaySmallExplosion();
                break;
        }

        if (asteroid.AsteroidScript.Size != AsteroidSize.Small)
        {
            _activeAsteroids.Add(CreateSplitAsteroid(asteroid));
            _activeAsteroids.Add(CreateSplitAsteroid(asteroid));
        }
    }

    private void AsteroidHit(GameObject asteroid)
    {
        asteroid.SetActive(false);
        
        var asteroidDetails = _activeAsteroids.Find(ad => ad.Asteroid == asteroid);
        SplitAsteroid(asteroidDetails);

        _activeAsteroids.Remove(asteroidDetails);
        Destroy(asteroidDetails.Asteroid);

        if (_activeAsteroids.Count<=0)
        {
            LevelCleared?.Invoke();
        }
    }

}
