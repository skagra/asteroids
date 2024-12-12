using System;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public event Action<GameObject> OnCollisionWithAsteroid;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        var collidedWith = collider.gameObject;
        if (collidedWith.layer == Layers.LayerMaskAsteroid)
        {
            OnCollisionWithAsteroid?.Invoke(gameObject);
        }
        else
        {
            Debug.LogWarning($"Erroneous collision flagged by Missile with {collidedWith.name}.");
        }

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
