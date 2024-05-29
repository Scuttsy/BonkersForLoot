using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class Loot : MonoBehaviour
{
    private GameObject _parent = null;
    [SerializeField] private float _floatSpeed = 1.0f; //the time in seconds that the coin should take to bounce up and down
    [SerializeField] private float _noCollisionTime = 1.0f;
    [SerializeField] private float _explodeSpeed = 1.0f;
    [SerializeField] private float _minLaunchHeight = 0.5f;
    [SerializeField] private float _maxLaunchHeight = 1.5f;
    [HideInInspector] public bool IsActive = true;
    [HideInInspector] public Vector3 ExplodeTarget = Vector3.zero; //The place that the coin tries to move to

    [SerializeField]
    private AudioClip _audioClip;
    private AudioSource _source;

    [SerializeField] private GameObject _lootPickupBlueIcon;
    [SerializeField] private GameObject _lootPickupGreenIcon;
    [SerializeField] private GameObject _lootPickupRedIcon;
    [SerializeField] private GameObject _lootPickupYellowIcon;

    private bool _isForceSpawned;
    private Rigidbody _fishRigidbody;
    private BoxCollider _fishBoxCollider;

    void Start()
    {
        _source = FindAnyObjectByType<AudioSource>();
    }

    private void Awake()
    {
        GameSettings.LootOnMap++; // Add 1 to loot on map when created
        _fishRigidbody = GetComponent<Rigidbody>();
        _fishBoxCollider = GetComponent<BoxCollider>();
        if (transform.parent != null)
        {
            _parent = this.transform.parent.gameObject;
        }

        _isForceSpawned = _parent == null;
        if (!_isForceSpawned)
        {
            _parent.GetComponent<LootSpawnPoint>().HasSpawnedLoot = true;
        }
        else
        {
            IsActive = false;
            _fishBoxCollider.enabled = false;
            Invoke(nameof(DelayedEnable), _noCollisionTime);
            Invoke(nameof(DelayedColliderEnable), _noCollisionTime/4);
        }
        
    }

    private void OnDestroy()
    {
        GameSettings.LootOnMap--; // Substract 1 from loot on map when Destroyed
        if (!_isForceSpawned)
            _parent.GetComponent<LootSpawnPoint>().HasSpawnedLoot = false;                                                                                                                                                                                                                                                       
    }

    private void OnTriggerEnter(Collider other)
    {
        // On trigger Check if collider is Player
        if (other.tag == "Player")
        // and if so add 1 UnclaimedLoot of that player and destroy loot.
        {
            if (!IsActive) return;
            GameObject otherParent = other.gameObject.transform.parent.gameObject;

            if (other.gameObject.TryGetComponent(out Player player))
            {
                player.UnclaimedLoot++;
                //AudioSource.PlayClipAtPoint(_audioClip, transform.position);
                if (_source != null)
                    _source.PlayOneShot(_audioClip);

                Debug.Log("Is " + player.UnclaimedLoot);
            }

            if (otherParent.TryGetComponent(out Player parentPlayer))
            {
                parentPlayer.UnclaimedLoot++;
                if (_source == null)
                    TryGetComponent(out _source);
                if (_source != null)
                    _source.PlayOneShot(_audioClip);
                else Debug.LogError("WDYM ITS STILL NULL");

                //Debug.Log("Is " + parentPlayer.UnclaimedLoot);

                if (parentPlayer.PlayerName == "Blue")
                {
                    Instantiate(_lootPickupBlueIcon, this.transform.position, Quaternion.identity);
                }

                else if (parentPlayer.PlayerName == "Green")
                {
                    Instantiate(_lootPickupGreenIcon, this.transform.position, Quaternion.identity);
                }

                else if (parentPlayer.PlayerName == "Red")
                {
                    Instantiate(_lootPickupRedIcon, this.transform.position, Quaternion.identity);
                }
                else if (parentPlayer.PlayerName == "Yellow")
                {
                    Instantiate(_lootPickupYellowIcon, this.transform.position, Quaternion.identity);
                }
                else
                {
                    Debug.Log("Player not found (Loot)");
                }

            }
            Destroy(this.gameObject);
        }

        if (other.tag == "OutOfBounds")
        {
            Destroy(gameObject);
        }
        if (other.tag == "LobbyOutOfBounds")
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (ExplodeTarget != Vector3.zero)
        //{
        //    Vector3 velocity = (ExplodeTarget - transform.position).normalized;
        //    float Amplitude = Mathf.Min(_explodeSpeed, (ExplodeTarget - transform.position).magnitude);
        //    transform.position += velocity * Amplitude * Time.deltaTime;

        //}
    }

    private void DelayedEnable()
    {
        IsActive = true;
    }
    private void DelayedColliderEnable()
    {
        _fishBoxCollider.enabled = true;
    }

    public void LaunchFishToPos(Vector3 endPosition)
    {
        _fishRigidbody.velocity = GetLaunchVelocity(endPosition);
    }

    private Vector3 GetLaunchVelocity(Vector3 endPosition)
    {
        float displacementY = endPosition.y - _fishRigidbody.position.y;
        Vector3 displacementXZ = new Vector3(endPosition.x - _fishRigidbody.position.x, 0,
            endPosition.z - _fishRigidbody.position.z);
        float g = Physics.gravity.magnitude;
        //TODO: randomize height
        float h = _maxLaunchHeight;
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(2*g*h);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt((2 * h) / g) + Mathf.Sqrt((-2 * (displacementY - h)) / g));

        return velocityY + velocityXZ;
    }
}
