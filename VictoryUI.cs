using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VictoryUI : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    // Start is called before the first frame update
    void Start()
    {
        timerText.text = PlayTime.instance.minutes + ":" + PlayTime.instance.seconds.ToString("#.00");
        Cursor.visible = true;
    }

}
