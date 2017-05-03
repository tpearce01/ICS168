using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyWindow : GenericWindow {
    private float _timeLeft = 60.0f;

    private void Update() {
        if ((ServerConnection.Instance.NumberOfConnections >= 1 && _timeLeft <= 0.0f) || ServerConnection.Instance.NumberOfConnections == 1) {
            GameManager.Instance.addPlayers();
            //ToggleWindows(WindowIDs.None, WindowIDs.Lobby);
            //MapGenerator.Instance.GenerateMap();
            SceneManager.LoadScene("TestMap");
        }

        _timeLeft -= Time.deltaTime;
    }
    public void GoToGame(string NewGameScene) {
        ToggleWindows(WindowIDs.Lobby, WindowIDs.None);
    }
}
