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

    private void OnEnable() {
        _timeout = _defaultTimeout;

        _timeoutDisplay.text = "Waiting for players...";
        _gameStartsIn = _gameStartsInDefault;
        _messageSent = false;

        foreach (GameObject player in _players) {
            player.SetActive(false);
        }

        foreach (Text username in _usernames) {
            username.text = "";
        }
    }

    private void Update() {

        if (ServerConnection.Instance.InGamePlayers >= _minPlayers && ServerConnection.Instance.InGamePlayers < ServerConnection.Instance.MaxConnections) {
            if (_timeout > 0.0f) {
                _timeout -= Time.deltaTime;
                _timeoutDisplay.text = "Waiting for players: " + (int)_timeout;
            }
            else if (_timeout <= 0.0f) {

                if (!_messageSent) {
                    ServerConnection.Instance.PreventDisconnects();
                    _messageSent = true;
                }

                _timeoutDisplay.text = "Game starts in: " + (int)_gameStartsIn;
                _gameStartsIn -= Time.deltaTime;

                if (_gameStartsIn <= 0.0f) {
                    ServerConnection.Instance.EnableClientControls();

                    // do MapGenerator.Instance.CanGenerateMap = true; After generating map, MapGenerator starts the game
                    MapGenerator.Instance.GenerateMap();
                    GameManager.Instance.StartGame();

                    ToggleWindows(WindowIDs.Lobby, WindowIDs.None);
                }
            }
        }
        else if (ServerConnection.Instance.InGamePlayers == ServerConnection.Instance.MaxConnections) {

            if (!_messageSent) {
                ServerConnection.Instance.PreventDisconnects();
                _messageSent = true;
            }

            _timeoutDisplay.text = "Game starts in: " + (int)_gameStartsIn;
            _gameStartsIn -= Time.deltaTime;

            if (_gameStartsIn <= 0.0f) {
                ServerConnection.Instance.EnableClientControls();
                MapGenerator.Instance.GenerateMap();
                GameManager.Instance.StartGame();
                ToggleWindows(WindowIDs.Lobby, WindowIDs.None);
            }
        }
        else if (ServerConnection.Instance.InGamePlayers < _minPlayers) {
            _timeoutDisplay.text = "Waiting for players...";
            _gameStartsIn = _gameStartsInDefault;
            _timeout = _defaultTimeout;
        }
        else if (ServerConnection.Instance.InGamePlayers <= 0) {
            ToggleWindows(WindowIDs.Lobby, WindowIDs.None);
        }
    }

    public void AddPlayerToLobby(string username, int playerNum) {
        int playerWho = 0;

        // This if makes sure that this player is part of this group
        if (playerNum > 3) {
            playerWho = playerNum % 4;
        }else {
            playerWho = playerNum;
        }

        if (_players[playerNum].activeInHierarchy == false) {
            _players[playerNum].SetActive(true);
            //GameManager.Instance.setUsername(0, username);
            //AddPlayer
            _usernames[playerNum].text = username;
        }

        /*
        if (_players[0].activeInHierarchy == false) {
            _players[0].SetActive(true);
            GameManager.Instance.setUsername(0, username);
            _usernames[0].text = username;
            
        }
        else if (_players[1].activeInHierarchy == false) {
            _players[1].SetActive(true);
            GameManager.Instance.setUsername(1, username);
            _usernames[1].text = username;
        }
        else if (_players[2].activeInHierarchy == false) {
            _players[2].SetActive(true);
            GameManager.Instance.setUsername(2, username);
            _usernames[2].text = username;
        }
        else if (_players[3].activeInHierarchy == false) {
            _players[3].SetActive(true);
            GameManager.Instance.setUsername(3, username);
            _usernames[3].text = username;
        }
        */
    }

    public void RemovePlayerFromLobby(string username) {

        if (_usernames[0].text == username) {
            _players[0].SetActive(false);
            GameManager.Instance.setUsername(0, "");
            _usernames[0].text = "";
        }
        else if (_usernames[1].text == username) {
            _players[1].SetActive(false);
            GameManager.Instance.setUsername(1, "");
            _usernames[1].text = "";
        }
        else if (_usernames[2].text == username) {
            _players[2].SetActive(false);
            GameManager.Instance.setUsername(2, "");
            _usernames[2].text = "";
        }
        else if (_usernames[3].text == username) {
            _players[3].SetActive(false);
            GameManager.Instance.setUsername(3, "");
            _usernames[3].text = "";
        }
    }

    public void GoToGame(string NewGameScene) {
        ToggleWindows(WindowIDs.Lobby, WindowIDs.None);
    }
}
