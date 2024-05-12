using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.EventSystems.EventTrigger;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

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

    private InputActionAsset _inputAsset;
    private InputActionMap _player;
    private InputAction _aim;
    private InputAction _fire;
    private InputAction _reset;

    private bool _isOnRail;
    private Vector3 _entryVelocity;

    private bool _hasShotRay;
    private float _distanceFromOutOfBounds;

    private bool _hasRecentlyFired;


    void Awake()
    {
        _inputAsset = this.GetComponent<PlayerInput>().actions;
        _player = _inputAsset.FindActionMap("Player");
    }

    void OnEnable()
    {
        _player.FindAction("Fire").started += OnFire;
        _player.FindAction("Stop").started += OnStop;
        //_player.FindAction("SetPosition").started += OnSetPosition;
        _aim = _player.FindAction("Aim");
        _player.Enable();

    }

    public void SetPlayerStartingPosition(int playerCount)
    {
        if (playerCount == 0)
        {
            _playerGFX.rotation = Quaternion.LookRotation(new Vector3(0,90,0));
        }
        _playerRigidbody.position = GameSettings.SpawnPointList[playerCount].position;
        Debug.Log("player" + (playerCount + 1) + ", " + GameSettings.SpawnPointList[playerCount].position);
    }

    void OnDisable()
    {
        _player.FindAction("Fire").started -= OnFire;
        _player.FindAction("Stop").started -= OnStop;
        //_player.FindAction("SetPosition").started -= OnSetPosition;
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

    public static void StartGame()
    {
        foreach (var player in GameSettings.PlayersInGame)
        {
            player.gameObject.GetComponent<PlayerMovementController>().SetPlayerStartingPosition(GameSettings.PlayersInGame.Count - 1);
        }
    }

    public void OnSetPosition(InputAction.CallbackContext ctx)
    {
        for (int i = 0; i < GameSettings.PlayersInGame.Count; i++)
        {
            var player = GameSettings.PlayersInGame[i];
            player.gameObject.transform.position = GameSettings.SpawnPointList[i].position;
            player.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

    void Update()
    {
        GetAimingInput();
        Vector3 tempPosition = SetPlayerFacing();

        if (_usingMouse)
        //TODO: remove mouse controls
        {
            Vector3 centeredMousePos = Input.mousePosition - new Vector3(Screen.width/2f,Screen.height/2f,0);
            Vector3 pointerRotationMouse = new Vector3(0,Mathf.Atan2(centeredMousePos.x, centeredMousePos.y) * Mathf.Rad2Deg, 0);
            _pointerPivot.eulerAngles = pointerRotationMouse;
        }

        KeepOldInputIfNoInput();
        Fire();
        SetPlayerScaleBasedOnHeight();

        _shouldFire = false;
        _previousPosition = tempPosition;
    }

    private void SetPlayerScaleBasedOnHeight()
    {
        if (!(transform.position.y < 0)) return;
        if (!_hasShotRay)
        {
            Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit);
            _hasShotRay = true;
            _distanceFromOutOfBounds = hit.distance;
        }

        _playerGFX.localScale = Vector3.Lerp(Vector3.one, Vector3.one / 10,
            Mathf.Abs(transform.position.y)/ _distanceFromOutOfBounds);
    }

    private void Fire()
    {
        if (_playerRigidbody.velocity.magnitude < _minVelocityToMove)
        {
            _pointer.enabled = true;
            if (!_readyToFire || !_shouldFire) return;
            _readyToFire = false;
            _hasRecentlyFired = true;
            _playerGFX.forward = _pointerPivot.forward;
            _playerRigidbody.AddForce(_playerGFX.forward.normalized * _forceStrength, ForceMode.Impulse);
            Invoke(nameof(ResetFire), 0.25f);
            Invoke(nameof(ResetHasRecentlyFired), 0.10f);
        }
        else
        {
            _pointer.enabled = false;
        }
    }

    private void KeepOldInputIfNoInput()
    {
        // Checking if the input is non-zero, if it isn't we keep the old input direction.
        if (Mathf.Abs(_horizontalInput) > _controllerDeadZone || Mathf.Abs(_verticalInput) > _controllerDeadZone)
        {
            Vector3 pointerRotation = new Vector3(0, Mathf.Atan2(_horizontalInput, _verticalInput) * Mathf.Rad2Deg, 0);
            _pointerPivot.eulerAngles = pointerRotation;
        }
    }

    private Vector3 SetPlayerFacing()
    {
        Vector3 tempPosition = transform.position;
        tempPosition.y = 0.5f;

        if ((tempPosition - _previousPosition).magnitude > 0.01f)
        {
            _playerGFX.forward = (tempPosition - _previousPosition).normalized;
        }

        return tempPosition;
    }

    private void GetAimingInput()
    {
        _horizontalInput = _aim.ReadValue<Vector2>().x;
        _verticalInput = _aim.ReadValue<Vector2>().y;
    }

    void ResetFire()
    {
        _readyToFire = true;
    }

    void ResetHasRecentlyFired()
    {
        _hasRecentlyFired = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("NoBounce"))
        {
            if (_hasRecentlyFired) return;
            _playerRigidbody.velocity = Vector3.zero;
        }

        if (collision.gameObject.tag == "OutOfBounds")
        {
            Respawn();
        }

        //tryout players bouncing
        //if (collision.gameObject.CompareTag("Player"))
        //{
        //    Vector3 collisionNormal = collision.contacts[0].normal;
        //    float collisionAngle = Vector3.Angle(_playerRigidbody.velocity, collision.relativeVelocity);

        //    Vector3 newDirection = Vector3.Reflect(_playerRigidbody.velocity, collisionNormal).normalized;

        //    _playerRigidbody.velocity = newDirection * _forceStrength * Mathf.Cos(collisionAngle * Mathf.Deg2Rad);
        //}
    }

    public void Respawn()
    {
        _playerRigidbody.velocity = Vector3.zero;
        _playerGFX.localScale = Vector3.one;
        _distanceFromOutOfBounds = float.MaxValue;
        _hasShotRay = false;

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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RotatingTile"))
        {
            RotatingTile rotatingTileScript = other.GetComponent<RotatingTile>();
            rotatingTileScript.PlayerYeeter.rotation = _playerGFX.rotation;
            rotatingTileScript.LaunchPlayer(_playerRigidbody);
        }

        if (other.CompareTag("StartGuardRail"))
        {
            _isOnRail = !_isOnRail;
            GuardRail guardRailScript = other.transform.GetComponentInParent<GuardRail>();
            if (_isOnRail)
            {
                StartRail(guardRailScript);
            }
            else
            {
                StopRail(guardRailScript);
            }
        }
    }

    private void StartRail(GuardRail guardRailScript)
    {
        float entry1DistanceFromPlayer = Vector3.Distance(guardRailScript.EntryPoint1.position, transform.position);
        float entry2DistanceFromPlayer = Vector3.Distance(guardRailScript.EntryPoint2.position, transform.position);

        guardRailScript.StartsAt1 = entry1DistanceFromPlayer < entry2DistanceFromPlayer;
        guardRailScript.SetStartingAngle();
        _entryVelocity = _playerRigidbody.velocity;
        _playerRigidbody.velocity = Vector3.zero;
        guardRailScript.StartAnimation = true;
        guardRailScript.PlayerTransform = transform;
        _playerCollider.isTrigger = true;
    }

    private void StopRail(GuardRail guardRailScript)
    {
        _playerRigidbody.velocity = (guardRailScript.StartsAt1 ? guardRailScript.EntryPoint2.forward : guardRailScript.EntryPoint1.forward) * _entryVelocity.magnitude * 1.1f;
        guardRailScript.StartAnimation = false;
        guardRailScript.PlayerTransform = null;
        Invoke(nameof(DelayedColliderReactivation),0.11f);
    }

    private void DelayedColliderReactivation()
    {
        _playerCollider.isTrigger = false;
    }
}

