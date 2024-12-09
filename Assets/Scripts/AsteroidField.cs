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

        public AsteroidSize AsteroidSize { get; set; }
    }

    // Values customisable in the Unity Inspector
    [Header("Asteroid Prefabs")]
    [SerializeField]
    private GameObject[] _largeAsteroidPrefabs;
    [SerializeField]
    private GameObject[] _mediumAsteroidPrefabs;
    [SerializeField]
    private GameObject[] _smallAsteroidPrefabs;

    [Header("Asteroid Speed Range")]
    [SerializeField]
    private float _minSpeed;
    [SerializeField]
    private float _maxSpeed;

    [Header("Audio")]
    [SerializeField]
    private AudioHub _audioHub;

    private readonly List<AsteroidDetails> _activeLargeAsteroids = new();
    private readonly List<AsteroidDetails> _activeMediumAsteroids = new();
    private readonly List<AsteroidDetails> _activeSmallAsteroids = new();

    private void ClearAsteroids(List<AsteroidDetails> asteroids)
    {
        foreach (var asteroid in asteroids)
        {
            Destroy(asteroid.Asteroid);
        }
        _activeLargeAsteroids.Clear();

    }

    public void CreateSheet(int numAsteroids, Rect exclusionZone)
    {
        ClearAsteroids(_activeLargeAsteroids);
        ClearAsteroids(_activeMediumAsteroids);
        ClearAsteroids(_activeSmallAsteroids);

        for (var i = 0; i < numAsteroids; i++)
        {
            var asteroidDetails = CreateRandomAsteroid(AsteroidSize.Large, exclusionZone);
            _activeLargeAsteroids.Add(asteroidDetails);
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

        if (newAsteroidX > exclusionZone.xMin)
        {
            newAsteroidX += exclusionZone.size.x;
        }
        if (newAsteroidY > exclusionZone.yMin)
        {
            newAsteroidY += exclusionZone.size.y;
        }

        var newAsteroidPosition = new Vector3(newAsteroidX, newAsteroidY, 0);

        var prefab = size switch
        {
            AsteroidSize.Large => _largeAsteroidPrefabs[Random.Range(0, _largeAsteroidPrefabs.Length - 1)],
            AsteroidSize.Medium => _mediumAsteroidPrefabs[Random.Range(0, _mediumAsteroidPrefabs.Length - 1)],
            AsteroidSize.Small => _smallAsteroidPrefabs[Random.Range(0, _smallAsteroidPrefabs.Length - 1)],
            _ => throw new System.NotImplementedException()
        };

        var asteroid = Instantiate(prefab);

        var asteroidScript = asteroid.GetComponent<Asteroid>();
        
        asteroidScript.AsteroidHitByMissile += AsteroidHit;
        asteroidScript.AsteroidHitByPlayer += AsteroidHit;

        asteroidScript.Velocity = newAsteroidLinearVelocity;
        asteroidScript.Position = newAsteroidPosition;

        return new AsteroidDetails { Asteroid = asteroid, AsteroidScript = asteroidScript, AsteroidSize = size };
    }

    private AsteroidDetails CreateSplitAsteroid(AsteroidDetails existingAsteroid)
    {
        var newAsteroidSize = existingAsteroid.AsteroidSize switch
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
        switch (asteroid.AsteroidSize) { 
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
        
        if (asteroid.AsteroidSize != AsteroidSize.Small)
        {
            _activeLargeAsteroids.Add(CreateSplitAsteroid(asteroid));
            _activeLargeAsteroids.Add(CreateSplitAsteroid(asteroid));
        }
    }

    private void AsteroidHit(GameObject asteroid)
    {
        asteroid.SetActive(false);
        
        var asteroidDetails = _activeLargeAsteroids.Find(ad => ad.Asteroid == asteroid);
        SplitAsteroid(asteroidDetails);

        _activeLargeAsteroids.Remove(asteroidDetails);
        Destroy(asteroidDetails.Asteroid);

        if (_activeLargeAsteroids.Count<=0)
        {
            LevelCleared?.Invoke();
        }
    }

}
