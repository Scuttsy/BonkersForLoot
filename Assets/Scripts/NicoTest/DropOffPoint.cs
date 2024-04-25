using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropOffPoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // On trigger Check if collider is Player
        // and if so add UnclaimedLoot to score and UnclaimedLoot = 0.

        if (other.gameObject.TryGetComponent(out Player player))
        {
            player.Score += player.UnclaimedLoot;
            player.UnclaimedLoot = 0;
        }
    }
}