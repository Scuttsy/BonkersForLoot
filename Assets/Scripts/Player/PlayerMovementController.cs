using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Collider _playerCollider;
    [SerializeField] private Transform _pointerPivot;
    [SerializeField] private MeshRenderer _pointer;
    [SerializeField] private Transform _playerGFX;
    [SerializeField] private Rigidbody _playerRigidbody;
    [SerializeField] private PlayerInput _playerInput;

    [Header("Settings")]
    [SerializeField] private float _forceStrength;
    [SerializeField] private float _minVelocityToMove;
    [Range(0,1)]
    [SerializeField] private float _controllerDeadZone;
    [SerializeField] private float _fireCooldown;
    [SerializeField] private bool _usingMouse;

    private float _horizontalInput;
    private float _verticalInput;

    private bool _shouldFire;
    private bool _readyToFire = true;
    private Vector3 _previousPosition;

    private InputActionAsset inputAsset;
    private InputActionMap _player;

    private InputAction _aim;
    private InputAction _fire;

    void Awake()
    {
        inputAsset = this.GetComponent<PlayerInput>().actions;
        _player = inputAsset.FindActionMap("Player");
    }

    void OnEnable()
    {
        _player.FindAction("Fire").started += OnFire;
        _player.FindAction("Stop").started += OnStop;
        _aim = _player.FindAction("Aim");
        _player.Enable();
    }

    void OnDisable()
    {
        _player.FindAction("Fire").started -= OnFire;
        _player.FindAction("Stop").started -= OnStop;
        _player.Disable();
    }

    public void OnFire(InputAction.CallbackContext ctx)
    {
        _shouldFire = ctx.action.IsPressed();
    }
    public void OnStop(InputAction.CallbackContext ctx)
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }

    void Update()
    {
        _horizontalInput = _aim.ReadValue<Vector2>().x;
        _verticalInput = _aim.ReadValue<Vector2>().y;

        Vector3 tempPosition = transform.position;
        tempPosition.y = 0.5f;

        if ((tempPosition - _previousPosition).magnitude > 0.01f)
        {
            _playerGFX.forward = (tempPosition - _previousPosition).normalized;
        }

        if (_usingMouse)
        //TODO: remove mouse controls
        {
            Vector3 centeredMousePos = Input.mousePosition - new Vector3(Screen.width/2f,Screen.height/2f,0);
            Vector3 pointerRotationMouse = new Vector3(0,Mathf.Atan2(centeredMousePos.x, centeredMousePos.y) * Mathf.Rad2Deg, 0);
            _pointerPivot.eulerAngles = pointerRotationMouse;
        }


        // Checking if the input is non-zero, if it isn't we keep the old input direction.
        if (Mathf.Abs(_horizontalInput) > _controllerDeadZone || Mathf.Abs(_verticalInput) > _controllerDeadZone)
        {
            Vector3 pointerRotation = new Vector3(0, Mathf.Atan2(_horizontalInput, _verticalInput) * Mathf.Rad2Deg, 0);
            _pointerPivot.eulerAngles = pointerRotation;
        }

        if (_playerRigidbody.velocity.magnitude < _minVelocityToMove)
        {
            _pointer.enabled = true;
            if (_readyToFire && _shouldFire)
            {
                _shouldFire = false;
                _readyToFire = false;
                _playerGFX.forward = _pointerPivot.forward;
                _playerRigidbody.AddForce(_playerGFX.forward.normalized * _forceStrength, ForceMode.Impulse);
                Invoke(nameof(ResetFire), 0.25f);
            }
        }
        else
        {
            _pointer.enabled = false;
        }
        
        _previousPosition = tempPosition;
    }

    void ResetFire()
    {
        _readyToFire = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "NoBounce")
        {
            _playerRigidbody.velocity = Vector3.zero;
        }

        if (collision.gameObject.tag == "OutOfBounds")
        {
            Respawn();
        }

        //tryout players bouncing
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector3 collisionNormal = collision.contacts[0].normal;
            float collisionAngle = Vector3.Angle(_playerRigidbody.velocity, collision.relativeVelocity);

            Vector3 newDirection = Vector3.Reflect(_playerRigidbody.velocity, collisionNormal).normalized;

            _playerRigidbody.velocity = newDirection * _forceStrength * Mathf.Cos(collisionAngle * Mathf.Deg2Rad);
        }
    }

    public void Respawn()
    {
        _playerRigidbody.velocity = Vector3.zero;

        // respawn mechanic taking into consideration the positions of the other player,
        // it calculates per spawnpoint the closest distance, and then gets the furthest spawmpoint;
        SpawnPointData closestSpawnPoint = new SpawnPointData(null, 0);
        foreach (Transform spawnPoint in GameSettings.SpawnPointList)
        {
            float closestDistance = float.MaxValue;
            foreach (PlayerInput player in GameSettings.PlayersInGame)
            {
                if (player == this._playerInput) continue;
                float distance =
                    Vector3.Distance(spawnPoint.transform.position, player.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                }
            }

            if (closestDistance > closestSpawnPoint.FurthestDistanceToPlayer)
            {
                closestSpawnPoint.FurthestDistanceToPlayer = closestDistance;
                closestSpawnPoint.SpawnPointName = spawnPoint;
            }
        }

        if (closestSpawnPoint.SpawnPointName == null)
            throw new System.AccessViolationException("SpawnPoint was null");
        transform.position = closestSpawnPoint.SpawnPointName.transform.position;
        Debug.Log("Respawned.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "RotatingTile")
        {
            Vector3 playerVelocity = other.transform.forward * _playerRigidbody.velocity.magnitude;
            _playerRigidbody.velocity = playerVelocity;
        }
    }
}

