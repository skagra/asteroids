using UnityEngine;

public class Missile : MonoBehaviour
{
    public delegate void Notify(GameObject missile);
    public event Notify MissleHasHitAsteroid;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        MissleHasHitAsteroid?.Invoke(gameObject);
    }

    void Start()
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
