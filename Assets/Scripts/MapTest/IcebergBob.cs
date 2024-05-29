using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcebergBob : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float offset;

    private float _startTime;
    private void Start()
    {
        _startTime = Time.timeSinceLevelLoad + offset;
    }
    void Update()
    {
        if (Time.timeSinceLevelLoad > offset)
        {
            //Vector3 nextPos = Vector3.Lerp(transform.position);
        }
    }
}
