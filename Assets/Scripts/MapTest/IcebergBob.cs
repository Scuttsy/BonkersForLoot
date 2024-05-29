using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcebergBob : MonoBehaviour
{
    [SerializeField] private float Amplitude;
    [SerializeField] private float frequency;
    [SerializeField] private float shift;

    void Update()
    {
            transform.position += (transform.up * Amplitude * Mathf.Sin(frequency* (Time.time + shift))) * Time.deltaTime;
    }
}
