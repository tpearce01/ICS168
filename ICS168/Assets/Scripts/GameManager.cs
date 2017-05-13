﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : Singleton<GameManager> {

	public delegate void GameManagerEvent(WindowIDs close, WindowIDs open);
	public static event GameManagerEvent OnEndGame;

    [SerializeField] private List<PlayerActions> _players = new List<PlayerActions>();

    //This variable is used to track the number of players that are still alive internally.
    //It should not be visible for any reason on the inspector.
    private int _numOfAlivePlayers;
    public int NumOfAlivePlayers {
        get { return _numOfAlivePlayers; }
    }

    //Time tracking attributes
    //This attribute signifies the maximum time for the round.
    [SerializeField] private float _roundTime;
    public float RoundTime {
        get { return _roundTime; }
    }

    //This attribute signifies the current time, it shouldn't be seen in the inspector.
    private float _currTime;
    public float CurrentTime {
        get { return _currTime; }
    }

    //Game tracking attributes
    //This attribute is used to track if the game is over or not. Should not appear in the inspector.
    private bool _isGameOver;

    private bool _gameInSession = false;
    public bool GameInSession {
        get { return _gameInSession; }
        set { _gameInSession = value; }
    }

    //Player attributes.
    /* These attributes are used to track the game objects that will represent the players.
     * I'm guessing that we will be having 4 players in the networked version, similar to the normal bomberman game.
     *
     * I'll have 4 slots in there for now, but we can leave the other three empty OR have them be dummy characters the player needs to kill.
    */

    private int _winner = 0;

    [SerializeField]
    private Text _victoryText;

    /// <summary>
    /// Time that the victory screen will be shown.
    /// </summary>
    [SerializeField]
    private float _timeToShowVictory;

    private void OnEnable() {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 15;
    }

    public void StartGame() {
        _numOfAlivePlayers = GameObject.FindGameObjectsWithTag("Player").Length;

        _currTime = _roundTime;
        _isGameOver = false;
        _gameInSession = true;
    }

    public void ResetGameManager() {
        _gameInSession = false;
        _isGameOver = true;
        _players.Clear();
    }

    // Update is called once per frame
    void Update () {

        if (_gameInSession) {

            //Count down the timer
            _currTime -= Time.deltaTime;

            if (_currTime < 0.0f) {
                _isGameOver = true;
            }

            if (_isGameOver) {
                _timeToShowVictory -= Time.deltaTime;
                //Do whatever you need to do once the game is over. 
                //_winner = findWinner();
                //displayVictor();
                //After a few seconds, go to the next scene.
                if (_timeToShowVictory < 0.0f) {
                    SceneManager.LoadScene("UI and Controls Testing");
                }
            }
        }
    }

    public void PlayerActions(int playerID, PlayerIO command) {

        // This check will prevent any null references to players which have been killed.
        if (_players[playerID-1] != null) { _players[playerID - 1].RequestAction(command); }
    }

    public void addPlayers() {

        // The map generated the maximum amount of players the map can handle.
        // It then finds all of them.
        GameObject[] tempPlayers = GameObject.FindGameObjectsWithTag("Player");

        // Keep track of the number of active players.
        _numOfAlivePlayers = ServerConnection.Instance.InGamePlayers;

        // Add the players to the list of players.
        for(int i = 0; i < _numOfAlivePlayers; i++) {
            _players.Add(tempPlayers[i].GetComponent<PlayerActions>());
        }

        // For all extra players which were created, destroy the leftovers.
        for(int i = _numOfAlivePlayers; i < tempPlayers.Length; i++) {
            Destroy(tempPlayers[i]);
        }
    }


    /// <summary>
    /// Call this function to decrement the number of alive players by 1.
    /// </summary>
  //  public void decAlivePlayers()
  //  {
  //      _numOfAlivePlayers -= 1;
		//if (_numOfAlivePlayers <= 1) {
		//	findWinner ();
		//}
		////Is the game over?
  //  }

    /// <summary>
    /// Returns a bool stating whether or not the game is over.
    /// </summary>
    /// <returns>bool</returns>
    //public bool getIsGameOver()
    //{
    //    return _isGameOver;
    //}

    /// <summary>
    /// Returns the player number of the winner, 0 if there is a draw.
    /// </summary>
    /// <returns>int</returns>
  //  int findWinner()
  //  {
		//if (OnEndGame != null) {
		//	OnEndGame (WindowIDs.Game, WindowIDs.Victory);
		//} 

		//GameObject[] ps = GameObject.FindGameObjectsWithTag ("Player"); //Tells us the winners

		//if (ps.Length >= 1) {
		//	return 0;
		//} /*else if (ps.Length == 1) {
		//	return ps [0].GetComponent<PlayerScript> ().PlayerNumber;
		//} */else {
		//	Debug.Log ("Wasted");
		//	return 0;
		//}
  //  }

    /// <summary>
    /// Displays the victory text.
    /// </summary>
    //void displayVictor()
    //{ 
    //    _victoryText.enabled = true;
    //    _victoryText.text = _winner.ToString() + " wins!";
    //}
}