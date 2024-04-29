using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingTile : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed;
    void Update()
    {
        transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime);
    }
}
