using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyWindow : GenericWindow {

    [SerializeField] private float _defaultTimeout = 30.0f;
    [SerializeField] private float _minPlayers = -1;

    private float _timeout = -1.0f;

    private void OnEnable() {
        _timeout = _defaultTimeout;
    }

    private void Update() {
        if ((ServerConnection.Instance.InGamePlayers >= _minPlayers && _timeout <= 0.0f) || 
            ServerConnection.Instance.InGamePlayers == ServerConnection.Instance.MaxConnections) {

            GameManager.Instance.StartGame();
            ServerConnection.Instance.EnableClientControls();
            MapGenerator.Instance.GenerateMap();
            ToggleWindows(WindowIDs.Lobby, WindowIDs.None);
        }
        else if (ServerConnection.Instance.InGamePlayers < 1) {
            ToggleWindows(WindowIDs.Lobby, WindowIDs.None);
        }

        _timeout -= Time.deltaTime;
    }
    public void GoToGame(string NewGameScene) {
        ToggleWindows(WindowIDs.Lobby, WindowIDs.None);
    }
}
