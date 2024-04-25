using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int UnclaimedLoot = 0;
    public int Score = 0;

    // Start is called before the first frame update
    void Start()
    {
        // If this player hasn't been added yet to the PlayersInGame list add it.
        if (!GameSettings.PlayersInGame.Contains(this))
        {
            GameSettings.PlayersInGame.Add(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
