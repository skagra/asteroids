using System;
using UnityEngine;

public class EventHub : MonoBehaviour
{
    public event Action PlayerHasDied;
    public event Action ExtraLifeThresholdPassed;
    public event Action PlayerExploded;
    public event Action PlayerExploding;
    public event Action AsteroidFieldCleared;
    public event Action<AsteroidSize> AsteroidCollidedWithMissile;

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
        _lives.PlayerHasDied += () => PlayerHasDied?.Invoke();
        _score.ExtraLifeThresholdPassed += () => ExtraLifeThresholdPassed?.Invoke();
        _player.Exploded += () => PlayerExploded?.Invoke();
        _player.Exploding += () => PlayerExploding?.Invoke();
        _asteroidField.FieldCleared += () => AsteroidFieldCleared?.Invoke();
        _asteroidField.CollidedWithMissile += (AsteroidSize size) => AsteroidCollidedWithMissile.Invoke(size);
    }
}
