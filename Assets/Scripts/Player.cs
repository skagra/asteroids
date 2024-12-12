using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public sealed class Player : MonoBehaviour
{
    private sealed class ActiveMissile
    {
        public GameObject Missile { get; set; }
        public float SpawnTime { get; set; }
    }

    public event Action Exploded;
    public event Action Exploding;

    // Input names
    private const string _INPUT_ROTATE_ACW = "Rotate ACW";
    private const string _INPUT_ROTATE_CW = "Rotate CW";
    private const string _INPUT_THRUST = "Thrust";
    private const string _INPUT_HYPERSPACE = "Hyperspace";
    private const string _INPUT_FIRE = "Fire";

    // Animation parameters
    private const string _ANIM_IS_THRUSTING_PARAM = "Is Thrusting";
    private const string _ANIM_IS_EXPLODING_TRIGGER = "Is Exploding";

    // Lists of active and dormant missiles
    private readonly List<GameObject> _dormantMissiles = new();
    private readonly List<ActiveMissile> _activeMissiles = new();

    // Values customisable in the Unity Inspector
    [Header("Ship")]
    [SerializeField]
    private float _rotationSpeed;
    [SerializeField]
    private float _acceleration;
    [SerializeField]
    private float _speedLimit;
    [SerializeField]
    private float _dampening;

    [Header("Missile")]
    [SerializeField]
    private GameObject _missilePrefab;
    [SerializeField]
    private float _missileSpeed;
    [SerializeField]
    private int _missileCount;
    [SerializeField]
    private float _missileDuration;
    [SerializeField]

    [Header("Hyperspace")]
    private float _hyperspaceBorder;
    [SerializeField]
    private float _hyperspaceCooldown;

    [Header("References")]
    [SerializeField]
    private AudioHub _audioHub;

    // User inputs
    private InputAction _acwAction;
    private InputAction _cwAction;
    private InputAction _thrustAction;
    private InputAction _hyperspaceAction;
    private InputAction _fireAction;

    // Components
    private Rigidbody2D _body;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;

    // Hyperspace
    private bool _hyperspaceAvailable = true;
    private float _timeSinceLastHyperspace;

    // Magnitude of offset of missile spawn relative to player
    private float _missileOffset;

    // This is ensure that no more than one collision with the ship is flagged each frame
    private bool _shipFlaggedAsDestroyedThisFrame = false;
    private bool _isExploding = false;

    private void Awake()
    {
        // Actions
        _acwAction = InputSystem.actions.FindAction(_INPUT_ROTATE_ACW);
        _cwAction = InputSystem.actions.FindAction(_INPUT_ROTATE_CW);
        _thrustAction = InputSystem.actions.FindAction(_INPUT_THRUST);
        _hyperspaceAction = InputSystem.actions.FindAction(_INPUT_HYPERSPACE);
        _fireAction = InputSystem.actions.FindAction(_INPUT_FIRE);

        // Components
        _body = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();

        // Missiles
        for (var i = 0; i < _missileCount; i++)
        {
            var dormantMissile = Instantiate(_missilePrefab as GameObject);
            _dormantMissiles.Add(dormantMissile);
            var missileScript = dormantMissile.GetComponent<Missile>();
            missileScript.CollidedWithAsteroid += MissileHasHitAsteroid;

        }
        _missileOffset = _spriteRenderer.sprite.bounds.size.y / 2.0f +
            _dormantMissiles[0].GetComponent<SpriteRenderer>().sprite.bounds.size.y / 2.0f;
    }

    private void MissileHasHitAsteroid(GameObject missile)
    {
        var activeMissile = _activeMissiles.Find(am => am.Missile == missile);
        // The missile might have hit more than one asteroids in one frame
        if (activeMissile != null)
        {
            activeMissile.Missile.SetActive(false);
            _activeMissiles.Remove(activeMissile);
            _dormantMissiles.Add(activeMissile.Missile);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!_isExploding)
        {
            var collidedWith = collider.gameObject;
            if (collidedWith.layer == Layers.LayerMaskAsteroid)
            {
                if (!_shipFlaggedAsDestroyedThisFrame)
                {
                    _animator.SetTrigger(_ANIM_IS_EXPLODING_TRIGGER);
                    _shipFlaggedAsDestroyedThisFrame = true;
                    _audioHub.PlayLargeExplosion();
                    _isExploding = true;
                    _collider.enabled = false;
                    Exploding?.Invoke();
                }
            }
            else
            {
                Debug.LogWarning($"Erroneous collision flagged by Player with {collidedWith.name}.");
            }
        }
    }

    private void OnEnable()
    {
        transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        _body.linearVelocity = Vector3.zero;
        _isExploding = false;
        _collider.enabled = true;
    }

    private void Update()
    {
        _shipFlaggedAsDestroyedThisFrame = false;

        UpdateHyperspaceAvailability();

        KeepOnScreen();

        ClearUpMissiles();

        ProcessInputs();
    }

    private void ProcessInputs()
    {
        if (!_isExploding)
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
                _body.linearDamping = _dampening;
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
    }

    private void ClearUpMissiles()
    {
        // Move missiles that have timed out to dormant state
        for (var missileIndex = _activeMissiles.Count - 1; missileIndex >= 0; missileIndex--)
        {
            var activeMissile = _activeMissiles[missileIndex];

            // Has the missile aged out?
            if (Time.time - activeMissile.SpawnTime > _missileDuration)
            {
                // Make the missile dormant
                _dormantMissiles.Add(activeMissile.Missile);
                activeMissile.Missile.SetActive(false);
                _activeMissiles.RemoveAt(missileIndex);
            }
        }
    }

    private void FirePressed()
    {
        if (_dormantMissiles.Count > 0)
        {
            _audioHub.PlayFire();

            // Remove missile from the dormant list and add to the active list
            var newMissile = _dormantMissiles[0];
            _dormantMissiles.RemoveAt(0);
            _activeMissiles.Add(new ActiveMissile { Missile = newMissile, SpawnTime = Time.time });

            newMissile.SetActive(true);
            // Configure the active missile
            var newMissileBody = newMissile.GetComponent<Rigidbody2D>();

            // Position missile beyond the front of the ship
            newMissile.transform.position = transform.position + transform.up * _missileOffset;
            // Set direction and speed of the missile
            newMissileBody.linearVelocity = _body.linearVelocity + (((Vector2)transform.up) * _missileSpeed);
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

    // Called via an animation event
#pragma warning disable IDE0051 // Remove unused private members
    private void ExplosionCompleted()
#pragma warning restore IDE0051 // Remove unused private members
    {
        _isExploding = false;
        Exploded?.Invoke();
    }

    private void KeepOnScreen()
    {
        transform.position = ScreenUtils.Instance.Adjust(transform.position);
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
        _audioHub.PlayThrust();

        // Calculate the new velocity
        var acceleratedVelocity = new Vector2(_body.linearVelocity.x + transform.up.x * _acceleration * Time.deltaTime,
            _body.linearVelocity.y + transform.up.y * _acceleration * Time.deltaTime);

        // Restrict the maximum speed
        _body.linearVelocity = Vector2.ClampMagnitude(acceleratedVelocity, _speedLimit);

        // So we can accelerate to max velocity unimpeded
        _body.linearDamping = 0.0f;
    }

    private void HyperspacePressed()
    {
        if (_hyperspaceAvailable)
        {
            transform.position = new Vector2(Random.Range(ScreenUtils.Instance.MinScreenX + _hyperspaceBorder, ScreenUtils.Instance.MaxScreenX - _hyperspaceBorder),
                Random.Range(ScreenUtils.Instance.MinScreenY + _hyperspaceBorder, ScreenUtils.Instance.MaxScreenY - _hyperspaceBorder));

            _hyperspaceAvailable = false;
            _timeSinceLastHyperspace = 0f;
        }
    }
}
