using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// !!! There should be an empty object in the GameOverScene with this script attached. !!!
// !!! There should be an empty object in the GameOverScene with this script attached. !!!
// !!! There should be an empty object in the GameOverScene with this script attached. !!!
public class GameOverScene : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _winnerText;

    [Header("References")]
    [SerializeField] private Transform _winnerPos;
    [SerializeField] private Transform _secondPos;
    [SerializeField] private Transform _thirdPos;

    [SerializeField] private List<TMP_Text> _leaderBoardNames = new List<TMP_Text>();
    [SerializeField] private List<TMP_Text> _leaderBoardScores = new List<TMP_Text>();

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.PlayOneShot(audioSource.clip);
        }

        GameSettings.GameIsInProgress = false;
        SetPlayerPositions();
        SetPlayerLeaderboard();
        // Set Winning player in UI
        if ( _winnerText != null && GameSettings.FirstPlace != null)
        {
            _winnerText.text = $"The winner is: {GameSettings.FirstPlace.gameObject.GetComponent<Player>().PlayerName}";
        }

        else
        {
            Debug.Log("Something is Null in winner text");
        }
    }

    private void SetPlayerLeaderboard()
    {
        for (int i = 0; i < GameSettings.PlayersInGame.Count; i++)
        {
            _leaderBoardNames[i].text = WinnerDecider.Leaderboard[i].PlayerName;
            _leaderBoardScores[i].text = WinnerDecider.Leaderboard[i].Score.ToString();
        }
    }

    private void SetPlayerPositions()
    {
        if (GameSettings.FirstPlace != null)
        {
            GameSettings.FirstPlace.transform.rotation = new Quaternion(0, 0, 0, 0);
            GameSettings.FirstPlace.transform.position = _winnerPos.position;
        }

        if (GameSettings.SecondPlace != null)
        {
            GameSettings.SecondPlace.transform.rotation = new Quaternion(0, 0, 0, 0);
            GameSettings.SecondPlace.transform.position = _secondPos.position;
        }

        if (GameSettings.ThirdPlace != null)
        {
            GameSettings.ThirdPlace.transform.rotation = new Quaternion(0, 0, 0, 0);
            GameSettings.ThirdPlace.transform.position = _thirdPos.position;
        }



    }

}
