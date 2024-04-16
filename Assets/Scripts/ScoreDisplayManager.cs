using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using TMPro;
using System;
using System.Collections.Generic;

public class ScoreDisplayManager : MonoBehaviour
{
	public GameObject scorePrefab;
	public Transform scoresParent;
	public List<int> scores = new List<int>();

	private string getScoresUrl = "https://leaderboard.cyprien.workers.dev/get";

	[System.Serializable]
	public class Score
	{
		public string pseudo;
		public int score;
	}

	[System.Serializable]
	public class ScoreArray
	{
		public Score[] items;
	}

	void Start()
	{
		StartCoroutine(GetScores());
	}

	public IEnumerator GetScores()
	{
        ClearScores();
        using (UnityWebRequest www = UnityWebRequest.Get(getScoresUrl))
		{
			yield return www.SendWebRequest();

			if (www.result != UnityWebRequest.Result.Success)
			{
				Debug.LogError("Failed to retrieve scores: " + www.error);
			}
			else
			{
				Debug.Log("Raw JSON received: " + www.downloadHandler.text);
				ProcessScores(www.downloadHandler.text);
			}
		}
	}

	void ProcessScores(string jsonArray)
	{
		string adjustedJson = "{\"items\":" + jsonArray + "}";
		Debug.Log("Adjusted JSON: " + adjustedJson);
		ScoreArray scoreArray = JsonUtility.FromJson<ScoreArray>(adjustedJson);

		if (scoreArray == null || scoreArray.items == null)
		{
			Debug.LogError("Failed to parse the JSON string.");
			return;
		}

		System.Array.Sort(scoreArray.items, (x, y) => y.score.CompareTo(x.score));

		foreach (var score in scoreArray.items)
		{
			if (scorePrefab == null || scoresParent == null)
			{
				Debug.LogError("Score prefab or parent transform not assigned in the inspector.");
				return;
			}

			GameObject scoreItem = Instantiate(scorePrefab, scoresParent);
			var nameTextComponent = scoreItem.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
			var scoreTextComponent = scoreItem.transform.Find("ScoreText")?.GetComponent<TextMeshProUGUI>();

			if (nameTextComponent == null || scoreTextComponent == null)
			{
				Debug.LogError("NameText or ScoreText component not found on instantiated score item.");
				Destroy(scoreItem);
				continue;
			}

			nameTextComponent.text = score.pseudo;
            TimeSpan bestTimeSpan = TimeSpan.FromMilliseconds(score.score);
            string bestTimeText = string.Format("{0:D2}:{1:D2}:{2:D2}",
                bestTimeSpan.Minutes,
                bestTimeSpan.Seconds,
                bestTimeSpan.Milliseconds / 10
            );
            scoreTextComponent.text = bestTimeText;

            scores.Add(score.score);
        }
	}

	void ClearScores()
	{
        foreach (Transform child in scoresParent)
		{
            Destroy(child.gameObject);
        }
    }
}
