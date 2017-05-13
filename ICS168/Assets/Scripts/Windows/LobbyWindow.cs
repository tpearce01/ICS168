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

    private int _inGamePlayers = -1;

    private float _timeout = -1.0f;
    private float _gameStartsIn = -1.0f;

    private void OnEnable() {
        _timeout = _defaultTimeout;

        _timeoutDisplay.text = "Waiting for players...";
        _gameStartsIn = _gameStartsInDefault;
    }

    private void Update() {

        _inGamePlayers = ServerConnection.Instance.InGamePlayers;

        if (_inGamePlayers >= _minPlayers && _inGamePlayers < ServerConnection.Instance.MaxConnections) {
            if (_timeout > 0.0f) {
                _timeout -= Time.deltaTime;
                _timeoutDisplay.text = "Waiting for players: " + (int)_timeout;
            }
            else if (_timeout <= 0.0f) {
                _timeoutDisplay.text = "Game starts in: " + (int)_gameStartsIn;
                _gameStartsIn -= Time.deltaTime;

                if (_gameStartsIn <= 0.0f) {
                    GameManager.Instance.StartGame();
                    ServerConnection.Instance.EnableClientControls();
                    MapGenerator.Instance.GenerateMap();
                    ToggleWindows(WindowIDs.Lobby, WindowIDs.None);
                }
            }
        }
        else if (_inGamePlayers == ServerConnection.Instance.MaxConnections) {
            _timeoutDisplay.text = "Game starts in: " + (int)_gameStartsIn;
            _gameStartsIn -= Time.deltaTime;

            if (_gameStartsIn <= 0.0f) {
                GameManager.Instance.StartGame();
                ServerConnection.Instance.EnableClientControls();
                MapGenerator.Instance.GenerateMap();
                ToggleWindows(WindowIDs.Lobby, WindowIDs.None);
            }
        }
        else if (_inGamePlayers < _minPlayers) {
            _timeoutDisplay.text = "Waiting for players...";
            _gameStartsIn = _gameStartsInDefault;
            _timeout = _defaultTimeout;
        }
        else if (_inGamePlayers <= 0) {
            ToggleWindows(WindowIDs.Lobby, WindowIDs.None);
        }
    }

    public void AddPlayerToLobby(string username) {

        if (_players[0].activeInHierarchy == false) {
            _players[0].SetActive(true);
            _usernames[0].text = username;
        }
        else if (_players[1].activeInHierarchy == false) {
            _players[1].SetActive(true);
            _usernames[1].text = username;
        }
        else if (_players[2].activeInHierarchy == false) {
            _players[2].SetActive(true);
            _usernames[2].text = username;
        }
        else if (_players[3].activeInHierarchy == false) {
            _players[3].SetActive(true);
            _usernames[3].text = username;
        }
    }

    public void RemovePlayerFromLobby(string username) {

        if (_usernames[0].text == username) {
            _players[0].SetActive(false);
            _usernames[0].text = "";
        }
        else if (_usernames[1].text == username) {
            _players[1].SetActive(false);
            _usernames[1].text = "";
        }
        else if (_usernames[2].text == username) {
            _players[2].SetActive(false);
            _usernames[2].text = "";
        }
        else if (_usernames[3].text == username) {
            _players[3].SetActive(false);
            _usernames[3].text = "";
        }
    }

    public void GoToGame(string NewGameScene) {
        ToggleWindows(WindowIDs.Lobby, WindowIDs.None);
    }
}
