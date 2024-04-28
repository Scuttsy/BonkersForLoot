using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static bool GameIsInProgress = false;

    public static List<Player> PlayersInGame;
    public static Player WinningPlayer;

    public static SpawnPoint[] SpawnPointList;
    public SpawnPoint SpawnPointA;
    public SpawnPoint SpawnPointB;
    public SpawnPoint SpawnPointC;
    public SpawnPoint SpawnPointD;

    void Awake()
    {
        SpawnPointList = new SpawnPoint[] { SpawnPointA, SpawnPointB, SpawnPointC, SpawnPointD };
    }
}
