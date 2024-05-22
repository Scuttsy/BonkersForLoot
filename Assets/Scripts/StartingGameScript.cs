using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StartingGameScript : MonoBehaviour
{
    public PlayerInput PlayerInput;

    //public int requiredPlayerCount = 5;
    private int currentPlayerCount = 0;
    private float countdownTimer = 5f;
    private bool countingDown = false;
    private float elapsedTime = 0f;
    private float maxTime;

    public delegate void CountdownFinished();
    public static event CountdownFinished OnCountdownFinished;

    [SerializeField] private TextMeshProUGUI _startTimerText;
    [SerializeField] private Image _startTimerImage;
    [SerializeField] private Image _timerImage;
    [SerializeField] private TextMeshProUGUI _playerCounterText;
    [SerializeField] private TextMeshProUGUI _instructionPlayer1Text;
    [SerializeField] private TextMeshProUGUI _instructionPlayer2Text;
    [SerializeField] private TextMeshProUGUI _instructionPlayer3Text;
    [SerializeField] private TextMeshProUGUI _instructionPlayer4Text;

    private void Awake()
    {
        _timerImage.color = new Color(255f, 255f, 255f, 0f);
        maxTime = countdownTimer;
        _startTimerText.gameObject.SetActive(false);
        _playerCounterText.gameObject.SetActive(false);
        _instructionPlayer1Text.text = "Press any button to join!";
        _instructionPlayer2Text.text = "Press any button to join!";
        _instructionPlayer3Text.text = "Press any button to join!";
        _instructionPlayer4Text.text = "Press any button to join!";
    }

    private void Update()
    {
        if (GameSettings.PlayersInGame.Count > 0)
        {
            _instructionPlayer1Text.gameObject.SetActive(false);
            _playerCounterText.gameObject.SetActive(true);
            _playerCounterText.text = $"{currentPlayerCount} / {GameSettings.PlayersInGame.Count} players to start!";
        }

        if (countingDown)
        {
            elapsedTime += Time.deltaTime;
            _startTimerImage.fillAmount = elapsedTime/maxTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            currentPlayerCount++;
            //currentPlayerCount >= GameSettings.PlayersRequiredToStart || currentPlayerCount == GameSettings.MinimumPlayersRequiredToStart
            if (currentPlayerCount == GameSettings.PlayersInGame.Count && !countingDown)
            {
                _timerImage.color = new Color(255f, 255f, 255f, 255f);
                _startTimerText.gameObject.SetActive(true);
                countingDown = true;
                InvokeRepeating("CountDown", 1f, 1f);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            currentPlayerCount--;

            _startTimerText.gameObject.SetActive(false);

            if (currentPlayerCount < GameSettings.PlayersRequiredToStart && countingDown)
            {
                countdownTimer = 5f;
                countingDown = false;
                CancelInvoke("CountDown");
            }
        }
    }

    private void CountDown()
    {
        countdownTimer -= 1f;
        Debug.Log("Countdown: " + countdownTimer.ToString("F0"));

        //_startTimerText.gameObject.SetActive(true);
        _startTimerText.text = countdownTimer.ToString();

        if (countdownTimer <= 0f)
        {
            Debug.Log("Countdown finished!");
            // Reset countdown and player count
            countdownTimer = 5f;
            currentPlayerCount = 0;
            countingDown = false;
            CancelInvoke("CountDown");

            foreach (PlayerInput player in GameSettings.PlayersInGame)
            {
                DontDestroyOnLoad(player);
            }

            OnCountdownFinished?.Invoke();
        }
    }
}