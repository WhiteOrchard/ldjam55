using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
	public GameObject[] uiElementsToDisable;

	public void LoadMenuScene()
	{
		SceneManager.LoadScene("MenuScene");
	}

	public void LoadLeaderboard()
	{
		foreach (GameObject uiElement in uiElementsToDisable)
		{
			if (uiElement != null)
			{
				uiElement.SetActive(false);
			}
		}

		SceneManager.LoadScene("Leaderboard");
	}

}
