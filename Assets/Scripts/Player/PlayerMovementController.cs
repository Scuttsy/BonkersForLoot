using System.Collections;
using System.Collections.Generic;
//using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Collider _playerCollider;
    [SerializeField] private Transform _pointerPivot;
    [SerializeField] private Transform _playerGFX;
    [SerializeField] private Rigidbody _playerRigidbody;

    [Header("Settings")]
    [SerializeField] private float _forceStrength;
    [SerializeField] private float _minVelocityToMove;
    [Range(0,1)]
    [SerializeField] private float _controllerDeadZone;
    [SerializeField] private float _fireCooldown;

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

        if ((tempPosition - _previousPosition).magnitude > 0)
            _playerGFX.forward = (tempPosition - _previousPosition).normalized;
        else
            //TODO: if standing still player suddenly faces world forward.

        // Checking if the input is non-zero, if it isn't we keep the old input direction.
        if (Mathf.Abs(_horizontalInput) > _controllerDeadZone || Mathf.Abs(_verticalInput) > _controllerDeadZone)
        {
            Vector3 pointerRotation = new Vector3(0,Mathf.Atan2(_horizontalInput, _verticalInput) * Mathf.Rad2Deg, 0);
            _pointerPivot.eulerAngles = pointerRotation;
        }
        Debug.Log("" + _horizontalInput + ", " + _verticalInput);

        if (Input.GetAxis("Fire2") > 0.5f && _readyToFire)
        {
            if (_playerRigidbody.velocity.magnitude < _minVelocityToMove)
            {
                _readyToFire = false;
                _playerGFX.forward = _pointerPivot.forward;
                _playerRigidbody.AddForce(_playerGFX.forward.normalized * _forceStrength, ForceMode.Impulse);
                Invoke(nameof(SetReadyToFire), 0.25f);
            }
            else
            {
                Debug.Log("Too much speed");
            }
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

    void SetReadyToFire()
    {
        _readyToFire = true;
    }
}
