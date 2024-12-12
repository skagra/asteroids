using System;
using UnityEngine;

public class AsteroidExplosion : MonoBehaviour
{
    public event Action<GameObject> OnAsteroidExplosion;

    private void AnimationComplete()
    {
        OnAsteroidExplosion?.Invoke(gameObject);
    }
}
