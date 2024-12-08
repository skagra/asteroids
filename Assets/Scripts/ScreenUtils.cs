using UnityEngine;

public class ScreenUtils : MonoBehaviour
{
    public static ScreenUtils Instance { get; private set; }

    public float MinScreenX
    {
        get; private set;
    }

    public float MinScreenY
    {
        get; private set;
    }

    public float MaxScreenX
    {
        get; private set;
    }

    public float MaxScreenY
    {
        get; private set;
    }

    public float Width
    {
        get { return MaxScreenX - MinScreenX; }
    }
    public float Height
    {
        get { return MaxScreenY - MinScreenY; }
    }

    private void Awake()
    {
        Instance = this;

        // Screen bounds
        var bottomLeft = Camera.main.ScreenToWorldPoint(Vector2.zero);
        MinScreenX = bottomLeft.x;
        MinScreenY = bottomLeft.y;

        var topRight = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        MaxScreenX = topRight.x;
        MaxScreenY = topRight.y;
    }

    public Vector3 Adjust(Vector3 currentPosition)
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
