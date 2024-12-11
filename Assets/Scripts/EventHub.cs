using UnityEngine;
using static AsteroidField;

public class EventHub : MonoBehaviour
{
    public delegate void PlayerHasDiedDelegate();
    public event PlayerHasDiedDelegate PlayerHasDied;

    public delegate void ExtraLifeThresholdPassedDelegate();
    public event ExtraLifeThresholdPassedDelegate ExtraLifeThresholdPassed;

    public delegate void PlayerCollidedWithAsteroidDelegate();
    public event PlayerCollidedWithAsteroidDelegate PlayerExploded;
    public event PlayerCollidedWithAsteroidDelegate PlayerExploding;

    public delegate void AsteroidFieldClearedDelegate();
    public event AsteroidFieldClearedDelegate AsteroidFieldCleared;

    public delegate void AsteroidCollidedWithMissileDelegate(AsteroidSize size);
    public event AsteroidCollidedWithMissileDelegate AsteroidCollidedWithMissile;

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
