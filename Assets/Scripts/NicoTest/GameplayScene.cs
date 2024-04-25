using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// !!! There should be an empty object in the gameplayscene with this script attached. !!!
// !!! There should be an empty object in the gameplayscene with this script attached. !!!
// !!! There should be an empty object in the gameplayscene with this script attached. !!!

public class GameplayScene : MonoBehaviour
{
    private float _timeRemaining = 30f; // In Seconds!
    [SerializeField] private TextMeshProUGUI _timerText;
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

        // Check if time has run out
        if (_timeRemaining < 0)
        {
            //To-do Switch to game over scene
            GameSettings.GameIsInProgress = false;
            Debug.Log("Game over");
        }
        else
        {
            _timeRemaining -= Time.deltaTime;
        }

        DisplayTime();
    }

    private void DisplayTime()
    {
        float minutes = Mathf.FloorToInt(_timeRemaining / 60);
        float seconds = Mathf.FloorToInt(_timeRemaining % 60);

        _timerText.text = $"{minutes}:{seconds}";
    }
}
