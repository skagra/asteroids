using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public enum AsteroidSize { Large, Medium, Small }

    public delegate void Notify(GameObject asteroid);
    public event Notify AsteroidHitByMissile;
    public event Notify AsteroidHitByPlayer;

    private Rigidbody2D _body;

    public Vector2 Velocity { get; set; }
    public Vector3 Position { get; set; }

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
        _body.linearVelocity = Velocity;
        transform.position = Position;
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
