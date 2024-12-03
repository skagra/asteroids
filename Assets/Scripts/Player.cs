using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private sealed class ActiveMissile
    {
        public GameObject Missile { get; set; }
        public float SpawnTime { get; set; }

    }

    // Input names
    private const string _INPUT_ROTATE_ACW = "Rotate ACW";
    private const string _INPUT_ROTATE_CW = "Rotate CW";
    private const string _INPUT_THRUST= "Thrust";
    private const string _INPUT_HYPERSPACE = "Hyperspace";
    private const string _INPUT_FIRE = "Fire";

    // Animation parameters
    private const string _ANIM_IS_THRUSTING_PARAM = "Is Thrusting";

    // Lists of active and dormant missiles
    private readonly List<GameObject> _dormantMissiles = new();
    private readonly List<ActiveMissile> _activeMissiles = new();

    // Values customisable in the Unity Inspector
    [SerializeField]
    [InspectorName("Rotation Speed")]
    private float _rotationSpeed;
    [SerializeField]
    [InspectorName("Thrust Force")]
    private float _acceleration;
    [SerializeField]
    [InspectorName("Speed Limit")]
    private float _speedLimit;
    [SerializeField]
    [InspectorName("Missile Speed")]
    private float _missileSpeed;
    [SerializeField]
    [InspectorName("Hyperspace Border")]
    private float _hyperspaceBorder;
    [SerializeField]
    [InspectorName("Hyperspace Cooldown")]
    private float _hyperspaceCooldown;
    [SerializeField]
    [InspectorName("Missile Count")]
    private int _missileCount;
    [SerializeField]
    [InspectorName("Missile Prefab")]
    private GameObject _missilePrefab;

    // User inputs
    private InputAction _acwAction;
    private InputAction _cwAction;
    private InputAction _thrustAction;
    private InputAction _hyperspaceAction;
    private InputAction _fireAction;

    // Components
    private Rigidbody2D _body;
    private Animator _animator;
    private ParticleSystem _phosphorTrailParticleSystem;
    private SpriteRenderer _spriteRenderer;

    // Screen dimensions
    private float _minScreenX;
    private float _maxScreenX;
    private float _minScreenY;
    private float _maxScreenY;

    // Hyperspace
    private bool _hyperspaceAvailable = true;
    private float _timeSinceLastHyperspace;

    // Magnitude of offset of missile spawn relative to player
    private float _missileOffset;

    void Start()
    {
        // Actions
        _acwAction= InputSystem.actions.FindAction(_INPUT_ROTATE_ACW);
        _cwAction = InputSystem.actions.FindAction(_INPUT_ROTATE_CW);
        _thrustAction = InputSystem.actions.FindAction(_INPUT_THRUST);
        _hyperspaceAction=InputSystem.actions.FindAction(_INPUT_HYPERSPACE);
        _fireAction= InputSystem.actions.FindAction(_INPUT_FIRE);

        // Components
        _body = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _phosphorTrailParticleSystem = GetComponent<ParticleSystem>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        // Screen bounds
        var bottomLeft = Camera.main.ScreenToWorldPoint(Vector2.zero);
        _minScreenX = bottomLeft.x;
        _minScreenY = bottomLeft.y;

        var topRight = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        _maxScreenX = topRight.x;
        _maxScreenY = topRight.y;

        // Missiles
        for (int i = 0; i < _missileCount; i++)
        {
            _dormantMissiles.Add(Instantiate<GameObject>(_missilePrefab as GameObject));
        }
        _missileOffset = _spriteRenderer.sprite.bounds.size.y / 2.0f + 
            _dormantMissiles[0].GetComponent<SpriteRenderer>().sprite.bounds.size.y / 2.0f;
    }

    void Update()
    {
        UpdateHyperspaceAvailability();

        KeepOnScreen();

        OrientatePhosphorTrail();

        ClearUpMissiles();

        ProcessInputs();
    }

    private void ProcessInputs()
    {
        // Rotation
        if (_acwAction.IsPressed())
        {
            RotateAWCPressed();
        }
        else if (_cwAction.IsPressed())
        {
            RotateCWPressed();
        }

        // Thrust
        if (_thrustAction.IsPressed())
        {
            _animator.SetBool(_ANIM_IS_THRUSTING_PARAM, true);
            ThrustPressed();
        }
        else
        {
            _animator.SetBool(_ANIM_IS_THRUSTING_PARAM, false);
        }

        // Fire
        if (_fireAction.WasPressedThisFrame())
        {
            FirePressed();
        }

        // Hyperspace
        if (_hyperspaceAction.WasPressedThisFrame())
        {
            HyperspacePressed();
        }
    }

    private void ClearUpMissiles()
    {
        if (_activeMissiles.Count>0)
        {
            var activeMissile=_activeMissiles[0];

            if (Time.time - activeMissile.SpawnTime > 1)
            {
                _dormantMissiles.Add(activeMissile.Missile);
                activeMissile.Missile.SetActive(false);
                _activeMissiles.RemoveAt(0);
            }
        }
    }

    private void FirePressed()
    {
        if (_dormantMissiles.Count > 0)
        {
            var newMissile = _dormantMissiles[0];
            _dormantMissiles.RemoveAt(0);
            _activeMissiles.Add(new ActiveMissile { Missile=newMissile, SpawnTime=Time.time });

            var newMissileBody = newMissile.GetComponent<Rigidbody2D>();

            newMissile.SetActive(true);
            newMissile.transform.position = transform.position + transform.up * _missileOffset;
            newMissileBody.linearVelocity = transform.up * _missileSpeed;  
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
        main.startRotation = -transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
    }

    private void KeepOnScreen()
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
            _timeSinceLastHyperspace = 0f;
        }
    }
}
