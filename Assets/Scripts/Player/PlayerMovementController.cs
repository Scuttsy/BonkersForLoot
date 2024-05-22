using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerMovementController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CapsuleCollider _playerCollider;
    [SerializeField] private MeshRenderer _pointer;
    [SerializeField] private Transform _pointerPivot;
    [SerializeField] private Transform _pointerScalePivot;
    [SerializeField] private Transform _playerGFX;
    public Rigidbody PlayerRigidbody;
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private GameObject _coinPrefab;

    [Header("Settings")]
    [SerializeField] private float _minVelocityToMove;
    [Range(0, 1)]
    [SerializeField] private float _controllerDeadZone;
    [SerializeField] private float _fireCooldown;
    [SerializeField] private bool _usingMouse;
    [SerializeField] private float _impactFreezeTime;
    [Min(0.001f)]
    [SerializeField] private float _impactFreezeDivider;

    [SerializeField] private Vector3 _minPointerSize;
    [SerializeField] private Vector3 _maxPointerSize;
    [SerializeField] private float _timeForMaxShot;
    [SerializeField] private float _minForceStrength;
    [SerializeField] private float _maxForceStrength;

    [SerializeField] private float _respawnTimer;


    [Header("Step settings")]
    [SerializeField] private Transform _stepRayCastLower;
    [SerializeField] private Transform _stepRayCastUpper;
    [SerializeField] private float _stepHeight;
    [SerializeField] private float _stepSmooth;
    [SerializeField] private float _lowerRayCastMaxDistance = .3f;
    [SerializeField] private float _upperRayCastMaxDistance = .4f;


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
    private bool _isHoldingDownFire;
    private float _chargeTimer;

    private bool _isOnRail;
    private Vector3 _entryVelocity;

    private bool _hasShotRay;
    private float _distanceFromOutOfBounds;

    private bool _hasRecentlyFired;

    private AudioSource _audioSource;

    private float _impactVelocityAfterFreeze;
    private float _impactVelocityDuringFreeze;
    private bool _isInImpactFreeze;

    [HideInInspector] public RotatingCarrier RotatingCarrier;

    void Awake()
    {
        GameSettings.PlayersInGame.Add(_playerInput);
        _inputAsset = _playerInput.actions;
        _player = _inputAsset.FindActionMap("Player");
        SetPlayerStartingPosition(GameSettings.PlayersInGame.Count - 1);

        Vector3 stepRayCastUpper = _stepRayCastUpper.localPosition;
        stepRayCastUpper.y += _stepHeight;
        _stepRayCastUpper.localPosition = stepRayCastUpper;
    }

    void OnEnable()
    {
        _player.FindAction("Fire").started += OnFireStart;
        _player.FindAction("Fire").canceled += OnFireStop;
        _player.FindAction("Stop").started += OnStopPressed;
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
        PlayerRigidbody.position = GameSettings.SpawnPointList[playerCount].position;
    }

    void OnDisable()
    {
        _player.FindAction("Fire").started -= OnFireStart;
        _player.FindAction("Fire").canceled -= OnFireStop;
        _player.FindAction("Stop").started -= OnStopPressed;
        //_player.FindAction("SetPosition").started -= OnSetPosition;
        _player.Disable();
    }

    public void OnFireStart(InputAction.CallbackContext ctx)
    {
        _isHoldingDownFire = true;

    }
    public void OnFireStop(InputAction.CallbackContext ctx)
    {
        _isHoldingDownFire = false;
        _shouldFire = true;
    }

    public void OnStopPressed(InputAction.CallbackContext ctx)
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }

    public static void StartGame()
    {
        foreach (var player in GameSettings.PlayersInGame)
        {
            PlayerMovementController playerMovementScript = player.gameObject.GetComponent<PlayerMovementController>();
            playerMovementScript.SetPlayerStartingPosition(GameSettings.PlayersInGame.Count - 1);
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
        if (RotatingCarrier != null)
        {
            //Debug.Log(Math.Abs(PlayerRigidbody.velocity.magnitude / Vector3.Distance(transform.position, RotatingCarrier.CarrierRigidbody.position) - RotatingCarrier.AngularVelocity) < 0.01f);
            //Debug.Log("Velocity: " + PlayerRigidbody.velocity.magnitude);
            //Debug.Log("AngularVelocity: " + PlayerRigidbody.angularVelocity.magnitude);
            //Debug.Log("CarrierVelocity: " + RotatingCarrier.AngularVelocity);
            //Debug.Log("Pointer: " + Mathf.Abs(0.054f * _minVelocityToMove * RotatingCarrier.transform.localScale.x * RotatingCarrier.RotationSpeed));
        }

        GetAimingInput();
        Vector3 tempPosition = SetPlayerFacing();

        if (_usingMouse)
        //TODO: remove mouse controls
        {
            Vector3 centeredMousePos = Input.mousePosition - new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
            Vector3 pointerRotationMouse = new Vector3(0, Mathf.Atan2(centeredMousePos.x, centeredMousePos.y) * Mathf.Rad2Deg, 0);
            _pointerPivot.eulerAngles = pointerRotationMouse;
        }

        KeepOldInputIfNoInput();
        Fire();
        ChargeFire();
        SetPlayerScaleBasedOnHeight();

        if (_isInImpactFreeze)
            PlayerRigidbody.velocity = PlayerRigidbody.velocity.normalized * _impactVelocityDuringFreeze;

        _shouldFire = false;
        _previousPosition = tempPosition;
    }


    private void SetPlayerScaleBasedOnHeight()
    {
        if (!(transform.position.y < 0)) return;
        bool hitFloor = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit);
        if (hitFloor)
        {
            if (hit.distance < 1) return;
            _distanceFromOutOfBounds = hit.distance;
            _playerGFX.localScale = Vector3.Lerp(Vector3.one, Vector3.one / 10,
                Mathf.Abs(transform.position.y) / _distanceFromOutOfBounds);
        }
        else
        {
            Debug.LogError("FloorRaycast hit nothing!");
        }

    }

    private bool PlayerSlowEnoughForPointer => PlayerRigidbody.velocity.magnitude < _minVelocityToMove;
    private bool IsPointerActive => (!_isInImpactFreeze && PlayerSlowEnoughForPointer) || PointerActiveOnRotatingPlatform();

    private bool PointerActiveOnRotatingPlatform()
    {
        if (RotatingCarrier == null) return false;

        return (PlayerRigidbody.velocity.magnitude <= Mathf.Abs(0.054f * _minVelocityToMove * RotatingCarrier.transform.localScale.x * RotatingCarrier.RotationSpeed));
    }

    private void Fire()
    {
        if (IsPointerActive)
        {
            _pointer.enabled = true;
            if (!_readyToFire || !_shouldFire) return;
            _readyToFire = false;
            _hasRecentlyFired = true;
            _playerGFX.forward = _pointerPivot.forward;
            float ratio = _chargeTimer / _timeForMaxShot;
            ratio = Mathf.Clamp01(ratio);
            float force = Mathf.Lerp(_minForceStrength, _maxForceStrength, ratio);
            PlayerRigidbody.AddForce(_playerGFX.forward.normalized * force, ForceMode.Impulse);
            Invoke(nameof(ResetFire), 0.25f);
            Invoke(nameof(ResetHasRecentlyFired), 0.10f);
        }
        else
        {
            _pointer.enabled = false;
        }
    }
    private void ChargeFire()
    {
        if (_isHoldingDownFire && IsPointerActive)
        {
            _chargeTimer += Time.deltaTime;
            float ratio = _chargeTimer / _timeForMaxShot;
            ratio = Mathf.Clamp01(ratio);
            _pointerScalePivot.localScale = Vector3.Lerp(_minPointerSize, _maxPointerSize, ratio);
        }
        else
        {
            _chargeTimer = 0f;
            _pointerScalePivot.localScale = _minPointerSize;
        }
    }


    public void Fire(Vector3 direction)
    {
        _pointer.enabled = true;

        _playerGFX.forward = direction;
        PlayerRigidbody.AddForce(_playerGFX.forward.normalized * PlayerRigidbody.velocity.magnitude, ForceMode.Impulse);
        Invoke(nameof(ResetFire), 0.25f);
        Invoke(nameof(ResetHasRecentlyFired), 0.10f);
       
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

    private void ResetFire()
    {
        _readyToFire = true;
    }

    private void ResetHasRecentlyFired()
    {
        _hasRecentlyFired = false;
    }

    private void SetMotorSpeeds(float lowFrequency, float highFrequency, float resetSpeed)
    {
        Debug.Log("Motor");
        var controller = (Gamepad)_playerInput.devices[0];
        if (controller == null) return;
        controller.SetMotorSpeeds(lowFrequency, highFrequency);
        CancelInvoke(nameof(ResetMotorSpeeds));
        Invoke(nameof(ResetMotorSpeeds),resetSpeed);
    }

    private void ResetMotorSpeeds()
    {
        Debug.Log("no motor");
        var controller = (Gamepad)_playerInput.devices[0];
        controller.ResetHaptics();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("GroundPlane"))
        {
            Debug.Log("hit floor with " + collision.contactCount + " collisions");
            foreach (ContactPoint contact in collision.contacts)
            {
                Debug.DrawLine(contact.point,contact.point + Vector3.up/5, Color.black,30f);
            }
        }

        if (collision.gameObject.CompareTag("BouncyWall"))
        {
            SetMotorSpeeds(0.2f, 0.3f, 0.25f);
        }

        if (collision.gameObject.CompareTag("NoBounce"))
        {
            if (_hasRecentlyFired) return;
            PlayerRigidbody.velocity = Vector3.zero;
            SetMotorSpeeds(0.2f, 0.3f, 0.25f);
        }

        if (collision.gameObject.tag == "OutOfBounds")
        {
            CancelInvoke(nameof(Invoke));
            Invoke(nameof(Respawn), _respawnTimer);
            SetMotorSpeeds(0.4f, 0.5f, 0.5f);
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            if (_audioSource != null && _audioSource.clip != null)
            {
                _audioSource.PlayOneShot(_audioSource.clip);
            }

            // 0 time = called next frame right before next Update()
            Invoke(nameof(ImpactFreeze),0);

            SetMotorSpeeds(0.6f, 0.7f, 0.5f);
        }

        //Coins explode
        //int playerScore = gameObject.GetComponent<Player>().Score;
        //int coinsToLose = _numCoinsImpact <= playerScore ? _numCoinsImpact : playerScore;
        //gameObject.GetComponent<Player>().Score -= coinsToLose;
        //ExplodeCoins(transform, coinsToLose);
    }

    private void StepUp()
    {
        PlayerRigidbody.position += Vector3.up * _stepSmooth;
    }

    private bool HasCollidedWithStepable()
    {
        Vector3 ray45LDir = (_playerGFX.forward - _playerGFX.right).normalized;
        Vector3 ray45RDir = (_playerGFX.forward + _playerGFX.right).normalized;
        Vector3 rayFDir = _playerGFX.forward;

        bool lowerRayHit45L = Physics.Raycast(_stepRayCastLower.position, ray45LDir, out RaycastHit hitLower, _lowerRayCastMaxDistance);
        bool lowerRayHit45R = Physics.Raycast(_stepRayCastLower.position, ray45RDir, out hitLower, _lowerRayCastMaxDistance);
        bool lowerRayHitF = Physics.Raycast(_stepRayCastLower.position, rayFDir, out hitLower, _lowerRayCastMaxDistance);
        bool upperRayHit45L = Physics.Raycast(_stepRayCastUpper.position, ray45LDir, out RaycastHit hitUpper, _upperRayCastMaxDistance);
        bool upperRayHit45R = Physics.Raycast(_stepRayCastUpper.position, ray45RDir, out hitUpper, _upperRayCastMaxDistance);
        bool upperRayHitF = Physics.Raycast(_stepRayCastUpper.position, rayFDir, out hitUpper, _upperRayCastMaxDistance);

        #region DebugRaycasts

        Debug.DrawLine(_stepRayCastLower.position, _stepRayCastLower.position + ray45LDir * _lowerRayCastMaxDistance, Color.green, 3f);
        Debug.DrawLine(_stepRayCastLower.position, _stepRayCastLower.position + ray45RDir * _lowerRayCastMaxDistance, Color.green, 3f);
        Debug.DrawLine(_stepRayCastLower.position, _stepRayCastLower.position + rayFDir * _lowerRayCastMaxDistance, Color.green, 3f);
        Debug.DrawLine(_stepRayCastUpper.position, _stepRayCastUpper.position + ray45LDir * _upperRayCastMaxDistance, Color.red, 3f);
        Debug.DrawLine(_stepRayCastUpper.position, _stepRayCastUpper.position + ray45RDir * _upperRayCastMaxDistance, Color.red, 3f);
        Debug.DrawLine(_stepRayCastUpper.position, _stepRayCastUpper.position + rayFDir * _upperRayCastMaxDistance, Color.red, 3f);

        #endregion

        bool lowerRayCast = (lowerRayHitF || lowerRayHit45L || lowerRayHit45R);
        bool upperRayCast = (upperRayHitF || upperRayHit45L || upperRayHit45R);
        Debug.Log("Upper Ray Hit: " + upperRayCast);
        Debug.Log("Lower Ray Hit: " + lowerRayCast);


        if (upperRayCast && lowerRayCast)
            Debug.Log("Not same collider: " + !hitLower.collider.Equals(hitUpper.collider));
        if (upperRayCast) Debug.Log(hitUpper.collider.name);
        if (lowerRayCast) Debug.Log(hitLower.collider.name);

        bool returnUpper = !upperRayCast;
        if (upperRayCast)
            returnUpper = !hitLower.collider.Equals(hitUpper.collider);

        return lowerRayCast && returnUpper;
    }

    private void ImpactFreeze()
    {
        _impactVelocityAfterFreeze = PlayerRigidbody.velocity.magnitude;
        _impactVelocityDuringFreeze = PlayerRigidbody.velocity.magnitude / _impactFreezeDivider;
        Invoke(nameof(EndImpactFreeze), _impactFreezeTime);
        _isInImpactFreeze = true;
    }

    private void EndImpactFreeze()
    {
        PlayerRigidbody.velocity = PlayerRigidbody.velocity.normalized * _impactVelocityAfterFreeze;
        _isInImpactFreeze = false;
    }

    private void ExplodeCoins(Transform collision, int coins)
    {
        int maxLoops = 5;
        Vector3 targetPosition = Vector3.zero;
        Ray ray;
        GameObject coin;

        for (int i = 0; i < coins - 1; i++)
        {
            coin = Instantiate(_coinPrefab, collision.position, Quaternion.identity);
            int loops = 0;
            do
            {
                loops++;
                Vector2 pos = UnityEngine.Random.insideUnitCircle;
                targetPosition = transform.position + new Vector3(pos.x / 2, 0, pos.y / 2);

                ray = new Ray(collision.position, targetPosition);
                Debug.DrawRay(collision.position, ray.direction);
            } while (Physics.Raycast(ray, (targetPosition - transform.position).magnitude) && loops <= maxLoops);

            coin.GetComponent<Loot>()._ExplodeTarget = targetPosition;
        }
    }

    public void Respawn()
    {
        PlayerRigidbody.velocity = Vector3.zero;
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
            rotatingTileScript.LaunchPlayer(PlayerRigidbody);
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

        if (other.gameObject.tag == "OutOfBounds")
        {
            _playerCollider.isTrigger = false;
            Respawn();
        }

        if (other.gameObject.tag == "LobbyOutOfBounds")
        {
            Vector3 playerPosition = PlayerRigidbody.position;
            playerPosition.y = 1.17f;
            PlayerRigidbody.position = playerPosition;

            Vector3 playerVelocity = PlayerRigidbody.velocity;
            playerVelocity.y = 0f;
            PlayerRigidbody.velocity = playerVelocity;
        }
    }

    // Added OnTriggerStay to be 100% sure it fires off.
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "LobbyOutOfBounds")
        {
            Vector3 playerPosition = PlayerRigidbody.position;
            playerPosition.y = 1.17f;
            PlayerRigidbody.position = playerPosition;

            Vector3 playerVelocity = PlayerRigidbody.velocity;
            playerVelocity.y = 0f;
            PlayerRigidbody.velocity = playerVelocity * PlayerRigidbody.velocity.magnitude;
        }
    }

    private void StartRail(GuardRail guardRailScript)
    {
        float entry1DistanceFromPlayer = Vector3.Distance(guardRailScript.EntryPoint1.position, transform.position);
        float entry2DistanceFromPlayer = Vector3.Distance(guardRailScript.EntryPoint2.position, transform.position);

        guardRailScript.StartsAt1 = entry1DistanceFromPlayer < entry2DistanceFromPlayer;
        guardRailScript.SetStartingAngle();
        _entryVelocity = PlayerRigidbody.velocity;
        PlayerRigidbody.velocity = Vector3.zero;
        guardRailScript.StartAnimation = true;
        guardRailScript.PlayerRigidbody = PlayerRigidbody;
        guardRailScript.PlayerStartPos = transform.position;
        _playerCollider.isTrigger = true;
    }

    private void StopRail(GuardRail guardRailScript)
    {
        PlayerRigidbody.velocity = (guardRailScript.StartsAt1 ? guardRailScript.EntryPoint2.forward : guardRailScript.EntryPoint1.forward) * _entryVelocity.magnitude * 1.1f;
        guardRailScript.StartAnimation = false;
        guardRailScript.PlayerRigidbody = null;
        Invoke(nameof(DelayedColliderReactivation), 0.11f);
    }

    private void DelayedColliderReactivation()
    {
        _playerCollider.isTrigger = false;
    }
}

