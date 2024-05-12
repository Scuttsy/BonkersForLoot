using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameSettings : MonoBehaviour
{
    public static bool GameIsInProgress = false;

    public static int PlayersRequiredToStart;

    public static List<PlayerInput> PlayersInGame;
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
        PlayersRequiredToStart = PlayersInGame.Count;

        SpawnPointList = new List<Transform>();
        SpawnPointList.Add(SpawnPointA);
        SpawnPointList.Add(SpawnPointB);
        SpawnPointList.Add(SpawnPointC);
        SpawnPointList.Add(SpawnPointD);
    }
}
