using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetTimer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayTime.instance.ResetTimer();
        PlayTime.instance.isPlaying = true;

    }

}
