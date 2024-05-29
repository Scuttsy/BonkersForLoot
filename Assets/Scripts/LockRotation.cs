using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRotation : MonoBehaviour
{
    private Quaternion _startQuaternion;
    void Start()
    {
        _startQuaternion = Quaternion.Euler(90,0,0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = _startQuaternion;
    }
}
