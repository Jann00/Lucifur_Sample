using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayTime : MonoBehaviour
{
    public static PlayTime instance;
    public float seconds = 0;
    public int minutes = 0;
    public bool isPlaying = false;
    // Start is called before the first frame update
    void Start()
    {
		if (instance != null && instance != this)
		{
			Destroy(gameObject);
			instance.ResetTimer();
			return;
		}
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlaying)
        {
            seconds += Time.deltaTime;
            if (seconds >= 60)
            {
                seconds = 0;
                minutes++;
            }
        }
    }

    public void ResetTimer()
    {
        seconds = 0;
        minutes = 0;
    }
}
