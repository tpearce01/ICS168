using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyWindow : GenericWindow {

    [SerializeField] private float _defaultTimeout = 30.0f;
    [SerializeField] private float _minPlayers = -1;

    [SerializeField] private GameObject[] _players;
    [SerializeField] private Text[] _usernames;

    private float _timeout = -1.0f;

    private void OnEnable() {
        _timeout = _defaultTimeout;
    }

    private void Update() {

        // If the minimum number of players are logged in and the timer runs out, start the game.
        // Or if all four players are logged in, start the game.
        if ((ServerConnection.Instance.InGamePlayers >= _minPlayers && _timeout <= 0.0f)) {

            GameManager.Instance.StartGame();
            ServerConnection.Instance.EnableClientControls();
            MapGenerator.Instance.GenerateMap();
            ToggleWindows(WindowIDs.Lobby, WindowIDs.None);
        }

        // If all players leave, close out the lobby window.
        else if (ServerConnection.Instance.InGamePlayers < 1) {
            ToggleWindows(WindowIDs.Lobby, WindowIDs.None);
        }

        _timeout -= Time.deltaTime;


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
        else if (_usernames[0].text == username) {
            _players[1].SetActive(false);
            _usernames[1].text = "";
        }
        else if (_usernames[0].text == username) {
            _players[2].SetActive(false);
            _usernames[2].text = "";
        }
        else if (_usernames[0].text == username) {
            _players[3].SetActive(false);
            _usernames[3].text = "";
        }
    }

    public void GoToGame(string NewGameScene) {
        ToggleWindows(WindowIDs.Lobby, WindowIDs.None);
    }
}
