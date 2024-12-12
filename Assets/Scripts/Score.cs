using System;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public event Action ExtraLifeThresholdPassed;

    [Header("Scores")]
    [SerializeField]
    private int _scoreLarge;
    [SerializeField]
    private int _scoreMedium;
    [SerializeField]
    private int _scoreSmall;

    [Header("Lives")]
    [SerializeField]
    private int _additionalLifeThreshold;

    [Header("References")]
    [SerializeField]
    private EventHub _eventHub;

    private Text _text;
    private int _score;
    private int _nextLifeThreshold;

    private void Awake()
    {
        _text = GetComponent<Text>();
        _score = 0;
        _nextLifeThreshold = _additionalLifeThreshold;
        _eventHub.AsteroidCollidedWithMissile += Scored;
    }

    private void Start()
    {
        DrawScore();
    }

    private void DrawScore()
    {
        _text.text = $"{_score:00000}";
    }

    private void Scored(AsteroidSize asteroidSize)
    {
        _score += asteroidSize switch
        {
            AsteroidSize.Large => _scoreLarge,
            AsteroidSize.Medium => _scoreMedium,
            AsteroidSize.Small => _scoreSmall,
            _ => throw new System.NotImplementedException()
        };

        DrawScore();

        if (_score >= _nextLifeThreshold)
        {
            _nextLifeThreshold += _additionalLifeThreshold;
            ExtraLifeThresholdPassed?.Invoke();
        }
    }

}
