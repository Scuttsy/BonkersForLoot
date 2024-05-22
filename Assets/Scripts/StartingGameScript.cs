using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class StartingGameScript : MonoBehaviour
{
    public PlayerInput PlayerInput;

    //public int requiredPlayerCount = 5;
    private int currentPlayerCount = 0;
    private float countdownTimer = 5f;
    private bool countingDown = false;

    public delegate void CountdownFinished();
    public static event CountdownFinished OnCountdownFinished;

    [SerializeField] private TextMeshProUGUI _startTimerText;
    [SerializeField] private TextMeshProUGUI _playerCounterText;
    [SerializeField] private TextMeshProUGUI _instructionPlayer1Text;
    [SerializeField] private TextMeshProUGUI _instructionPlayer2Text;
    [SerializeField] private TextMeshProUGUI _instructionPlayer3Text;
    [SerializeField] private TextMeshProUGUI _instructionPlayer4Text;

    private void Awake()
    {
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
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            currentPlayerCount++;
            //currentPlayerCount >= GameSettings.PlayersRequiredToStart || currentPlayerCount == GameSettings.MinimumPlayersRequiredToStart
            if (currentPlayerCount == GameSettings.PlayersInGame.Count && !countingDown)
            {
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

        _startTimerText.gameObject.SetActive(true);
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