using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // On trigger Check if collider is Player
        // and if so add 1 UnclaimedLoot of that player and destroy loot.

        if (other.gameObject.TryGetComponent(out Player player))
        {
            player.UnclaimedLoot++;
            Debug.Log("Is " + player.UnclaimedLoot); 
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
