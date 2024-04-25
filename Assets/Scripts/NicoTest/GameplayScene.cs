using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// !!! There should be an empty object in the gameplayscene with this script attached. !!!
// !!! There should be an empty object in the gameplayscene with this script attached. !!!
// !!! There should be an empty object in the gameplayscene with this script attached. !!!

public class GameplayScene : MonoBehaviour
{
    private float _timeRemaining = 10f; // In Seconds!
    [SerializeField] private TextMeshProUGUI _timerText;

    private WinnerDecider _winnerDecider = new WinnerDecider();
    private void Awake()
    {
        // Create Player list in GameSettings before other script's starts are called
        GameSettings.PlayersInGame = new List<Player>();
    }

    // Start is called before the first frame update
    void Start()
    {
        GameSettings.GameIsInProgress = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameSettings.GameIsInProgress)
        {
            return;
        }

        DisplayTime();

        // Check if time has run out
        if (_timeRemaining < 0)
        {
            GameSettings.GameIsInProgress = false;
            _winnerDecider.DecideWinner();

            // Set DontDestroyOnLoad for WinningPlayer 
            // so that we can acces it and its fields in the gameOverScene.
            DontDestroyOnLoad(GameSettings.WinningPlayer);

            SceneManager.LoadScene("GameOverScene");
        }
        else
        {
            _timeRemaining -= Time.deltaTime;
        }
    }

    private void DisplayTime()
    {
        float minutes = Mathf.FloorToInt(_timeRemaining / 60);
        float seconds = Mathf.FloorToInt(_timeRemaining % 60);

        _timerText.text = $"{minutes}:{seconds}";
    }
}
