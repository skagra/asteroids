using UnityEngine;
using UnityEngine.Android;

public class ExclusionZone : MonoBehaviour
{
    private int _maskIndex;
    private CircleCollider2D _circleCollider;

    [Header("General Settings")]
    [SerializeField]
    private float _radius;

    public float Radius
    {
        get { return _radius; }
        set { _radius = value; }
    }

    private void Awake()
    {
        _circleCollider = gameObject.GetComponent<CircleCollider2D>();
        _maskIndex = LayerMask.GetMask(Layers.LAYER_NAME_ASTEROID);
    }

    private void Start()
    {
        _circleCollider.radius = _radius;
    }

    public bool IsSafe(Vector2 position)
    {
        transform.position = position;
        return !_circleCollider.IsTouchingLayers(_maskIndex);
    }
}
