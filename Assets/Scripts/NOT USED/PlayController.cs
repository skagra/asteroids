using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{

   // public static GameController Instance { get; private set; }

    public void OnEnable()
    {
        Debug.Log("OnEnable called");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    //private void Awake()
    //{
    //    if (Instance == null)
    //    {
    //        Instance = this;
    //        DontDestroyOnLoad(gameObject);
    //    }
    //}

    private void Start()
    {
        Debug.Log("Loading first scene");
        SceneManager.LoadScene("Game Play");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game Play") { 
            var player = GameObject.Find("Player");
            var playerScript = player.GetComponent<Player>();
            playerScript.Exploded += PlayerHasHitAsteroid;
        }
    }

    private void PlayerHasHitAsteroid()
    {
        Debug.Log("IN CONTROLLER PLAYER HAS HIT ASTEROID");
    }
}
