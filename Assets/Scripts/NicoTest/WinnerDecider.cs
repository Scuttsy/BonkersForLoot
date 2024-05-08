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

        GameObject _currentFirstPlace = null;
        GameObject _currentSecondPlace = null;
        GameObject _currentThirdPlace = null;
        foreach (PlayerInput player in GameSettings.PlayersInGame)
        {   
            // If there is no First place player.
            if (_currentFirstPlace == null)
            {
                _currentFirstPlace = player.gameObject;
            }

            // If the next player has a higher score than the current first place.
            if (player.GetComponent<Player>().Score > _currentFirstPlace.GetComponent<Player>().Score)
            {
                _currentSecondPlace = _currentFirstPlace;
                _currentFirstPlace = player.gameObject;
            }

            // If the next player has a lower score than the current first place.
            else if (player.GetComponent<Player>().Score < _currentFirstPlace.GetComponent<Player>().Score)
            {
                // If there is no second place player
                if (_currentSecondPlace == null)
                {
                    _currentSecondPlace = player.gameObject;
                }

                // If the next player score is higher than the current second place.
                else if (player.GetComponent<Player>().Score > _currentSecondPlace.GetComponent<Player>().Score)
                {
                    _currentThirdPlace = _currentSecondPlace;
                    _currentSecondPlace = player.gameObject;
                }

                // If the next player score is smaller than the second place.
                else
                {
                    // If there is no third place player
                    if (_currentThirdPlace == null)
                    {
                        _currentThirdPlace = player.gameObject;
                    }

                    // If the next player score is bigger than the third place
                    if (player.GetComponent<Player>().Score > _currentThirdPlace.GetComponent<Player>().Score)
                    {
                        _currentThirdPlace = player.gameObject;
                    }
                }
            }
        }
        
        GameSettings.FirstPlace = _currentFirstPlace;
        GameSettings.SecondPlace = _currentSecondPlace;
        GameSettings.ThirdPlace = _currentThirdPlace;
    }
}
