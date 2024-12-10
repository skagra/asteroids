using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public delegate void AsteroidHitByMissileDelegate(GameObject asteroid);
    public event AsteroidHitByMissileDelegate AsteroidHitByMissile;

    public delegate void AsteroidHitByPlayerDelegate(GameObject asteroid);
    public event AsteroidHitByPlayerDelegate AsteroidHitByPlayer;

    private Rigidbody2D _body;

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
            AsteroidHitByMissile?.Invoke(gameObject);
        }
        else if (collidedWith.layer == Layers.LayerMaskPlayer)
        {
            AsteroidHitByPlayer?.Invoke(gameObject);
        }
        else
        {
            Debug.LogWarning($"Erronious collision flagged by Asteroid with {collidedWith.name}.");
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
