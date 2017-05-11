using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyWindow : GenericWindow {
    private float _timeLeft = 30.0f;

    private void Update() {
        if ((GameManager.Instance.SignedInPlayers >= 2 && _timeLeft <= 0.0f) ||  GameManager.Instance.SignedInPlayers == 3) {
            ServerConnection.Instance.EnableClientControls();
            MapGenerator.Instance.GenerateMap();
            ToggleWindows(WindowIDs.Lobby, WindowIDs.None);
        }

        _timeLeft -= Time.deltaTime;
    }
    public void GoToGame(string NewGameScene) {
        ToggleWindows(WindowIDs.Lobby, WindowIDs.None);
    }
}
