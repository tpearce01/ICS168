using System.Collections;
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
    [SerializeField] private int _numOfAlivePlayers;
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

    /// <summary>
    /// Used to store the usernames from login.
    /// </summary>
    private string[] usernameArray = new string[4];

    public string getUsername(int index) {
        return usernameArray[index];
    }

    public void setUsername(int index, string playerName) {
        usernameArray[index] = playerName;
    }

    private void OnEnable() {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 15;
    }

    public void StartGame() {
        if (GameObject.Find("Player1(Clone)") != null) {
            GameObject.Find("Player1(Clone)").GetComponent<PlayerScript>().PlayerName = getUsername(0);
        }
        if (GameObject.Find("Player2(Clone)") != null) {
            GameObject.Find("Player2(Clone)").GetComponent<PlayerScript>().PlayerName = getUsername(1);
        }
        if (GameObject.Find("Player3(Clone)") != null) {
            GameObject.Find("Player3(Clone)").GetComponent<PlayerScript>().PlayerName = getUsername(2);
        }
        if (GameObject.Find("Player4(Clone)") != null) {
            GameObject.Find("Player4(Clone)").GetComponent<PlayerScript>().PlayerName = getUsername(3);
        }
        
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
                WindowManager.Instance.GetComponentInChildren<VictoryWindow>().setText("", false);
                WindowManager.Instance.ToggleWindows(WindowIDs.Game, WindowIDs.Victory);
                DestroyEverything();
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
    public void decAlivePlayers() {
        _numOfAlivePlayers -= 1;
        if (_numOfAlivePlayers <= 1) {
            findWinner();
        }
        //Is the game over?
    }

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
    void findWinner() {
        if (OnEndGame != null) {
            OnEndGame(WindowIDs.Game, WindowIDs.Victory);
        }

        GameObject[] ps = GameObject.FindGameObjectsWithTag("Player"); //Tells us the winners

        if (ps.Length > 1) {
            WindowManager.Instance.GetComponentInChildren<VictoryWindow>().setText("", false);
            WindowManager.Instance.ToggleWindows(WindowIDs.Game, WindowIDs.Victory);
            DestroyEverything();
        } 
        else if (ps.Length == 1) {
            WindowManager.Instance.GetComponentInChildren<VictoryWindow>().setText(ps[0].GetComponent<PlayerScript>().PlayerName, true);
            WindowManager.Instance.ToggleWindows(WindowIDs.Game, WindowIDs.Victory);
            DestroyEverything();
        } 
        else {
            Debug.Log("Wasted");
            
        }
    }

    private void DestroyEverything() {
        GameObject[] allWalls               = GameObject.FindGameObjectsWithTag("Wall");
        GameObject[] allFloors              = GameObject.FindGameObjectsWithTag("Floor");
        GameObject[] allDestructableWalls   = GameObject.FindGameObjectsWithTag("Destructable");
        GameObject[] allPowerupWalls        = GameObject.FindGameObjectsWithTag("WallPowerUp");
        GameObject[] allRemainingPlayers    = GameObject.FindGameObjectsWithTag("Player");


        for (int i = 0; i < allWalls.Length; ++i)
            Destroy(allWalls[i]);
        for (int i = 0; i < allFloors.Length; ++i)
            Destroy(allFloors[i]);
        for (int i = 0; i < allDestructableWalls.Length; ++i)
            Destroy(allDestructableWalls[i]);
        for (int i = 0; i < allPowerupWalls.Length; ++i)
            Destroy(allPowerupWalls[i]);
        for (int i = 0; i < allRemainingPlayers.Length; ++i)
            Destroy(allRemainingPlayers[i]);
    }

    /// <summary>
    /// Displays the victory text.
    /// </summary>
    //void displayVictor()
    //{ 
    //    _victoryText.enabled = true;
    //    _victoryText.text = _winner.ToString() + " wins!";
    //}
}