using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI lapCounter;
    public TextMeshProUGUI totalTimeCounter;
    public TextMeshProUGUI currentTimeCounter;

    public VehicleController playerVehicleController;

    void Start()
    {
        
    }

    void Update()
    {
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
    }
}
