using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static bool GameIsInProgress = false;

    public static List<Player> PlayersInGame;
    public static Player WinningPlayer;
}
