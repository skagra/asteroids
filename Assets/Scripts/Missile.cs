using UnityEngine;

public class Missile : MonoBehaviour
{
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
