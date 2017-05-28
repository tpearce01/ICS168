using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyWindow : GenericWindow {

    [SerializeField] private Text _timeoutDisplay;
    [SerializeField] private float _defaultTimeout = 30.0f;
    [SerializeField] private float _gameStartsInDefault = 5.0f;
    [SerializeField] private float _minPlayers = -1;

    [SerializeField] private GameObject[] _players;
    [SerializeField] private Text[] _usernames;

    private float _timeout = -1.0f;
    private float _gameStartsIn = -1.0f;
    private bool _messageSent = false;

    // Sets lobby timer
    private void OnEnable() {
        _timeout = _defaultTimeout;

        _timeoutDisplay.text = "Waiting for players...";
        _gameStartsIn = _gameStartsInDefault;
        _messageSent = false;

        // Disables the boxes and sets usernames to blank
        foreach (GameObject player in _players) {
            player.SetActive(false);
        }

        foreach (Text username in _usernames) {
            username.text = "";
        }
    }

    private void Update() {
        if (GameServerManager.Instance.InGamePlayers >= _minPlayers && GameServerManager.Instance.InGamePlayers < GameServerManager.Instance.MaxConnections) {
            if (_timeout > 0.0f) {
                _timeout -= Time.deltaTime;
                _timeoutDisplay.text = "Waiting for players: " + (int)_timeout;
            }
            else if (_timeout <= 0.0f) {

                if (!_messageSent) {
                    GameServerManager.Instance.PreventDisconnects();
                    _messageSent = true;
                }

                _timeoutDisplay.text = "Game starts in: " + (int)_gameStartsIn;
                _gameStartsIn -= Time.deltaTime;

                if (_gameStartsIn <= 0.0f) {
                    GameServerManager.Instance.EnableClientControls();
                    MapGenerator.Instance.GenerateMap();
                    GameManager.Instance.StartGame();
                    ToggleWindows(WindowIDs.Lobby, WindowIDs.None);
                }
            }
        }
        else if (GameServerManager.Instance.InGamePlayers == GameServerManager.Instance.MaxConnections) {

            if (!_messageSent) {
                GameServerManager.Instance.PreventDisconnects();
                _messageSent = true;
            }

            _timeoutDisplay.text = "Game starts in: " + (int)_gameStartsIn;
            _gameStartsIn -= Time.deltaTime;

            if (_gameStartsIn <= 0.0f) {
                GameServerManager.Instance.EnableClientControls();
                MapGenerator.Instance.GenerateMap();
                GameManager.Instance.StartGame();
                ToggleWindows(WindowIDs.Lobby, WindowIDs.None);
            }
        }
        else if (GameServerManager.Instance.InGamePlayers < _minPlayers) {
            _timeoutDisplay.text = "Waiting for players...";
            _gameStartsIn = _gameStartsInDefault;
            _timeout = _defaultTimeout;
        }
        else if (GameServerManager.Instance.InGamePlayers <= 0) {
            ToggleWindows(WindowIDs.Lobby, WindowIDs.None);
        }
    }

    // Adds player to lobby and sets both that player's color and username based on playerNum (which is connectionID)
    public void AddPlayerToLobby(string username, int playerNum) {
        int actualPlayerNum = playerNum % 4;
        
        if (_players[actualPlayerNum].activeInHierarchy == false) {
            _players[actualPlayerNum].SetActive(true);
            _usernames[actualPlayerNum].text = username;
            MapGenerator.Instance.AddPlayerToMap(actualPlayerNum, username);
        }
    }
    
    // Removes player from lobby based on playerNum that was passed in
    public void RemovePlayerFromLobby(int playerNum) {
        int actualPlayerNum = playerNum % 4;

        if (_players[actualPlayerNum].activeInHierarchy == true) {
            _players[actualPlayerNum].SetActive(false);
            _usernames[actualPlayerNum].text = "";
            MapGenerator.Instance.RemovePlayerFromMap(actualPlayerNum);
        }
    }

    public void GoToGame(string NewGameScene) {
        ToggleWindows(WindowIDs.Lobby, WindowIDs.None);
    }
}
