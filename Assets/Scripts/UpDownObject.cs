using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class UpDownObject : MonoBehaviour
{
    [Tooltip("The amount of time it takes for the object to move up and down, in seconds")]
    [SerializeField] private float _upAndDownSpeed = 1f;
    [SerializeField] private float _minY = -1.8f;
    [SerializeField] private float _maxY = -.4f;
    [SerializeField] private float _upTime = 5f;
    [SerializeField] private float _downTime = 5f;

    private float _initialDelay;
    private bool _isMoving;
    private bool _isDown;
    private float _timer = float.MaxValue;
    private float _movingStartTimeStamp;
    private float _movingEndTimeStamp;

    void Start()
    {
        _initialDelay = Random.Range(0.5f, _upTime);
        _minY *= 2; 
        _maxY *= 2;
    }

    void Update()
    {
        if (Time.time < _initialDelay) return;

        if (_isMoving)
        {
            float timeRatio = (Time.time - _movingStartTimeStamp) / (_movingEndTimeStamp - _movingStartTimeStamp);
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(_maxY, _minY, _isDown ? timeRatio : 1 - timeRatio), transform.position.z);
            _isMoving = timeRatio < 1;
            return;
        }

        _timer += Time.deltaTime;

        if (_timer > (_isDown ? _downTime : _upTime))
        {
            _isDown = !_isDown;
            _isMoving = true;
            _timer = 0f;
            _movingStartTimeStamp = Time.time;
            _movingEndTimeStamp = _movingStartTimeStamp + _upAndDownSpeed;
        }

    }
}
