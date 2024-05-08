using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// !!! There should be an empty object in the gameplayscene with this script attached. !!!
// !!! There should be an empty object in the gameplayscene with this script attached. !!!
// !!! There should be an empty object in the gameplayscene with this script attached. !!!

public class GameplayScene : MonoBehaviour
{
    [SerializeField]
    private float _timeRemaining; // In Seconds!
    [SerializeField] private TextMeshProUGUI _timerText;

    [SerializeField] private List<TextMeshProUGUI> _unClaimedScoresTexts;
    [SerializeField] private List<TextMeshProUGUI> _scoresTexts;

    private WinnerDecider _winnerDecider = new WinnerDecider();
    private void Awake()
    {

        foreach (TextMeshProUGUI text in _unClaimedScoresTexts)
        {
            text.gameObject.SetActive(false);
        }

        foreach (TextMeshProUGUI text in _scoresTexts)
        {
            text.gameObject.SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GameSettings.GameIsInProgress = true;
        SetUIStartOfGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameSettings.GameIsInProgress)
        {
            return;
        }

        DisplayTime();
        DisplayScores();
        // Check if time has run out
        if (_timeRemaining < 0)
        {
            GameSettings.GameIsInProgress = false;
            _winnerDecider.DecideWinner();

            // Set DontDestroyOnLoad for TOP 3 players
            // so that we can acces it and its fields in the gameOverScene.

            //DontDestroyOnLoad(GameSettings.FirstPlayer); 
            //DontDestroyOnLoad(GameSettings.SecondPlace);
            //DontDestroyOnLoad(GameSettings.ThirdPlace);

            if (GameSettings.FirstPlace != null)
            {
                GameSettings.FirstPlace.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            }

            if (GameSettings.SecondPlace != null)
            {
                GameSettings.SecondPlace.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            }

            if (GameSettings.ThirdPlace != null)
            {
                GameSettings.ThirdPlace.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            }

            SceneManager.LoadScene("GameOverScene");
        }
        else
        {
            _timeRemaining -= Time.deltaTime;
        }
    }

    private void DisplayScores()
    {
        if (GameSettings.PlayersInGame.Count <= 0)
        {
            return;
        }

        if (_unClaimedScoresTexts.Count <= 0 || _scoresTexts.Count <= 0) return;
    }

    private void SetUIStartOfGame()
    {
        for (int i = 0; i < GameSettings.PlayersInGame.Count; i++)
        {
            _unClaimedScoresTexts[i].gameObject.SetActive(true);
            _scoresTexts[i].gameObject.SetActive(true);
        }

        for (int i = 0; i < GameSettings.PlayersInGame.Count; i++)
        {
            _unClaimedScoresTexts[i].text =
                $"Unclaimed: {GameSettings.PlayersInGame[i].gameObject.GetComponent<Player>().UnclaimedLoot}";
        }

        for (int i = 0; i < GameSettings.PlayersInGame.Count; i++)
        {
            _scoresTexts[i].text =
                $"Score: {GameSettings.PlayersInGame[i].gameObject.GetComponent<Player>().Score}";
        }
    }

    private void DisplayTime()
    {
        float minutes = Mathf.FloorToInt(_timeRemaining / 60);
        float seconds = Mathf.FloorToInt(_timeRemaining % 60);

        if (_timerText != null)
        {
            if (seconds < 10)
            {
                _timerText.text = $"{minutes}:0{seconds}";
            }
            else
            {
                _timerText.text = $"{minutes}:{seconds}";
            }
        }
    }
}
