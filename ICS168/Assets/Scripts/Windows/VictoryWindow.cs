using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryWindow : GenericWindow {
    public float redirectingSeconds = 5.0f;

    private void Update() {
        redirectingSeconds -= Time.deltaTime;
        gameObject.GetComponents<Text>()[1].text = "Remaining time " + redirectingSeconds + "...";

        if(redirectingSeconds <= 0.0f) {
            ServerConnection.Instance.SendJSONMessage("10");
        }
    }

    public void setText(string winner, bool winnerExists) {
        if (winnerExists)
            gameObject.GetComponentInChildren<Text>().text = winner + " wins!";
        else
            gameObject.GetComponentInChildren<Text>().text = "Draw!";
    }
    //placeholder
}
