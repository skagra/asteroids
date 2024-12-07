using UnityEngine;

public class GamePlayerController : MonoBehaviour
{
    // Values customisable in the Unity Inspector
    [SerializeField]
    private GameObject _player;

    [SerializeField]
    private GameObject _asteroidField;

    private void Awake()
    {
        var playerSript=_player.GetComponent<Player>();
        playerSript.PlayerHasHitAsteroid += PlayerHitByAsteroid;

        var asteroidFieldScript = _asteroidField.GetComponent<AsteroidField>();
        asteroidFieldScript.LevelCleared += LevelCleared;
    }

    private void PlayerHitByAsteroid(GameObject player)
    {
        Debug.Log("LOST A LIFE");
    }

    private void LevelCleared()
    {
        Debug.Log("LEVEL CLEARED");
    }
}
