using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt("GameMode", 0);
        PlayerPrefs.SetInt("CurrentTime", -1);
        PlayerPrefs.Save();
    }

    public void StartSummonMode()
    {
        PlayerPrefs.SetInt("GameMode", 0);
        PlayerPrefs.Save();
        SceneManager.LoadScene("RaceScene");
    }

    public void StartRaceMode()
    {
        PlayerPrefs.SetInt("GameMode", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene("RaceScene");
    }

    public void StartTimeAttackMode()
    {
        PlayerPrefs.SetInt("GameMode", 2);
        PlayerPrefs.Save();
        SceneManager.LoadScene("RaceScene");
    }

    public void OpenLeaderboard()
    {
        SceneManager.LoadScene("LeaderboardScene");
    }

    public void OpenCredits()
    {

    }
}
