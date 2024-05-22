using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootPickUp : MonoBehaviour
{
    private float _delay = 1f;
    private float _timer;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (_timer > _delay)
        {
        }
        else
        {
            _timer += Time.deltaTime;
        }
    }

    public void DestroyObject()
    {
        Debug.Log("Destroy Icon");
        Destroy(this.gameObject);
    }
}
