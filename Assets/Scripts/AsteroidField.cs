using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AsteroidField : MonoBehaviour
{
    // Events
    // Raised when all asteroids have been destroyed
    public event Action FieldCleared;
    // Raised when an asteroid collides with a player missile
    public event Action<AsteroidSize> CollidedWithMissile;

    // Associated data with each asteroid
    private class AsteroidDetails
    {
        // The C# object associated with the asteroid
        public Asteroid AsteroidScript { get; set; }

        // The asteroid GameObject
        public GameObject Asteroid { get; set; }

        // Size of the asteroid
        public AsteroidSize AsteroidSize { get; set; }

        // Flag the asteroid GameObject for destruction on the next cycle
        public bool ReadyFromCleanUp { get; set; } = false;
    }

    // Values customisable in the Unity Inspector
    [Header("Asteroid Speed")]
    [SerializeField]
    [Range(2, 10)]
    private float _minSpeed;
    [SerializeField]
    [Range(2, 10)]
    private float _maxSpeed;

    [Header("Asteroid Spin")]
    [SerializeField]
    private bool _isSpinEnabled;
    [SerializeField]
    [Range(-180, 180)]
    private float _minDegreesPerSecond;
    [SerializeField]
    [Range(-180, 180)]
    private float _maxDegreesPerSecond;

    [Header("Asteroid Prefabs")]
    [SerializeField]
    private GameObject[] _largeAsteroidPrefabs;
    [SerializeField]
    private GameObject[] _mediumAsteroidPrefabs;
    [SerializeField]
    private GameObject[] _smallAsteroidPrefabs;

    [Header("References")]
    [SerializeField]
    private AudioHub _audioHub;

    // List of all active asteroids
    private readonly List<AsteroidDetails> _activeAsteroids = new();

    // Clear active asteroid list and destroy all associated GameObjects
    private void ClearAsteroids(List<AsteroidDetails> asteroids)
    {
        foreach (var asteroid in asteroids)
        {
            Destroy(asteroid.Asteroid);
        }
        _activeAsteroids.Clear();
    }

    // Create a sheet of large asteroids with random position, velocity and angular velocity,
    // while avoiding the exclusionZone Rect
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

    // Create an asteroid of the given size, with random position, velocity and angular velocity,
    // while avoiding the exclusionZone Rect.  Add the 
    private AsteroidDetails CreateRandomAsteroid(AsteroidSize size, Rect exclusionZone)
    {
        // Asteroid rotation, linear velocity, angular velocity and location
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

        // Pick random asteroid prefab of the required size
        var prefab = size switch
        {
            AsteroidSize.Large => _largeAsteroidPrefabs[Random.Range(0, _largeAsteroidPrefabs.Length - 1)],
            AsteroidSize.Medium => _mediumAsteroidPrefabs[Random.Range(0, _mediumAsteroidPrefabs.Length - 1)],
            AsteroidSize.Small => _smallAsteroidPrefabs[Random.Range(0, _smallAsteroidPrefabs.Length - 1)],
            _ => throw new System.NotImplementedException()
        };

        // Create the asteroid GameObject and set rotation, linear velocity, angular velocity and location
        var asteroid = Instantiate(prefab);

        var asteroidScript = asteroid.GetComponent<Asteroid>();

        asteroidScript.CollidedWithMissile += AsteroidHitByMissile;
        asteroidScript.CollidedWithPlayer += AsteroidHitByPlayer;

        asteroidScript.Velocity = newAsteroidLinearVelocity;
        asteroidScript.Position = newAsteroidPosition;
        asteroidScript.AngularVelocity = newAsteroidAngularVelocity;

        // Return the created asteroid and associated information
        return new AsteroidDetails { Asteroid = asteroid, AsteroidScript = asteroidScript, AsteroidSize = size, ReadyFromCleanUp = false };
    }

    // Create a random asteroid at the position of the given asteroid
    // at one size smaller than the given asteroid
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

    // Split the given asteroid into two smaller asteroids if the asteroid is Large or Medium,
    // and play the associated explosion audio
    private void SplitAsteroid(AsteroidDetails asteroid)
    {
        switch (asteroid.AsteroidSize)
        {
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

    // Called every frame
    private void Update()
    {
        // Destroy previously flagged asteroids
        CleanUpFlaggedAsteroids();
    }

    // Delete flagged entries from _activeAsteroids and destroy the associated GameObject for each.
    // Asteroids are not deleted immediately on collision as more than one collision may occur in a
    // a single frame and the associated AsteroidDetails may be required to handle each collision.
    private void CleanUpFlaggedAsteroids()
    {
        // Remove all flagged entries from _activeAsteroids and Destroy their associated GameObjects 
        for (var asteroidIndex = _activeAsteroids.Count - 1; asteroidIndex >= 0; asteroidIndex--)
        {
            var asteroidDetails = _activeAsteroids[asteroidIndex];
            if (asteroidDetails.ReadyFromCleanUp)
            {
                _activeAsteroids.RemoveAt(asteroidIndex);
                Destroy(asteroidDetails.Asteroid);
            }
        }
        // If all asteroids have been destroyed then raise the associated event
        if (_activeAsteroids.Count <= 0)
        {
            FieldCleared?.Invoke();
        }
    }

    // Handle a collision between an asteroid and either the player ship or missile
    private void AsteroidHit(bool isMissileCollision, GameObject asteroid)
    {
        // Find data associated with the asteroid
        var asteroidDetails = _activeAsteroids.Find(ad => ad.Asteroid == asteroid);

        if (asteroidDetails != null)
        {
            // If the collision was with a player missile then raise the appropriate event
            if (isMissileCollision)
            {
                CollidedWithMissile?.Invoke(asteroidDetails.AsteroidSize);
            }

            // Has this asteroid already been flagged for clean up this cycle?
            if (!asteroidDetails.ReadyFromCleanUp)
            {
                // Deactivate, flag for clean up and split into two smaller asteroids
                asteroid.SetActive(false);
                asteroidDetails.ReadyFromCleanUp = true;
                SplitAsteroid(asteroidDetails);
            }
        }
        else
        {
            Debug.LogError($"Failed to find AsteroidDetails name='{asteroid.name}', layer='{asteroid.layer}'.");
        }
    }

    // Handle collision between an asteroid and the player's ship
    private void AsteroidHitByPlayer(GameObject asteroid)
    {
        AsteroidHit(false, asteroid);
    }

    // Handle collision between an asteroid and a player missile
    private void AsteroidHitByMissile(GameObject asteroid)
    {
        AsteroidHit(true, asteroid);
    }
}
