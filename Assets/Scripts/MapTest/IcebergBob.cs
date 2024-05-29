using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcebergBob : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float offset;

    void Update()
    {
            transform.position += transform.up * Mathf.Sin(Time.time + offset)/(speed);
    }
}
