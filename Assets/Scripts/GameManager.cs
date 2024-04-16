using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameMode
{
    Summon,
    Race,
    TimeAttack
}

public class GameManager : MonoBehaviour
{
    public GameMode gameMode = GameMode.Summon;

    public AudioSource audioThrusters;
    public AudioSource audioMusic;

    public AudioClip idleThrusters;
    public AudioClip activeThrusters;
    public AudioClip turboThrusters;
    public AudioClip music;

    public TextMeshProUGUI lapCounter;
    public TextMeshProUGUI totalTimeCounter;
    public TextMeshProUGUI currentTimeCounter;
    public TextMeshProUGUI bestTimeCounter;
    public TextMeshProUGUI positionCounter;
    public TextMeshProUGUI reverseLabel;

    public GameObject resultsPanel;
    public TextMeshProUGUI finalPositionLabel;
    public TextMeshProUGUI finalTimeLabel;

    public GameObject countdownPanel;
    public TextMeshProUGUI countdownText;

    public VehicleController playerVehicleController;
    List<VehicleController> enemyVehicleControllerList = new List<VehicleController>();
    public TrackInstrumentationController trackInstrumentationController;

    public Image fader;
    public float fadeInDuration = 2f;
    public float fadeOutDuration = 3f;

    float countdownDuration = 4f;
    public static GameManager instance;
    public int numberOfLaps = 3;

    public bool isCountdownStarted = false;
    public bool isRaceStarted = false;
    public bool isRaceFinished = false;

    public GameObject enemyPrefab;
    public GameObject defaultEnemies1;
    public GameObject defaultEnemies2;

    void Start()
    {
        audioThrusters.loop = true;
        audioThrusters.clip = idleThrusters;

        audioMusic.loop = true;
        audioMusic.clip = music;
        audioMusic.Play();

        if (instance == null)
        {             
            instance = this;
        }

        PlayerPrefs.SetInt("CurrentTime", -1);
        PlayerPrefs.Save();

        switch (PlayerPrefs.GetInt("GameMode", 0))
        {
            case 0:
                gameMode = GameMode.Summon;
                break;
            case 1:
                gameMode = GameMode.Race;
                break;
            case 2:
                gameMode = GameMode.TimeAttack;
                break;
        }

        StartFadeIn();
        defaultEnemies1.SetActive(gameMode == GameMode.Summon || gameMode == GameMode.Race);
        defaultEnemies2.SetActive(gameMode == GameMode.Race);

        Debug.Log(gameMode == GameMode.Summon || gameMode == GameMode.Race);
        Debug.Log(gameMode == GameMode.Race);

        if (gameMode == GameMode.Summon || gameMode == GameMode.Race)
        {
            foreach (VehicleController enemyController in defaultEnemies1.GetComponentsInChildren<VehicleController>())
            {
                enemyVehicleControllerList.Add(enemyController);
            }
        }

        if (gameMode == GameMode.Race)
        {
            foreach (VehicleController enemyController in defaultEnemies2.GetComponentsInChildren<VehicleController>())
            {
                enemyVehicleControllerList.Add(enemyController);
            }
        }
    }

    void Update()
    {
        int currentLap = Mathf.Max(1, playerVehicleController.getStartedLapsCount());
        if (currentLap > numberOfLaps)
            currentLap = numberOfLaps;
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
            countdownText.text = "GO!";
            
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
            isCountdownStarted = true;
            audioThrusters.Play();
        }

        updateReverseLabel();

        if (playerVehicleController.getStartedLapsCount() > numberOfLaps && !isRaceFinished)
        {
            isRaceFinished = true;
            playerVehicleController.isEnemy = true;
            StartFadeOut();
        }

        if (isRaceFinished && fader.color.a > 0.5 && !resultsPanel.activeSelf)
        {
            PlayerPrefs.SetInt("CurrentTime", (int)(bestTime * 1000));
            PlayerPrefs.Save();
            resultsPanel.SetActive(true);
            if (gameMode != GameMode.TimeAttack)
                finalPositionLabel.text = "YOU FINISHED IN POSITION " + currentRank + " OUT OF " + (enemyVehicleControllerList.Count + 1);
            finalTimeLabel.text = "YOUR BEST LAP TIME WAS " + bestTimeText;
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
        StartCoroutine(FadeInCoroutine());
    }

    IEnumerator FadeInCoroutine()
    {
        float fadeRate;
        float progress = 0;

        while (fader.color.a > 0)
        {
            progress += Time.deltaTime / fadeInDuration;
            fadeRate = progress * progress;
            fader.color = new Color(fader.color.r, fader.color.g, fader.color.b, Mathf.Clamp01(1 - fadeRate));
            yield return null;
        }
    }

    void StartFadeOut()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    IEnumerator FadeOutCoroutine()
    {
        float fadeRate;

        while (fader.color.a < 1)
        {
            fadeRate = Time.deltaTime / fadeOutDuration;
            fader.color = new Color(fader.color.r, fader.color.g, fader.color.b, Mathf.Clamp01(fader.color.a + fadeRate));
            yield return null;
        }
    }

    public void updateReverseLabel()
    {
        if (playerVehicleController.isReversingOnTrack() && !reverseLabel.gameObject.activeSelf)
        {
            reverseLabel.gameObject.SetActive(true);
        }
        else if (!playerVehicleController.isReversingOnTrack() && reverseLabel.gameObject.activeSelf)
        {
            reverseLabel.gameObject.SetActive(false);
        }
    }

    public void addOpponent(Vector3 position, Quaternion rotation, int lastCrossedSector)
    {
        GameObject enemy = Instantiate(enemyPrefab, position, rotation);
        VehicleController enemyController = enemy.GetComponent<VehicleController>();
        enemyController.forceLastCrossedSector(lastCrossedSector);
        enemyController.lapTimes.Clear();
        while (enemyController.getStartedLapsCount() < playerVehicleController.getStartedLapsCount())
        {
            enemyController.newLap();
        }
        enemyVehicleControllerList.Add(enemyController);
        enemyController.enableTurbo(true);
        enemyController.checkPointPos = position;
        enemyController.checkPointRot = rotation;
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void Retry()
    {
        SceneManager.LoadScene("RaceScene");
    }

    public void Leaderboard()
    {
        SceneManager.LoadScene("LeaderboardScene");
    }

    public void SetIdleThrustersAudio()
    {
        audioThrusters.Stop();
        audioThrusters.clip = idleThrusters;
        audioThrusters.loop = true;
        audioThrusters.Play();
    }
    public void SetActiveThrustersAudio()
    {
        audioThrusters.Stop();
        audioThrusters.clip = activeThrusters;
        audioThrusters.loop = true;
        audioThrusters.Play();
    }
    public void SetTurboThrustersAudio()
    {
        audioThrusters.Stop();
        audioThrusters.clip = turboThrusters;
        audioThrusters.loop = true;
        audioThrusters.Play();
    }
}
