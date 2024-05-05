using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RotatingTile : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed;

    public Transform PlayerYeeter;

    private bool _isRotatingLeft;
    // 22.26456 max velocity
    private const float MaxVelocity = 22.26456f;
    [SerializeField]
    private float _minVelocity = 17f;
    [SerializeField]
    private float _minAngle = 10f;
    [SerializeField]
    private float _maxAngle = 45f;

    void Start()
    {
        float xSign = Mathf.Sign(transform.localScale.x);
        float ySign = Mathf.Sign(transform.localScale.y);
        float zSign = Mathf.Sign(transform.localScale.z);

        _rotationSpeed *= -1 * xSign * ySign * zSign;

        _isRotatingLeft = _rotationSpeed > 0;
    }

    void Update()
    {
        transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime);
    }

    public void LaunchPlayer(Rigidbody playerRigidbody)
    {
        float speedRatio = Mathf.Clamp01((playerRigidbody.velocity.magnitude - _minVelocity) / (MaxVelocity - _minVelocity));
        float angle = Mathf.Lerp(_minAngle, _maxAngle, 1-speedRatio);
        PlayerYeeter.Rotate(Vector3.up, (_isRotatingLeft ? 1 : -1) * angle);

        Vector3 newPlayerVelocity = PlayerYeeter.forward * playerRigidbody.velocity.magnitude;
        playerRigidbody.velocity = newPlayerVelocity;
    }
}
