using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyWindow : GenericWindow {
    private float _timeLeft = 60.0f;

    private void Update() {
        if ((ServerConnection.Instance.NumberOfConnections >= 2 && _timeLeft <= 0.0f) || ServerConnection.Instance.NumberOfConnections == 4) {

            GameManager.Instance.BeginGame = true;
            MapGenerator.Instance.GenerateMap();
            ToggleWindows(WindowIDs.Lobby, WindowIDs.None);
        }

        _timeLeft -= Time.deltaTime;
    }
    public void GoToGame(string NewGameScene) {
        ToggleWindows(WindowIDs.Lobby, WindowIDs.None);
    }
}
