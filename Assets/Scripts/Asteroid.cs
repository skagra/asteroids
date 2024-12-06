using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public enum AsteroidSize { Large, Medium, Small }

    public delegate void Notify(GameObject asteroid);
    public event Notify AsteroidHitByMissile;

    private Rigidbody2D _body;

    public Vector2 Velocity { get; set; }
    public Vector3 Position { get; set; }
    public AsteroidSize Size { get; set; }

    private void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        AsteroidHitByMissile?.Invoke(gameObject);
    }

    private void OnEnable()
    {     
        _body.linearVelocity = Velocity;
        transform.position = Position;
        transform.localScale = Size switch
        { // Todo - Constants
            AsteroidSize.Large => new Vector3(1.0f, 1.0f, 1.0f),
            AsteroidSize.Medium => new Vector3(0.5f, 0.5f, 0.5f),
            AsteroidSize.Small => new Vector3(0.25f, 0.25f, 0.25f),
            _ => throw new System.NotImplementedException()
        };
    }

    private void OnDisable()
    {
    }

    void Update()
    {
        KeepOnScreen();
    }

    private void KeepOnScreen()
    {
       transform.position = ScreenUtils.Adjust(transform.position);
    }
}
