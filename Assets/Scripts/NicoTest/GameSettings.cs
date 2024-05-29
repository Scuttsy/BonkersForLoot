using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameSettings : MonoBehaviour
{
    public static bool GameIsInProgress = false;

    public static int MinimumPlayersRequiredToStart = 1;
    public static int PlayersRequiredToStart = 4;

    public static List<PlayerInput> PlayersInGame = new List<PlayerInput>();
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

    public static List<int> UnclaimedLoot
    {
        get
        {
            List<int> scores = new List<int>();

            for (int i = 0; i < PlayersInGame.Count; i++)
            {
                scores.Add((int)PlayersInGame[i].GetComponent<Player>().UnclaimedLoot);
            }
            return scores;
        }
    }
    public static List<int> PlayeScores
    {
        get
        {
            List<int> scores = new List<int>();

            for (int i = 0; i < PlayersInGame.Count; i++)
            {
                scores.Add(PlayersInGame[i].GetComponent<Player>().Score);
            }

            int max = 0;
            List<int> sortedScores = new List<int>();
            for (int i = 0; i < scores.Count; i++)
            {
                if (scores[i] > max)
                {
                    int x = scores[i];

                }
            }
            return scores;
        }
    }

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

    public static void CanEndGame()
    {
        foreach (var player in PlayersInGame)
        {
            player.GetComponent<PlayerMovementController>().CanEndGame();
        }
    }

    public static void ToggleDontDestroyOnLoadForPlayers()
    {
        foreach (PlayerInput player in PlayersInGame)
        {
            DontDestroyOnLoad(player);
        }
    }

    public static void DestroyAllPlayers()
    {
        foreach (PlayerInput player in PlayersInGame)
        {
            Destroy(player.gameObject);
        }
    }
}
