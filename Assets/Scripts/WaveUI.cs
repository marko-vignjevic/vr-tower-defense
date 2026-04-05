using UnityEngine;
using TMPro;
using System.Collections;

public class WaveUI : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI timerText;
    public WaveSpawner waveSpawner;

    private int timeRemaining;
    private bool isCountingDown = false;
    private bool wasSpawning = false;

    void Update()
    {
        UpdateWaveText();
        UpdateTimerText();
    }

    void UpdateWaveText()
    {
        // Update the wave text to show the current wave number
        waveText.text = "Wave " + waveSpawner.GetCurrentWave();
    }

    void UpdateTimerText()
    {
        // Check if the wave spawner is currently spawning a wave
        bool currentlySpawning = waveSpawner.IsSpawning();

        // If we just finished spawning a wave, start the countdown for the next wave
        if (wasSpawning && !currentlySpawning)
        {
            // Start the countdown for the next wave if not already counting down
            if (!isCountingDown)
            {
                StartCoroutine(TimerCountDown(waveSpawner.GetTimeBetweenWaves()));
            }
        }

        wasSpawning = currentlySpawning;

        // Update the timer text to show the countdown
        // if we are counting down for the next wave
        if (isCountingDown)
        {
            timerText.text = "Next wave in \n" + timeRemaining + " \nseconds";
        }
        else
        {
            timerText.text = "";
        }
    }

    IEnumerator TimerCountDown(float seconds)
    {
        // Start a countdown timer for the next wave
        isCountingDown = true;
        timeRemaining = Mathf.CeilToInt(seconds);

        while (timeRemaining > 0)
        {
            yield return new WaitForSeconds(1f);
            timeRemaining--;
        }

        isCountingDown = false;
        timerText.text = "";
    }
}
