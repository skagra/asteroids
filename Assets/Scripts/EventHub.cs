using System;
using UnityEngine;

public class EventHub : MonoBehaviour
{
    // Values customisable in the Unity Inspector
    // Raised when the player has no lives left
    public event Action OnPlayerDeath;
    // Raised when the player's score passes an extra life threshold
    public event Action OnExtraLifeThresholdPassed;
    // Raised once the player's ship has finished exploding
    public event Action OnShipExploded;
    // Raised when the player's ship starts to explode
    public event Action OnShipExploding;
    // Raised when all asteroids on the current level have been destroyed
    public event Action OnAsteroidFieldCleared;
    // Raised when an asteroid collides with a player missile
    public event Action<AsteroidSize> OnAsteroidCollisionWithMissile;

    [Header("References")]
    [SerializeField]
    private Lives _lives;
    [SerializeField]
    private Score _score;
    [SerializeField]
    private Player _player;
    [SerializeField]
    private AsteroidField _asteroidField;

    void Awake()
    {
        _lives.OnPlayerDeath += () => OnPlayerDeath?.Invoke();
        _score.ExtraLifeThresholdPassed += () => OnExtraLifeThresholdPassed?.Invoke();
        _player.OnExploded += () => OnShipExploded?.Invoke();
        _player.OnExploding += () => OnShipExploding?.Invoke();
        _asteroidField.OnFieldCleared += () => OnAsteroidFieldCleared?.Invoke();
        _asteroidField.OnCollisionWithMissile += (AsteroidSize size) => OnAsteroidCollisionWithMissile.Invoke(size);
    }
}
