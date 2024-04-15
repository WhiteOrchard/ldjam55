using UnityEngine;

public class ScoreSubmissionManager : MonoBehaviour
{
	private const string CanSubmitScoreKey = "CanSubmitScore";

	// Method to set the score submission state
	public void SetScoreSubmissionState(bool canSubmit)
	{
		Debug.Log($"Score submission state set to: {canSubmit}");
		PlayerPrefs.SetInt(CanSubmitScoreKey, canSubmit ? 1 : 0);
		PlayerPrefs.Save(); // Ensures the preferences are saved immediately
	}

	// Method to check if score submission is allowed
	public bool CanSubmitScore()
	{
		// Return true if the stored value is 1, otherwise false
		return PlayerPrefs.GetInt(CanSubmitScoreKey, 0) == 1;
	}

	// Method to disable score submission without a parameter
	public void DisableScoreSubmission()
	{
		SetScoreSubmissionState(false);
	}

	// Use this to initialize the PlayerPrefs on game start
	private void Start()
	{
		// Set default state to false when the game starts
		SetScoreSubmissionState(false);
	}
}
