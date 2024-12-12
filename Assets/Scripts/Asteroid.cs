using System;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    // Events
    // Raised when an Asteroid collides with a missile.
    public event Action<GameObject> OnCollidedWithMissile;
    // Raised when an Asteroid collides with the player's ship
    public event Action<GameObject> OnCollidedWithPlayer;

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

    // Raise events when there is a collision with either a player 
    // ship or missile
    private void OnTriggerEnter2D(Collider2D collider)
    {
        var collidedWith = collider.gameObject;
        if (collidedWith.layer == Layers.LayerMaskMissile)
        {
            OnCollidedWithMissile?.Invoke(gameObject);
        }
        else if (collidedWith.layer == Layers.LayerMaskPlayer)
        {
            OnCollidedWithPlayer?.Invoke(gameObject);
        }
        else
        {
            Debug.LogError($"Erroneous collision flagged by Asteroid with name='{collidedWith.name}', layer='{collidedWith.layer}'.");
        }
    }

    // Apply settings when the asteroid is enabled
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

    // If the asteroid has left the screen then flip it to the opposite side
    private void KeepOnScreen()
    {
        transform.position = ScreenUtils.Instance.Adjust(transform.position);
    }
}
