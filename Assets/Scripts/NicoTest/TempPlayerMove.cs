using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempPlayerMove : MonoBehaviour
{
    private float _speed = 500f;
    private Vector3 _movementVector;
    private Rigidbody _rb;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        ApplyMovement();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    private void ApplyMovement()
    {
        _movementVector.x = Input.GetAxis("Horizontal") * _speed * Time.deltaTime;
        _movementVector.z = Input.GetAxis("Vertical") * _speed * Time.deltaTime;

        _rb.velocity = _movementVector;
    }
}
