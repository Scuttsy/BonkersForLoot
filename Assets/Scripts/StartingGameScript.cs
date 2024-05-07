using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingGameScript : MonoBehaviour
{
    public int requiredPlayerCount = 5;
    private int currentPlayerCount = 0;
    private float countdownTimer = 5f;
    private bool countingDown = false;

    public delegate void CountdownFinished();
    public static event CountdownFinished OnCountdownFinished;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            currentPlayerCount++;

            if (currentPlayerCount >= requiredPlayerCount && !countingDown)
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

            if (currentPlayerCount < requiredPlayerCount && countingDown)
            {
                countingDown = false;
                CancelInvoke("CountDown");
            }
        }
    }

    private void CountDown()
    {
        countdownTimer -= 1f;
        Debug.Log("Countdown: " + countdownTimer.ToString("F0"));

        if (countdownTimer <= 0f)
        {
            Debug.Log("Countdown finished!");
            // Reset countdown and player count
            countdownTimer = 5f;
            currentPlayerCount = 0;
            countingDown = false;
            CancelInvoke("CountDown");

            OnCountdownFinished?.Invoke();
        }
    }
}