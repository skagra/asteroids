using System.Collections.Generic;
using UnityEngine;
using AsteroidSize = Asteroid.AsteroidSize;

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

    [Header("Audio")]
    [SerializeField]
    private AudioHub _audioHub;

    private GameObject[] _asteroidPrefabs;
    private readonly List<AsteroidDetails> _activeAsteroids = new();

    private void Awake()
    {
        _asteroidPrefabs = new GameObject[] { _fullSizedAsteroidPrefab1, _fullSizedAsteroidPrefab2, 
            _fullSizedAsteroidPrefab3, _fullSizedAsteroidPrefab4 };
    }

    public void CreateSheet(int numAsteroids, Rect exclusionZone)
    {
        foreach (var asteroid in _activeAsteroids)
        {
            Destroy(asteroid.Asteroid);
        }
        _activeAsteroids.Clear();

        for (var i = 0; i < numAsteroids; i++)
        {
            var asteroidDetails = CreateRandomAsteroid(AsteroidSize.Large, exclusionZone);
            _activeAsteroids.Add(asteroidDetails);
            asteroidDetails.Asteroid.SetActive(true);
        }
    }

    private AsteroidDetails CreateRandomAsteroid(AsteroidSize size, Rect exclusionZone)
    {
        var newAsteroidAngle = Random.Range(0f, 2f * Mathf.PI); 
        var newAsteroidSpeed = Random.Range(_minSpeed, _maxSpeed);
        var newAsteroidLinearVelocity = newAsteroidSpeed * 
            new Vector2(Mathf.Cos(newAsteroidAngle), Mathf.Sin(newAsteroidAngle));

        var newAsteroidX = Random.Range(ScreenUtils.Instance.MinScreenX, ScreenUtils.Instance.MaxScreenX - exclusionZone.size.x);
        var newAsteroidY = Random.Range(ScreenUtils.Instance.MinScreenY, ScreenUtils.Instance.MaxScreenY - exclusionZone.size.y);

        if (newAsteroidX>exclusionZone.xMin)
        {
            newAsteroidX += exclusionZone.size.x;
        }
        if (newAsteroidY > exclusionZone.yMin)
        {
            newAsteroidY += exclusionZone.size.y;
        }

        var newAsteroidPosition = new Vector3(newAsteroidX, newAsteroidY, 0);  

        var prefab = _asteroidPrefabs[Random.Range(0, _asteroidPrefabs.Length - 1)];
        var asteroid = Instantiate(prefab);

        var asteroidScript = asteroid.GetComponent<Asteroid>();
        
        asteroidScript.AsteroidHitByMissile += AsteroidHit;
        asteroidScript.AsteroidHitByPlayer += AsteroidHit;

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

        var asteroid = CreateRandomAsteroid(newAsteroidSize, Rect.zero);

        asteroid.AsteroidScript.Position = existingAsteroid.Asteroid.transform.position;
        asteroid.Asteroid.SetActive(true);

        return asteroid;
    } 

    private void SplitAsteroid(AsteroidDetails asteroid)
    {
        switch (asteroid.AsteroidScript.Size) { 
            case AsteroidSize.Large:
                _audioHub.PlayLargeExplosion();
                break;
            case AsteroidSize.Medium:
                _audioHub.PlayMediumExplosion();
                break;
            case AsteroidSize.Small:
                _audioHub.PlaySmallExplosion();
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
