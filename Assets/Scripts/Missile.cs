using UnityEngine;

public class Missile : MonoBehaviour
{
    public delegate void CollidedWithAsteroidDelegate(GameObject missile);
    public event CollidedWithAsteroidDelegate CollidedWithAsteroid;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        var collidedWith = collider.gameObject;
        if (collidedWith.layer == Layers.LayerMaskAsteroid)
        {
            CollidedWithAsteroid?.Invoke(gameObject);
        }
        else
        {
            Debug.LogWarning($"Erronious collision flagged by Missile with {collidedWith.name}.");
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
