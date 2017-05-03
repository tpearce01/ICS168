using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameWindow : GenericWindow {

    Text[] timerrange;
    float timeLeft = 60.0f;
    GameObject p1;
    
    private void Update()
    {
        p1 = GameObject.FindGameObjectWithTag("Player");
        timerrange = GetComponentsInChildren<Text>();

        
        timeLeft -= Time.deltaTime;

        if(timeLeft < 0.0f)
        {
            print("BAGUETTES!!!");
        }
        else
        {
            timerrange[0].text = "Time Left: " + (int)timeLeft;
            timerrange[1].text = "Range: " + p1.GetComponent<PlayerActions>().range;
        }
    }
}
