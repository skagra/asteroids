using UnityEngine;

public class Asteroid : MonoBehaviour
{
    // Events
    // Raised when an Asteroid collides with a missile.
    public delegate void CollidedWithMissileDelegate(GameObject asteroid);
    public event CollidedWithMissileDelegate CollidedWithMissile;
    // Raised when an Asteroid collides with the player's ship
    public delegate void CollidedWithPlayerDelegate(GameObject asteroid);
    public event CollidedWithPlayerDelegate CollidedWithPlayer;

    // Components
    private Rigidbody2D _body;

    // Initialization settings
    public Vector2 Velocity { get; set; }
    public Vector3 Position { get; set; }
    public float AngularVelocity { get; set; }

    private void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        var collidedWith = collider.gameObject;
        if (collidedWith.layer == Layers.LayerMaskMissile)
        {
            CollidedWithMissile?.Invoke(gameObject);
        }
        else if (collidedWith.layer == Layers.LayerMaskPlayer)
        {
            CollidedWithPlayer?.Invoke(gameObject);
        }
        else
        {
            Debug.LogWarning($"Erroneous collision flagged by Asteroid with {collidedWith.name}.");
        }
    }

    private void OnEnable()
    {
        transform.position = Position;
        _body.linearVelocity = Velocity;
        _body.angularVelocity = AngularVelocity;
    }

    private void Update()
    {
        KeepOnScreen();
    }

    private void KeepOnScreen()
    {
        transform.position = ScreenUtils.Instance.Adjust(transform.position);
    }
}
