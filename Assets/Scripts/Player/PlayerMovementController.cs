using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovementController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Collider _playerCollider;
    [SerializeField] private Transform _pointerPivot;
    [SerializeField] private MeshRenderer _pointer;
    [SerializeField] private Transform _playerGFX;
    [SerializeField] private Rigidbody _playerRigidbody;
    [SerializeField] private Player _playerScript;

    [Header("Settings")]
    [SerializeField] private float _forceStrength;
    [SerializeField] private float _minVelocityToMove;
    [Range(0,1)]
    [SerializeField] private float _controllerDeadZone;
    [SerializeField] private float _fireCooldown;
    [SerializeField] private bool _usingMouse;

    private float _horizontalInput;
    private float _verticalInput;

    private bool _readyToFire = true;
    private Vector3 _previousPosition;

    void Update()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");

        Vector3 tempPosition = transform.position;
        tempPosition.y = 0;

        if ((tempPosition - _previousPosition).magnitude > 0.001f)
        {
            _playerGFX.forward = (tempPosition - _previousPosition).normalized;
        }
        else
        {   
            //TODO: if standing still player suddenly faces world forward.
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
            if (Input.GetAxis("Fire2") > 0.5f && _readyToFire)
            {
                _pointer.enabled = true;
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
        

        // Exiting play mode in editor without repressing the play button
        if (Input.GetAxis("Cancel") > 0.5f)
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
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
            //TODO: rotation sometimes still changes
            //TODO: sometimes character is too far into the wall, causing them to be stuck twice
            _playerRigidbody.velocity = Vector3.zero;
        }

        if (collision.gameObject.tag == "OutOfBounds")
        {
            transform.position = Vector3.up / 2;
            _playerRigidbody.velocity = Vector3.zero;
            //TODO: add code to spawn at the spawn points, taking into consideration the positions of the other player
            //TODO: currently checks furthest total distance, but it might be better to check largest individual distances
            //float furthestDistance = 0f;
            //Spawnpoint furthestSpawnpoint;
            //foreach (Spawnpoint spawnpoint in GameSettings.Spawnpoints)
            //{
            //    float totalDistanceFromPlayers = 0;
            //    foreach (Player player in GameSettings.PlayersInGame)
            //    {
            //        if (player == this._playerScript) continue;
            //        totalDistanceFromPlayers +=
            //            Vector3.Distance(spawnpoint.transform.position, player.transform.position);
            //    }

            //    if (totalDistanceFromPlayers > furthestDistance)
            //    {
            //        furthestDistance = totalDistanceFromPlayers;
            //        furthestSpawnpoint = spawnpoint;
            //    }
            //}
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "RotatingTile")
        {
            Vector3 playerVelocity = other.transform.forward * _playerRigidbody.velocity.magnitude;
            _playerRigidbody.velocity = playerVelocity;
        }
    }
}
