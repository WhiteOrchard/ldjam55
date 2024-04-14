using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI lapCounter;
    public TextMeshProUGUI totalTimeCounter;
    public TextMeshProUGUI currentTimeCounter;
    public TextMeshProUGUI bestTimeCounter;
    public TextMeshProUGUI positionCounter;

    public GameObject countdownPanel;
    public TextMeshProUGUI countdownText;

    public VehicleController playerVehicleController;
    public List<VehicleController> enemyVehicleControllerList = new List<VehicleController>();
    public TrackInstrumentationController trackInstrumentationController;

    public Image fader;
    public float fadeDuration = 2f;

    float countdownDuration = 4f;
    public static GameManager instance;
    public int numberOfLaps = 3;

    public bool isRaceStarted = false;

    void Start()
    {
        if (instance == null)
        {             
            instance = this;
        }

        StartFadeIn();
    }

    void Update()
    {
        int currentLap = Mathf.Max(1, playerVehicleController.getStartedLapsCount());
        lapCounter.text = currentLap + "/" + numberOfLaps;

        float totalTime = playerVehicleController.getTotalTime();
        TimeSpan totalTimeSpan = TimeSpan.FromSeconds(totalTime);
        string totalTimeText = string.Format("{0:D2}:{1:D2}:{2:D2}",
            totalTimeSpan.Minutes,
            totalTimeSpan.Seconds,
            totalTimeSpan.Milliseconds / 10
        );
        totalTimeCounter.text = totalTimeText;

        float currentTime = playerVehicleController.getCurrentLapTime();
        TimeSpan currentTimeSpan = TimeSpan.FromSeconds(currentTime);
        string currentTimeText = string.Format("{0:D2}:{1:D2}:{2:D2}",
            currentTimeSpan.Minutes,
            currentTimeSpan.Seconds,
            currentTimeSpan.Milliseconds / 10
        );
        currentTimeCounter.text = currentTimeText;

        float bestTime = playerVehicleController.getBestLapTime();
        TimeSpan bestTimeSpan = TimeSpan.FromSeconds(bestTime);
        string bestTimeText = string.Format("{0:D2}:{1:D2}:{2:D2}",
            bestTimeSpan.Minutes,
            bestTimeSpan.Seconds,
            bestTimeSpan.Milliseconds / 10
        );
        if (bestTime > 0)
            bestTimeCounter.text = bestTimeText;

        int currentRank = getPlayerRank();
        positionCounter.text = currentRank + "/" + (enemyVehicleControllerList.Count + 1);

        if (countdownPanel.activeSelf && fader.color.a < 0.3)
            countdownDuration -= Time.deltaTime;

        if (countdownDuration <= -1.0f)
        {
            countdownPanel.SetActive(false);
        }
        else if (countdownDuration <= 0.0f)
        {
            countdownText.text = "GO";
            
            isRaceStarted = true;
        }
        else if (countdownDuration <= 1.0f)
        {
            countdownText.text = "1";
        }
        else if (countdownDuration <= 2.0f)
        {
            countdownText.text = "2";
        }
        else if (countdownDuration <= 3.0f)
        {
            countdownText.text = "3";
        }
        
    }

    public int GetSectorCount()
    {
        return trackInstrumentationController.sectorStartBehaviors.Count;
    }

    public Vector3 GetSectorStartPosition(int sectorNumber)
    {
        return trackInstrumentationController.GetSectorStartPosition(sectorNumber);
    }
    public int GetNextSectorIndex(int sectorNumber)
    {
        return (sectorNumber + 1) % GetSectorCount();
    }

    int getPlayerRank()
    {
        int rank = 1;
        foreach (VehicleController enemyVehicleController in enemyVehicleControllerList)
        {
            if (!playerVehicleController.isInFrontOfOtherVehicle(enemyVehicleController))
            {
                rank++;
            }
        }
        return rank;
    }

    void StartFadeIn()
    {
        StartCoroutine(FadeCoroutine());
    }

    IEnumerator FadeCoroutine()
    {
        float fadeRate;

        while (fader.color.a > 0)
        {
            fadeRate = Time.deltaTime / fadeDuration;
            fader.color = new Color(fader.color.r, fader.color.g, fader.color.b, Mathf.Clamp01(fader.color.a - fadeRate));
            yield return null;
        }
    }
}
