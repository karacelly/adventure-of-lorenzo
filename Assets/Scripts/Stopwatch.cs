using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Stopwatch : MonoBehaviour
{
    private float timeStart, timeElapsed;
    private int minute, seconds;
    private bool isFirst;

    public TMP_Text text;

    // Start is called before the first frame update
    void Start()
    {
        isFirst = true;

    }

    // Update is called once per frame
    void Update()
    {
        if (!UIController.inDialogue)
        {
            if (isFirst)
            {
                timeStart = Time.time;
                isFirst = false;
            }
            else
            {
                timeElapsed = Time.time - timeStart;
                seconds = (int)timeElapsed % 60;
                minute = (int)timeElapsed / 60;
            }            
            text.text = minute.ToString("00") + ":" + seconds.ToString("00");
        }
    }
}
