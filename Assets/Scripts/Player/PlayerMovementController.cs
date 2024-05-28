using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerMovementController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CapsuleCollider _playerCollider;
    [SerializeField] private CapsuleCollider _playerRespawnCollider;
    [SerializeField] private MeshRenderer _pointer;
    [SerializeField] private Transform _pointerPivot;
    [SerializeField] private Transform _pointerScalePivot;
    [SerializeField] private Transform _playerGFX;
    public Rigidbody PlayerRigidbody;
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private GameObject _coinPrefab;
    [SerializeField] private Player _playerScript;
    [SerializeField] private TMP_Text _respawnTimerVisuals;
    [SerializeField] private GameObject _respawnTimerObj;

    [Header("Settings")]
    [SerializeField] private float _minVelocityToMove;
    [Range(0, 1)]
    [SerializeField] private float _controllerDeadZone;
    [SerializeField] private float _fireCooldown;
    [SerializeField] private bool _usingMouse;

    [SerializeField] private Vector3 _minPointerSize;
    [SerializeField] private Vector3 _maxPointerSize;
    [SerializeField] private float _timeForMaxShot;
    [SerializeField] private float _minForceStrength;
    [SerializeField] private float _maxForceStrength;

    [SerializeField] private float _timeNeededToRespawn;
    [SerializeField] private int _maxCoinsLostOnPlayerImpact;
    [Range(0.02f,1f)]
    [SerializeField] private float _percentageOfCoinsLostOnPlayerImpact;
    [SerializeField] private float _maxExplosionRadius;
    [SerializeField] private float _minExplosionRadius;

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

    private float _distanceFromOutOfBounds;

    private bool _hasRecentlyFired;


    [Header("Audio")]
    [SerializeField]
    private AudioClip _audioClipBonk;
    [SerializeField]
    private AudioClip _audioClipFall;
    private AudioSource _audioSource;

    [HideInInspector] public bool IsRespawning;
    private float _respawningTimer;

    [HideInInspector] public GameplayScene GameplaySceneScript;

    [HideInInspector] public RotatingCarrier RotatingCarrier;

    private float _playerImpactCoinsDivider;

    void Awake()
    {
        
        GameSettings.PlayersInGame.Add(_playerInput);
        _playerScript.PlayerName = $"Player {GameSettings.PlayersInGame.Count}";
        SetCameraLayerMask();
        _inputAsset = _playerInput.actions;
        _player = _inputAsset.FindActionMap("Player");
        SetPlayerStartingPosition(GameSettings.PlayersInGame.Count - 1);

        _respawnTimerObj.SetActive(false);
        _audioSource = GetComponent<AudioSource>();
        _playerRespawnCollider.enabled = false;
    }

    private void SetCameraLayerMask()
    {
        GetComponentInChildren<Camera>().cullingMask = LayerMask.GetMask(_playerScript.PlayerName);

    }

    void OnEnable()
    {
        _player.FindAction("Fire").started += OnFireStart;
        _player.FindAction("Fire").canceled += OnFireStop;
        _player.FindAction("Stop").started += OnStopPressed;
        _aim = _player.FindAction("Aim");
        _player.Enable();

        _playerImpactCoinsDivider = (1 / _percentageOfCoinsLostOnPlayerImpact);
    }

    public void SetPlayerStartingPosition(int playerCount)
    {
        if (playerCount == 0)
        {
            _playerGFX.rotation = Quaternion.LookRotation(new Vector3(0,90,0));
        }
        PlayerRigidbody.position = GameSettings.SpawnPointList[playerCount].position;
        Debug.Log("setting player" + playerCount + " location to: " + GameSettings.SpawnPointList[playerCount].position);
    }

    void OnDisable()
    {
        _player.FindAction("Fire").started -= OnFireStart;
        _player.FindAction("Fire").canceled -= OnFireStop;
        _player.FindAction("Stop").started -= OnStopPressed;
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
        for (int i = 0; i < GameSettings.PlayersInGame.Count; i++)
        {
            var player = GameSettings.PlayersInGame[i];
            PlayerMovementController playerMovementScript = player.gameObject.GetComponent<PlayerMovementController>();
            playerMovementScript.SetPlayerStartingPosition(i);
        }
    }

    public static void SetGamePlayScene(GameplayScene gameplayScene)
    {
        foreach (var player in GameSettings.PlayersInGame)
        {
            PlayerMovementController playerMovementScript = player.gameObject.GetComponent<PlayerMovementController>();
            playerMovementScript.GameplaySceneScript = gameplayScene;
        }
    }


    void Update()
    {
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

        if (IsRespawning) DuringRespawn();

        _shouldFire = false;
        _previousPosition = tempPosition;
    }

    private void FixedUpdate()
    {
        
    }

    private void DuringRespawn()
    {
        _respawningTimer -= Time.deltaTime;
        if (_respawningTimer < 0)
        {
            IsRespawning = false;
            _respawnTimerObj.SetActive(false);
            PlayerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            _playerRespawnCollider.enabled = false;
            return;
        }

        float ratio = _respawningTimer / _timeNeededToRespawn;
        _playerGFX.localScale = Vector3.Lerp(Vector3.one / 1000, Vector3.one, 1-ratio);
        float seconds = Mathf.CeilToInt(_respawningTimer % 60);
        _respawnTimerVisuals.text = "" + seconds;
    }


    private void SetPlayerScaleBasedOnHeight()
    {
        if (!(transform.position.y < 0)) return;
        bool hitFloor = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit);
        if (hitFloor)
        {
            if (hit.distance < 1) return;
            _distanceFromOutOfBounds = hit.distance;
            _playerGFX.localScale = Vector3.Lerp(Vector3.one, Vector3.one / 1000,
                Mathf.Abs(transform.position.y) / _distanceFromOutOfBounds);
        }

    }

    private bool IsPointerActive => (PlayerRigidbody.velocity.magnitude < _minVelocityToMove || PointerActiveOnRotatingPlatform()) && !IsRespawning;

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
        var controller = (Gamepad)_playerInput.devices[0];
        if (controller == null) return;
        controller.SetMotorSpeeds(lowFrequency, highFrequency);
        CancelInvoke(nameof(ResetMotorSpeeds));
        Invoke(nameof(ResetMotorSpeeds),resetSpeed);
    }

    private void ResetMotorSpeeds()
    {
        var controller = (Gamepad)_playerInput.devices[0];
        controller.ResetHaptics();
    }

    private void PlayAudioClip(AudioClip clip)
    {
        if (_audioSource != null && clip != null)
        {
            _audioSource.clip = clip;
            _audioSource.PlayOneShot(clip);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("BouncyWall"))
        {
            SetMotorSpeeds(0.2f, 0.3f, 0.25f);
            PlayAudioClip(_audioClipBonk);
        }

        if (collision.gameObject.CompareTag("NoBounce"))
        {
            if (_hasRecentlyFired) return;
            PlayerRigidbody.velocity = Vector3.zero;
            SetMotorSpeeds(0.2f, 0.3f, 0.25f);
        }

        if (collision.gameObject.tag == "OutOfBounds")
        {
            if (!HasGameStarted()) return;
            _playerScript.LoseUnclaimedLoot();
            PlayAudioClip(_audioClipFall);
            CancelInvoke(nameof(Respawn));
            Invoke(nameof(Respawn), 0.5f);
            SetMotorSpeeds(0.4f, 0.5f, 0.5f);
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            if (IsRespawning) return;
            if (collision.gameObject.GetComponent<PlayerMovementController>().IsRespawning) return;
            SetMotorSpeeds(0.6f, 0.7f, 0.5f);
            PlayAudioClip(_audioClipBonk);


            //Coins explode
            int playerScore = _playerScript.UnclaimedLoot;

                // Code for exploding a set amount of coins
                //int coinsToLose = Math.Min(_maxCoinsLostOnPlayerImpact, playerScore);
                // Code for exploding a percentage amount of coins
                int coinsToLose = (int)((1 / _playerImpactCoinsDivider) * playerScore);
                if (coinsToLose == 0 && playerScore != 0) coinsToLose = 1;

            _playerScript.UnclaimedLoot -= coinsToLose;
            ExplodeCoins(coinsToLose);
        }

    }

    private bool HasGameStarted()
    {
        if (GameplaySceneScript == null) return false;

        return GameplaySceneScript.TimeRemaining <= GameplaySceneScript.StartTime - 1;
    }

    private void ExplodeCoins(int coinsToLose)
    {
        Vector3 targetPosition = Vector3.zero;
        GameObject coin;
        Vector3 rayCastDir = Vector3.zero;
        int maxAmountOfAttempts = 10;
        for (int i = 0; i < coinsToLose - 1; i++)
        {
            int amountOfAttempts = 0;
            coin = Instantiate(_coinPrefab, transform.position, Quaternion.identity);
            int loops = 0;
            do
            {
                amountOfAttempts++;
                Vector2 pos = UnityEngine.Random.insideUnitCircle.normalized * UnityEngine.Random.Range(_minExplosionRadius,_maxExplosionRadius);
                targetPosition = transform.position + new Vector3(pos.x, 0, pos.y);
                rayCastDir = targetPosition.normalized;
                Debug.DrawLine(transform.position, targetPosition, Color.black, 20f);
                if (amountOfAttempts > maxAmountOfAttempts)
                {
                    Debug.LogWarning("Broke the loop");
                    break;
                }
            } while (Physics.Raycast(transform.position, rayCastDir, (targetPosition - transform.position).magnitude));

            Loot lootScript = coin.GetComponent<Loot>();
            lootScript.LaunchFishToPos(targetPosition); 
        }
    }

    public void Respawn()
    {
        _playerCollider.isTrigger = false;
        _playerRespawnCollider.enabled = true;
        PlayerRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        IsRespawning = true;
        _respawningTimer = _timeNeededToRespawn;
        _respawnTimerObj.SetActive(true);

        PlayerRigidbody.velocity = Vector3.zero;
        _playerGFX.localScale = Vector3.one / 1000;
        _distanceFromOutOfBounds = float.MaxValue;
        _playerScript.HasLostPoints = false;

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
            CancelInvoke(nameof(Respawn));
            Invoke(nameof(Respawn), 0.5f);
            PlayAudioClip(_audioClipFall);
            SetMotorSpeeds(0.4f, 0.5f, 0.5f);
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

