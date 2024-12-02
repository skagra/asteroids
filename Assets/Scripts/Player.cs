using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private const string _INPUT_ROTATE_ACW = "Rotate ACW";
    private const string _INPUT_ROTATE_CW = "Rotate CW";
    private const string _INPUT_THRUST= "Thrust";
    private const string _INPUT_HYPERSPACE = "Hyperspace";

    private const string _ANIM_IS_THRUSTING_PARAM = "Is Thrusting";

    private bool _hyperspaceAvailable = true;

    [SerializeField]
    [InspectorName("Rotation Speed")]
    private float _rotationSpeed = 500.0f;
    [SerializeField]
    [InspectorName("Thrust Force")]
    private float _acceleration = 3.0f;
    [SerializeField]
    [InspectorName("Speed Limit")]
    private float _speedLimit = 4.0f;
    [SerializeField]
    [InspectorName("Hyperspace Border")]
    private float _hyperspaceBorder = 0.5f;
    [SerializeField]
    [InspectorName("Hyperspace Cooldown")]
    private float _hyperspaceCooldown = 5f;

    private InputAction _acwAction;
    private InputAction _cwAction;
    private InputAction _thrustAction;
    private InputAction _hyperspaceAction;
    private Rigidbody2D _body;
    private Animator _animator;
    private ParticleSystem _phosphorTrailParticleSystem;
    
    private float _minScreenX;
    private float _maxScreenX;
    private float _minScreenY;
    private float _maxScreenY;

    private float _timeSinceLastHyperspace;

    void Start()
    {
        _acwAction= InputSystem.actions.FindAction(_INPUT_ROTATE_ACW);
        _cwAction = InputSystem.actions.FindAction(_INPUT_ROTATE_CW);
        _thrustAction = InputSystem.actions.FindAction(_INPUT_THRUST);
        _hyperspaceAction=InputSystem.actions.FindAction(_INPUT_HYPERSPACE);

        _body = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _phosphorTrailParticleSystem = GetComponent<ParticleSystem>();

        var bottomLeft = Camera.main.ScreenToWorldPoint(Vector2.zero);
        _minScreenX = bottomLeft.x;
        _minScreenY = bottomLeft.y;

        var topRight = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        _maxScreenX = topRight.x;
        _maxScreenY = topRight.y;
    }

    void Update()
    {
        UpdateHyperspaceAvailability();

        KeepShipOnScreen();

        OrientatePhosphorTrail();

        ProcessInputs();
    }

    private void ProcessInputs()
    {
        if (_acwAction.IsPressed())
        {
            RotateAWCPressed();
        }
        else if (_cwAction.IsPressed())
        {
            RotateCWPressed();
        }
        if (_thrustAction.IsPressed())
        {
            _animator.SetBool(_ANIM_IS_THRUSTING_PARAM, true);
            ThrustPressed();
        }
        else
        {
            _animator.SetBool(_ANIM_IS_THRUSTING_PARAM, false);
        }

        if (_hyperspaceAction.WasPressedThisFrame())
        {
            HyperspacePressed();
        }
    }


    private void UpdateHyperspaceAvailability()
    {
        if (!_hyperspaceAvailable)
        {
            _timeSinceLastHyperspace += Time.deltaTime;
            if (_timeSinceLastHyperspace > _hyperspaceCooldown)
            {
                _hyperspaceAvailable = true;
            }
        }
    }

    private void OrientatePhosphorTrail()
    {
        var main = _phosphorTrailParticleSystem.main;
        main.startRotation = -transform.rotation.eulerAngles.z / (180.0f / Mathf.PI);
    }

    private void KeepShipOnScreen()
    {
        var newPosition=transform.position;

        if (transform.position.x < _minScreenX)
        {
            newPosition.x = _maxScreenX;
        }
        else if (transform.position.x > _maxScreenX)
        {
            newPosition.x = _minScreenX;
        }

        if (transform.position.y < _minScreenY)
        {
            newPosition.y = _maxScreenY;
        }
        else if (transform.position.y > _maxScreenY)
        {
            newPosition.y = _minScreenY;
        }

        transform.position = newPosition;
    }

    private void RotateAWCPressed()
    {
        transform.Rotate(0.0f, 0.0f, _rotationSpeed * Time.deltaTime);
    }

    private void RotateCWPressed()
    {
        transform.Rotate(0.0f, 0.0f, -1.0f * _rotationSpeed * Time.deltaTime);
    }

    private void ThrustPressed()
    {
        // Calculate the new velocity
        var acceleratedVelocity=new Vector2(_body.linearVelocity.x + transform.up.x * _acceleration * Time.deltaTime,
            _body.linearVelocity.y + transform.up.y * _acceleration * Time.deltaTime);

        // Restrict the maximum speed
        _body.linearVelocity = Vector2.ClampMagnitude(acceleratedVelocity, _speedLimit);
    }

    private void HyperspacePressed()
    {
        if (_hyperspaceAvailable) {
            transform.position = new Vector2(Random.Range(_minScreenX + _hyperspaceBorder, _maxScreenX - _hyperspaceBorder),
                Random.Range(_minScreenY + _hyperspaceBorder, _maxScreenY - _hyperspaceBorder));

            _hyperspaceAvailable = false;
            _timeSinceLastHyperspace = 0;
        }
    }
}
