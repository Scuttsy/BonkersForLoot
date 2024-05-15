using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GivePlayerPoints : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player>().Score += 100;
        }
    }
}
