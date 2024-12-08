using UnityEngine;

public class Layers : MonoBehaviour
{
    public const string LAYER_NAME_PLAYER = "Player";
    public const string LAYER_NAME_ASTEROID = "Asteroid";
    public const string LAYER_NAME_MISSILE = "Missile";

    public static int LayerMaskPlayer { get; private set; }  
    public static int LayerMaskAsteroid { get; private set; }
    public static int LayerMaskMissile { get; private set; }

    private void Awake()
    {
        LayerMaskPlayer = LayerMask.NameToLayer(LAYER_NAME_PLAYER);
        LayerMaskAsteroid =LayerMask.NameToLayer(LAYER_NAME_ASTEROID);
        LayerMaskMissile = LayerMask.NameToLayer(LAYER_NAME_MISSILE);
    }
}
