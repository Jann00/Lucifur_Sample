using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class YouDiedUI : MonoBehaviour
{
	[SerializeField]
	GameObject Blackout;

	public void OnRetry(InputAction.CallbackContext context)
    {
		Instantiate(Blackout).GetComponentInChildren<BlackoutScript>().index = SceneManager.GetActiveScene().buildIndex;
		AudioManager.instance.PlayLevelIfNot();
		AudioManager.instance.StopSound("ChaseMusic");

	}
}
