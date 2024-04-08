using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuManager : MonoBehaviour
{
	[SerializeField]
	GameObject Blackout;

	public void ExitGame()
	{
		AudioManager.instance.PlaySound("Click");
		Debug.Log("exit");
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
	}
	public void BeginGame(int index)
	{
		AudioManager.instance.PlaySound("Click");
		Instantiate(Blackout).GetComponentInChildren<BlackoutScript>().index = index;
	}

	public void ResetAudio()
    {
		AudioManager.instance.StopSound("ChaseMusic");
		AudioManager.instance.PlayLevelIfNot();
	}
}
