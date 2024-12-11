using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public delegate void CollidedWithMissileDelegate(GameObject asteroid);
    public event CollidedWithMissileDelegate CollidedWithMissile;

    public delegate void CollidedWithPlayerDelegate(GameObject asteroid);
    public event CollidedWithPlayerDelegate CollidedWithPlayer;

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
            CollidedWithMissile?.Invoke(gameObject);
        }
        else if (collidedWith.layer == Layers.LayerMaskPlayer)
        {
            CollidedWithPlayer?.Invoke(gameObject);
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
