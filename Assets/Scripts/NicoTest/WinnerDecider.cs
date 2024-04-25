using System.Collections;
using System.Collections.Generic;


public class WinnerDecider 
{
    public void DecideWinner()
    {
        // Loop through all players in the game
        // Assign the one with the highest score as the winner.

        Player _currentWinner = null;
        foreach (Player player in GameSettings.PlayersInGame)
        {
            
            if (_currentWinner == null)
            {
                _currentWinner = player;
            }
            else if (player.Score > _currentWinner.Score)
            {
                _currentWinner = player;
            }
        }
        
        GameSettings.WinningPlayer = _currentWinner;
    }
}
