using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    private GameObject _parent;
    private void Awake()
    {
        GameSettings.LootOnMap++; // Add 1 to loot on map when created
        _parent = this.transform.parent.gameObject;
        _parent.GetComponent<LootSpawnPoint>().HasSpawnedLoot = true;
    }

    private void OnDestroy()
    {
        GameSettings.LootOnMap--; // Substract 1 from loot on map when Destroyed
        _parent.GetComponent<LootSpawnPoint>().HasSpawnedLoot = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // On trigger Check if collider is Player
        // and if so add 1 UnclaimedLoot of that player and destroy loot.

        GameObject otherParent = other.gameObject.transform.parent.gameObject;

        if (other.gameObject.TryGetComponent(out Player player))
        {
            player.UnclaimedLoot++;
            Debug.Log("Is " + player.UnclaimedLoot); 
            Destroy(this.gameObject);
        }

        if (otherParent.TryGetComponent(out Player parentPlayer))
        {
            parentPlayer.UnclaimedLoot++;
            Debug.Log("Is " + parentPlayer.UnclaimedLoot);
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
