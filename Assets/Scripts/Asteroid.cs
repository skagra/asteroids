using System;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    // Events
    // Raised when an Asteroid collides with a missile.
    public event Action<GameObject> OnCollidedWithMissile;
    // Raised when an Asteroid collides with the player's ship
    public event Action<GameObject> OnCollidedWithPlayer;

    public Vector2 LinearVelocity
    {
        set { GetComponent<Rigidbody2D>().linearVelocity = value; }
    }

    public float AngularVelocity
    {
        set { GetComponent<Rigidbody2D>().angularVelocity = value; }
    }

    public Vector2 Position
    {
        set { transform.position = value; }
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
