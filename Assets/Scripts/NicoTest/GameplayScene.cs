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
    public float TimeRemaining; // In Seconds!
    [SerializeField] private Image _timerImage;
    [SerializeField] private TextMeshProUGUI _timerText;
    [HideInInspector] public float StartTime;

    [SerializeField] private List<TextMeshProUGUI> _unClaimedScoresTexts;
    [SerializeField] private List<TextMeshProUGUI> _scoresTexts;

    [SerializeField] private Color _startTimerColor;
    [SerializeField] private Color _endTimerColor;
    [SerializeField] private float _timeStartColorlerp;
    [SerializeField] private float _maxColorTime; //remaining time during which color is maximally red

    [Header("References")]
    [SerializeField] private List<Image> _arrowToCentreImg;

    [Header("Player UI Settings")]
    [SerializeField] private int _displayDunkArrowThreshold = 1;

    private WinnerDecider _winnerDecider = new WinnerDecider();
    private void Awake()
    {

        foreach (TextMeshProUGUI text in _unClaimedScoresTexts)
        {
            if (text != null)
            text.gameObject.SetActive(false);
        }

        foreach (TextMeshProUGUI text in _scoresTexts)
        {
            if (text != null)
            text.gameObject.SetActive(false);
        }

        StartTime = TimeRemaining;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameSettings.GameIsInProgress = true;
        PlayerMovementController.SetGamePlayScene(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameSettings.GameIsInProgress)
        {
            return;
        }

        DisplayTime();
        //DisplayScores();
        SetUIStartOfGame();
        // Check if time has run out
        if (TimeRemaining < 0)
        {
            GameSettings.GameIsInProgress = false;

            //Winner is decided
            _winnerDecider.DecideWinner();

            // Set DontDestroyOnLoad for TOP 3 players
            // so that we can acces it and its fields in the gameOverScene.

            //DontDestroyOnLoad(GameSettings.FirstPlace);
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
            TimeRemaining -= Time.deltaTime;
        }
    }
    private void FixedUpdate()
    {
        //for (int i = 0; i < GameSettings.PlayersInGame.Count; i++)
        //{
        //    if (GameSettings.UnclaimedLoot[i] >= 0)
        //    {
        //        _arrowToCentreImg[i].enabled = true;
        //        _arrowToCentreImg[i].transform.position = GameSettings.PlayersInGame[i].transform.position;
        //        Vector3 lookDirection = -_arrowToCentreImg[i].transform.position;
        //        _arrowToCentreImg[i].transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        //    }
        //    else
        //        _arrowToCentreImg[i].enabled = false;
        //}
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
        //Debug.Log($"Players in Game: {GameSettings.PlayersInGame.Count}");
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
        float minutes = Mathf.FloorToInt(TimeRemaining / 60);
        float seconds = Mathf.FloorToInt(TimeRemaining % 60);

        _timerImage.fillAmount = TimeRemaining / StartTime;
        _timerImage.enabled = false;
        if (_timerImage != null)
        {
            if (seconds < 10)
            {
                _timerText.text = $"{minutes}:0{seconds}";
            }
            else
            {
                _timerText.text = $"{minutes}:{seconds}";
            }

            if (TimeRemaining < _timeStartColorlerp && TimeRemaining > _maxColorTime)
            {
                _timerImage.color = Color.Lerp(_endTimerColor, _startTimerColor, TimeRemaining/ (_timeStartColorlerp - _maxColorTime));
                _timerText.color = Color.Lerp(_endTimerColor, _startTimerColor, TimeRemaining/ (_timeStartColorlerp - _maxColorTime));
            }

            if (TimeRemaining < 1)
            {
                _timerText.text = "0:00";
            }
        }
        _timerImage.enabled = true;
    }
}
