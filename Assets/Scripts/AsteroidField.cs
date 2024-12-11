using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AsteroidField : MonoBehaviour
{
    public enum AsteroidSize { Large, Medium, Small }

    public delegate void FieldClearedDelegate();
    public event FieldClearedDelegate FieldCleared;

    public delegate void AsteroidCollidedWithMissileDelegate(AsteroidSize size);
    public event AsteroidCollidedWithMissileDelegate AsteroidCollidedWithMissile;

    private class AsteroidDetails
    {
        public Asteroid AsteroidScript { get; set; }

        public GameObject Asteroid { get; set; }

        public AsteroidSize AsteroidSize { get; set; }

        public bool ReadyFromCleanUp { get; set; } = false;
    }

    // Values customisable in the Unity Inspector
    [Header("Asteroid Prefabs")]
    [SerializeField]
    private GameObject[] _largeAsteroidPrefabs;
    [SerializeField]
    private GameObject[] _mediumAsteroidPrefabs;
    [SerializeField]
    private GameObject[] _smallAsteroidPrefabs;

    [Header("Asteroid Speed")]
    [SerializeField]
    private float _minSpeed;
    [SerializeField]
    private float _maxSpeed;

    [Header("Asteroid Spin")]
    [SerializeField]
    private bool _isSpinEnabled;
    [SerializeField]
    private float _minDegreesPerSecond;
    [SerializeField]
    private float _maxDegreesPerSecond;

    [Header("References")]
    [SerializeField]
    private AudioHub _audioHub;
    [SerializeField]
    Player _player;

    private readonly List<AsteroidDetails> _activeAsteroids = new();

    private void ClearAsteroids(List<AsteroidDetails> asteroids)
    {
        foreach (var asteroid in asteroids)
        {
            Destroy(asteroid.Asteroid);
        }
        _activeAsteroids.Clear();
    }

    public void CreateSheet(int numAsteroids, Rect exclusionZone)
    {
        ClearAsteroids(_activeAsteroids);

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
        var newAsteroidAngularVelocity = _isSpinEnabled ? Random.Range(_minDegreesPerSecond, _maxDegreesPerSecond) : 0f;

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
        
        asteroidScript.AsteroidHitByMissile += AsteroidHitByMissile;
        asteroidScript.AsteroidHitByPlayer += AsteroidHitByPlayer;

        asteroidScript.Velocity = newAsteroidLinearVelocity;
        asteroidScript.Position = newAsteroidPosition;
        asteroidScript.AngularVelocity = newAsteroidAngularVelocity;

        return new AsteroidDetails { Asteroid = asteroid, AsteroidScript = asteroidScript, AsteroidSize = size, ReadyFromCleanUp = false };
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
            _activeAsteroids.Add(CreateSplitAsteroid(asteroid));
            _activeAsteroids.Add(CreateSplitAsteroid(asteroid));
        }
    }

    private void Update()
    {
        CleanUpFlaggedAsteroids();
    }

    private void CleanUpFlaggedAsteroids()
    {
        for (var asteroidIndex = _activeAsteroids.Count - 1; asteroidIndex >= 0; asteroidIndex--)
        {
            var asteroidDetails = _activeAsteroids[asteroidIndex];
            if (asteroidDetails.ReadyFromCleanUp)
            {
                _activeAsteroids.RemoveAt(asteroidIndex);
                Destroy(asteroidDetails.Asteroid);
            }
        }
        if (_activeAsteroids.Count <= 0)
        {
            FieldCleared?.Invoke();
        }
    }

    private void AsteroidHit(bool isMissileCollision, GameObject asteroid)
    {
        var asteroidDetails = _activeAsteroids.Find(ad => ad.Asteroid == asteroid);

        if (asteroidDetails != null)
        {
            if (isMissileCollision)
            {
                AsteroidCollidedWithMissile?.Invoke(asteroidDetails.AsteroidSize);
            }

            if (!asteroidDetails.ReadyFromCleanUp)
            {
                asteroid.SetActive(false);
                asteroidDetails.ReadyFromCleanUp = true;
                SplitAsteroid(asteroidDetails);
            }
        }
        else
        {
            Debug.LogError($"Failed to find AsteroidDetails {asteroid.name} {asteroid.layer}");
        }
    }

    private void AsteroidHitByPlayer(GameObject asteroid)
    {
        if (!_player.IsExploding) { 
            AsteroidHit(false, asteroid);
        }
    }

    private void AsteroidHitByMissile(GameObject asteroid)
    {
        AsteroidHit(true, asteroid);
    }
}
