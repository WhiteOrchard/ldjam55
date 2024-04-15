using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class LeaderboardManager : MonoBehaviour
{
	public TMP_InputField nameInput;
	public TMP_InputField scoreInput;
	public Button submitButton;

	private string leaderboardURL = "https://leaderboard.cyprien.workers.dev/add";

	void Start()
	{
		submitButton.onClick.AddListener(OnSubmitClicked);
	}

	public void OnSubmitClicked()
	{
		if (int.TryParse(scoreInput.text, out int score))
		{
			StartCoroutine(SubmitScore(nameInput.text, score));
		}
		else
		{
			Debug.LogError("Invalid score input. Please enter a valid integer.");
		}
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
			}
		}
	}
}
