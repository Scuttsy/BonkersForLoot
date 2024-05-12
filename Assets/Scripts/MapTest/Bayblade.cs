using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bayblade : MonoBehaviour
{
    [SerializeField] private float YeetMultiplier = 1f;
    private void OnCollisionEnter(Collision collision)
    {
        //collision.transform.rotation = gameObject.transform.rotation;
        collision.transform.gameObject.GetComponent<PlayerMovementController>().Fire(transform.forward * YeetMultiplier);
    }
}
