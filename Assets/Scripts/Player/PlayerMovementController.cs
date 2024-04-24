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

    private float _horizontalInput;
    private float _verticalInput;

    void Update()
    {
        //Vector3 centeredMousePos = Input.mousePosition - new Vector3(Screen.width/2f,Screen.height/2f,0);
        //Vector3 pointerRotation = new Vector3(0,Mathf.Atan2(centeredMousePos.x, centeredMousePos.y) * Mathf.Rad2Deg, 0);
        //_pointerPivot.eulerAngles = pointerRotation;

        Vector3 pointerRotation = new Vector3();

        if (Input.GetMouseButtonDown(0))
        {
            if (_playerRigidbody.velocity.magnitude < _minVelocityToMove)
            {
                _playerGFX.forward = _pointerPivot.forward;
                Debug.Log(_playerGFX.forward.normalized * _forceStrength);
                _playerRigidbody.AddForce(_playerGFX.forward.normalized * _forceStrength, ForceMode.Impulse);
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
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "BouncyWall")
        {
            Vector3 tempVelocity = _playerRigidbody.velocity;
            _playerRigidbody.velocity = Vector3.zero;
        //    Vector3 castStartPos = _playerGFX.forward
        //    Physics.Linecast()
        }
    }
}
