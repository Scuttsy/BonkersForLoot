using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameSettings : MonoBehaviour
{
    public static bool GameIsInProgress = false;

<<<<<<< Updated upstream
    public static int MinimumPlayersRequiredToStart = 1;
    public static int PlayersRequiredToStart = 4;

    public static List<PlayerInput> PlayersInGame;
=======
    public static List<PlayerInput> PlayersInGame = new List<PlayerInput>();
>>>>>>> Stashed changes
    public static GameObject FirstPlace;
    public static GameObject SecondPlace;
    public static GameObject ThirdPlace;

    public static List<LootSpawnPoint> LootSpawnPoints;
    public static int LootOnMap;

    public static List<Transform> SpawnPointList;
    public Transform SpawnPointA;
    public Transform SpawnPointB;
    public Transform SpawnPointC;
    public Transform SpawnPointD;

    void Awake()
    {
        SpawnPointList = new List<Transform>
        {
            SpawnPointA,
            SpawnPointB,
            SpawnPointC,
            SpawnPointD
        };
    }
}
