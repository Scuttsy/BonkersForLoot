using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LobbyScene : MonoBehaviour
{
    private void Awake()
    {
        // Create Player list in GameSettings before other script's starts are called
        GameSettings.PlayersInGame = new List<PlayerInput>();
        GameSettings.LootSpawnPoints = new List<LootSpawnPoint>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
