using UnityEngine;

public static class ScreenUtils 
{
    public static float MinScreenX { 
        get; private set;
    }

    public static float MinScreenY
    {
        get; private set;
    }

    public static float MaxScreenX
    {
        get; private set;
    }

    public static float MaxScreenY
    {
        get; private set;
    }

    [RuntimeInitializeOnLoadMethod]
    public static void OnLoad()
    {
        // Screen bounds
        var bottomLeft = Camera.main.ScreenToWorldPoint(Vector2.zero);
        MinScreenX = bottomLeft.x;
        MinScreenY = bottomLeft.y;

        var topRight = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        MaxScreenX = topRight.x;
        MaxScreenY = topRight.y;
    }

    public static Vector3 Adjust(Vector3 currentPosition)
    {
        var adjustedPosition = currentPosition;

        if (currentPosition.x < MinScreenX)
        {
            adjustedPosition.x = MaxScreenX;
        }
        else if (currentPosition.x > MaxScreenX)
        {
            adjustedPosition.x = MinScreenX;
        }

        if (currentPosition.y < MinScreenY)
        {
            adjustedPosition.y = MaxScreenY;
        }
        else if (currentPosition.y > MaxScreenY)
        {
            adjustedPosition.y = MinScreenY;
        }

        return adjustedPosition;
    }
}
