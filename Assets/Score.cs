using UnityEngine;
using UnityEngine.UI;
using AsteroidSize = Asteroid.AsteroidSize;

public class Score : MonoBehaviour
{
    private Text _text;
    private int _score;

    [Header("Scores")]
    [SerializeField]
    private int _scoreLarge;
    [SerializeField]
    private int _scoreMedium;
    [SerializeField]
    private int _scoreSmall;

    [Header("References")]
    [SerializeField]
    private AsteroidField _asteroidField;

    private void Awake()
    {
        _text = GetComponent<Text>();
        _score = 0;

        _asteroidField.AsteroidDestroyed += Scored;
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
    }
    
}
