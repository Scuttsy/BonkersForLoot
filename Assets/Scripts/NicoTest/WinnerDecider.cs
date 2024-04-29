using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class WinnerDecider 
{
    public void DecideWinner()
    {
        // Loop through all players in the game
        // Assign the one with the highest score as the winner.

        GameObject _currentWinner = null;
        foreach (PlayerInput player in GameSettings.PlayersInGame)
        {
            
            if (_currentWinner == null)
            {
                _currentWinner = player.gameObject;
            }
            else if (player.GetComponent<Player>().Score > _currentWinner.GetComponent<Player>().Score)
            {
                _currentWinner = player.gameObject;
            }
        }
        
        GameSettings.WinningPlayer = _currentWinner;
    }
}
