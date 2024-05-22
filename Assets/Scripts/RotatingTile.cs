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
    [SerializeField]
    private float _minVelocity = 17f;
    [SerializeField]
    private float _angle = 30f;

    public void LaunchPlayer(Rigidbody playerRigidbody)
    {
        PlayerYeeter.Rotate(Vector3.up, (_isRotatingLeft ? 1 : -1) * _angle);

        Vector3 newPlayerVelocity = PlayerYeeter.forward * playerRigidbody.velocity.magnitude;
        playerRigidbody.velocity = newPlayerVelocity;
    }

    private void Update()
    {
        if (!_isRotatingBayblade) return;
        transform.Rotate(new Vector3(0, _baybladeRotationSpeed * Time.deltaTime, 0), Space.World);
    }
}
