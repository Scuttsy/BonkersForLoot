// Ignore Spelling: Rigidbody

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RotatingTile : MonoBehaviour
{
    public Transform PlayerYeeter;
    [SerializeField] private bool _isRotatingBayblade;
    [SerializeField] private float _baybladeRotationSpeed;

    private bool _isRotatingLeft;
    // 22.26456 max velocity
    private const float MaxVelocity = 22.26456f;
    [SerializeField]
    private float _minVelocity = 17f;
    [SerializeField]
    private float _minAngle = 10f;
    [SerializeField]
    private float _maxAngle = 45f;

    public void LaunchPlayer(Rigidbody playerRigidbody)
    {
        float speedRatio = Mathf.Clamp01((playerRigidbody.velocity.magnitude - _minVelocity) / (MaxVelocity - _minVelocity));
        float angle = Mathf.Lerp(_minAngle, _maxAngle, 1-speedRatio);
        PlayerYeeter.Rotate(Vector3.up, (_isRotatingLeft ? 1 : -1) * angle);

        Vector3 newPlayerVelocity = PlayerYeeter.forward * playerRigidbody.velocity.magnitude;
        playerRigidbody.velocity = newPlayerVelocity;
    }

    private void Update()
    {
        if (!_isRotatingBayblade) return;
        transform.Rotate(new Vector3(0, _baybladeRotationSpeed * Time.deltaTime, 0), Space.World);
    }
}
