using System;
using UnityEngine;

public class AsteroidExplosion : MonoBehaviour
{
    public event Action<GameObject> OnAsteroidExplosion;

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


    private void AnimationComplete()
    {
        OnAsteroidExplosion?.Invoke(gameObject);
    }
}
