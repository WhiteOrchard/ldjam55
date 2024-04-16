using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;

public class LeaderboardManager : MonoBehaviour
{
	public TMP_InputField nameInput;
	public Button submitButton;
	public ScoreDisplayManager scoreDisplayManager;

	public GameObject submitUtilities;
    public TextMeshProUGUI bestLapTimeLabel;
    public TextMeshProUGUI currentRankLabel;

	private string leaderboardURL = "https://leaderboard.cyprien.workers.dev/add";

	int bestLapTime;

	void Start()
	{
		bestLapTime = PlayerPrefs.GetInt("CurrentTime", -1);
		if (bestLapTime == -1)
		{
            submitUtilities.SetActive(false);
        }
        else
        {
            submitUtilities.SetActive(true);
            TimeSpan bestTimeSpan = TimeSpan.FromMilliseconds(bestLapTime);
            string bestTimeText = string.Format("{0:D2}:{1:D2}:{2:D2}",
                bestTimeSpan.Minutes,
                bestTimeSpan.Seconds,
                bestTimeSpan.Milliseconds / 10
            );
            bestLapTimeLabel.text = "BEST LAP TIME: " + bestTimeText;

			int rank = 1;
			List<int> scores = scoreDisplayManager.scores;
			scores.Sort();
			foreach (int score in scores)
			{
				if (score < bestLapTime)
				{
                    rank++;
                }
				else
				{
                    break;
                }
			}
			currentRankLabel.text = "POSITION " + rank + " OUT OF " + (scores.Count + 1) + " RECORDED LAP TIMES" ;
        }

    }

	public void OnSubmitClicked()
	{
		submitButton.gameObject.SetActive(false);
		if (nameInput.text.Length > 16)
			StartCoroutine(SubmitScore(nameInput.text.Substring(0, 16), bestLapTime));
        else
            StartCoroutine(SubmitScore(nameInput.text, bestLapTime));
    }

	IEnumerator SubmitScore(string pseudo, int score)
	{
		WWWForm form = new WWWForm();
		form.AddField("pseudo", pseudo);
		form.AddField("score", score.ToString());

		using (UnityWebRequest www = UnityWebRequest.Post(leaderboardURL, form))
		{
			yield return www.SendWebRequest();

			if (www.result != UnityWebRequest.Result.Success)
			{
				Debug.LogError("Error submitting score: " + www.error);
			}
			else
			{
				Debug.Log("Score submitted successfully!");
				StartCoroutine(scoreDisplayManager.GetScores());
			}
		}
	}

	public void backToMenu()
	{
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
    }
}
