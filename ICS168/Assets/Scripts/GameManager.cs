using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : Singleton<GameManager> {

	public delegate void GameManagerEvent(WindowIDs close, WindowIDs open);
	public static event GameManagerEvent OnEndGame;

    private class PlayerInfo {
        public string username;
        public int playerNum;
    }

    private List<PlayerInfo> _listOfPlayers = new List<PlayerInfo>();

    [SerializeField] private List<PlayerActions> _players = new List<PlayerActions>();
    private List<GameObject> playerReferences = new List<GameObject>(); //references to the player game objects that the mapgenerator instantiated

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

    private int _winner = 0;

    private void OnEnable() {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 15;
    }

    public void StartGame() {
        _numOfAlivePlayers = playerReferences.Count;
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
            _currTime -= Time.deltaTime;

            if (_currTime <= 0.0f || _numOfAlivePlayers <= 1) {
                findWinner();
                DestroyEverything();
                _gameInSession = false;
            }
        }
    }

    public void PlayerActions(int playerID, PlayerIO command) {
        // This check will prevent any null references to players which have been killed.
        if (playerReferences[playerID-1] != null) { playerReferences[playerID - 1].GetComponent<PlayerActions>().RequestAction(command); }
    }

    public void LeaveGame(int playerID) {
        _numOfAlivePlayers--;
        if (_players[playerID - 1] != null) { _players[playerID - 1].LeaveGame(); }
    }
    public void getPlayerReferences() {
        playerReferences.AddRange(GameObject.FindGameObjectsWithTag("Player"));
    }
    /// <summary>
    /// Call this function to decrement the number of alive players by 1.
    /// </summary>
    public void decAlivePlayers() {
        _numOfAlivePlayers -= 1;
    }
    
    /// <summary>
    /// Returns the player number of the winner, 0 if there is a draw.
    /// </summary>
    /// <returns>int</returns>
    void findWinner() {
        if (OnEndGame != null) {
            OnEndGame(WindowIDs.Game, WindowIDs.Victory);
        }

        if (_numOfAlivePlayers > 1 || _numOfAlivePlayers == 0) {
            WindowManager.Instance.GetComponentInChildren<VictoryWindow>().setText("", false);
           
        } 
        else if (_numOfAlivePlayers == 1) {
            WindowManager.Instance.GetComponentInChildren<VictoryWindow>().setText(GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerActions>().playerName, true);
        } 
        else {
            Debug.Log("Wasted");         
        }

        WindowManager.Instance.ToggleWindows(WindowIDs.Game, WindowIDs.Victory);
        DestroyEverything();
    }

    private void DestroyEverything() {
        GameObject[] allBombs               = GameObject.FindGameObjectsWithTag("Bomb");
        GameObject[] allExplosions          = GameObject.FindGameObjectsWithTag("Explosion");
        GameObject[] allWalls               = GameObject.FindGameObjectsWithTag("Wall");
        GameObject[] allFloors              = GameObject.FindGameObjectsWithTag("Floor");
        GameObject[] allDestructableWalls   = GameObject.FindGameObjectsWithTag("Destructable");
        GameObject[] allPowerupWalls        = GameObject.FindGameObjectsWithTag("WallPowerUp");
        GameObject[] allPowerups            = GameObject.FindGameObjectsWithTag("PowerUp");
        GameObject[] allRemainingPlayers    = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < allBombs.Length; ++i)
            Destroy(allBombs[i]);
        for (int i = 0; i < allExplosions.Length; ++i)
            Destroy(allExplosions[i]);
        for (int i = 0; i < allWalls.Length; ++i)
            Destroy(allWalls[i]);
        for (int i = 0; i < allFloors.Length; ++i)
            Destroy(allFloors[i]);
        for (int i = 0; i < allDestructableWalls.Length; ++i)
            Destroy(allDestructableWalls[i]);
        for (int i = 0; i < allPowerupWalls.Length; ++i)
            Destroy(allPowerupWalls[i]);
        for (int i = 0; i < allPowerups.Length; ++i)
            Destroy(allPowerups[i]);
        for (int i = 0; i < allRemainingPlayers.Length; ++i)
            Destroy(allRemainingPlayers[i]);
    }
}