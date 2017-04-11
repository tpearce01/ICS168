using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameWindow : GenericWindow {

    Text timer;
    float timeLeft = 60.0f;

    private void Update()
    {
        timer = GetComponentInChildren<Text>();
        timeLeft -= Time.deltaTime;

        if(timeLeft < 0.0f)
        {
            print("BAGUETTES!!!");
        }
        else
        {
            timer.text = "Time Left: " + (int)timeLeft;
        }
    }
}
