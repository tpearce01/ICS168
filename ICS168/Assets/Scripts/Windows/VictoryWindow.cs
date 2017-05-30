using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class VictoryWindow : GenericWindow {
    float redirectingSeconds = 10.0f;

    private void Update() {
        redirectingSeconds -= Time.deltaTime;
        
        if(redirectingSeconds <= 0.0f) {
            GameServerManager.Instance.InGamePlayers = 0;
            GameServerManager.Instance.SendJSONMessageToAll("10", QosType.Reliable);
            ToggleWindows(WindowIDs.Victory, WindowIDs.None);
        }else {
            GetComponentsInChildren<Text>()[1].text = "Redirecting in " + (int)redirectingSeconds + " seconds...";
        }
    }

    public void setText(string winner, bool winnerExists) {
        if (winnerExists)
            gameObject.GetComponentInChildren<Text>().text = winner + " wins!";
        else
            gameObject.GetComponentInChildren<Text>().text = "Draw!";
    }
}
