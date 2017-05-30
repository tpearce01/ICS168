using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class VictoryWindow : GenericWindow {
    float redirectingSeconds = 10.0f;
    private float _timer = 0.0f;

    private void OnEnable() {
        _timer = redirectingSeconds;
    }

    private void Update() {
        _timer -= Time.deltaTime;
        
        if(_timer <= 0.0f) {
            GameServerManager.Instance.InGamePlayers = 0;
            GameServerManager.Instance.SendJSONMessageToAll("10", QosType.Reliable);
            ToggleWindows(WindowIDs.Victory, WindowIDs.None);

        }else {
            GetComponentsInChildren<Text>()[1].text = "Redirecting in " + (int)_timer + " seconds...";
        }
    }

    public void setText(string winner, bool winnerExists) {
        if (winnerExists)
            gameObject.GetComponentInChildren<Text>().text = winner + " wins!";
        else
            gameObject.GetComponentInChildren<Text>().text = "Draw!";
    }
}
