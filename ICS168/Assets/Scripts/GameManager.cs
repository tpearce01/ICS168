using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : Singleton<GameManager> {

	public delegate void GameManagerEvent(WindowIDs close, WindowIDs open);
	public static event GameManagerEvent OnEndGame;

    /*** TIME TRACKING ATTRIBUTES ***/
    // Signifies the maximum time for the round.
    [Header("Game Settings")]
    [SerializeField] private float _roundTime;
    public float RoundTime {
        get { return _roundTime; }
    }
    /// <summary>
    /// When the number of alive players is at this number, the round will end and a winner will be determined.
    /// Defaults to 1.
    /// </summary>
    [SerializeField] private int _numberOfPlayersToEndRound = 0;
    // Signifies the current time
    private float _currTime;
    public float CurrentTime {
        get { return _currTime; }
    }

    /*** GAME TRACKING ATTRIBUTES ***/
    // Stores references to all the player game objects in the game
    private List<GameObject> findPlayers = new List<GameObject>();
    private GameObject[] playerReferences = new GameObject[4];

    [Space]
    [Header("Tracking Variables")]
    // Used to track the number of players that are still alive internally.
    [SerializeField] private int _numOfAlivePlayers;
    public int NumOfAlivePlayers {
        get { return _numOfAlivePlayers; }
        set { _numOfAlivePlayers = value; }
    }

    // Used to track if the game is over or not.
    private bool _isGameOver;
    public bool IsGameOver {
        get { return _isGameOver; }
    }

    // Used to track if game is in session
    private bool _isGameInSession = false;
    
    private void OnEnable() {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 15;
    }

    // Initialization
    public void StartGame() {

        _numOfAlivePlayers = findPlayers.Count;
        foreach (GameObject playerObject in findPlayers) {
            if (playerObject != null) {
                string playerName = playerObject.GetComponent<PlayerActions>().playerName;
                int playerNumber = playerObject.GetComponent<PlayerActions>().PlayerNumber;
                WindowManager.Instance.GetComponentInChildren<PlayerInfo>().setPlayerText(playerName, playerNumber);
                WindowManager.Instance.GetComponentInChildren<PlayerInfo>().togglePlayerInfo(playerNumber, true);
            }
            _currTime = _roundTime;
            _isGameOver = false;
            _isGameInSession = true;
        }
    }

    // Resets game mamager to original state
    public void ResetGameManager() {
        _isGameInSession = false;
        _isGameOver = true;
        _numOfAlivePlayers = 0;
    }

    // Update is called once per frame
    void Update () {
        if (_isGameInSession) {
            _currTime -= Time.deltaTime;

            if (_currTime <= 0.0f || _numOfAlivePlayers <= _numberOfPlayersToEndRound) {
                findWinner();
                DestroyEverything();
                _isGameInSession = false;
            }
        }
    }

    // Called by ServerConnection when a client sends in move commands
    public void PlayerActions(int playerID, PlayerIO command) {
        if (playerReferences[playerID-2] != null) {
            playerReferences[playerID - 2].GetComponent<PlayerActions>().RequestAction(command);
        }
    }

    // Called by ServerConnection when the client disconnects from an ongoing game
    public void LeaveGame(int playerID) {
        decAlivePlayers();
        if (playerReferences.Length > 0 && playerReferences[playerID - 2] != null)
        {
            WindowManager.Instance.GetComponentInChildren<PlayerInfo>().setPlayerText(
                "", playerReferences[playerID-2].GetComponent<PlayerActions>().PlayerNumber);
            WindowManager.Instance.GetComponentInChildren<PlayerInfo>().togglePlayerInfo(
                playerReferences[playerID - 2].GetComponent<PlayerActions>().PlayerNumber, false);
            playerReferences[playerID - 2].GetComponent<PlayerActions>().LeaveGame();
        }
    }

    // Called by MapGenerator after map is generated. Gets references to the instantiated players on the map.
    public void getPlayerReferences() {
        findPlayers.AddRange(GameObject.FindGameObjectsWithTag("Player"));

        foreach(GameObject player in findPlayers) {
            if (player != null) {
                playerReferences[player.GetComponent<PlayerActions>().PlayerNumber] = player;
            }
        }
    }
    
    // Decrements the number of alive players
    public void decAlivePlayers() {
        _numOfAlivePlayers -= 1;
    }
    
    // Determines the result of the game, a draw or a win. Called when the game timer runs out or when the number of alive players is <= 1.
    void findWinner() {
        if (OnEndGame != null) {
            OnEndGame(WindowIDs.Game, WindowIDs.Victory);
        }

        if (_numOfAlivePlayers > 1 || _numOfAlivePlayers == 0) { // DRAW
            WindowManager.Instance.GetComponentInChildren<VictoryWindow>().setText("", false);
           
        } 
        else if (_numOfAlivePlayers == 1) { // 1-PLAYER WIN
            WindowManager.Instance.GetComponentInChildren<VictoryWindow>().setText(GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerActions>().playerName, true);
        } 
        else { // Something went wrong
            Debug.Log("Wasted");         
        }

        WindowManager.Instance.ToggleWindows(WindowIDs.PlayerInfo, WindowIDs.Victory);
        DestroyEverything();
    }

    // Destroys all the tiles in the map, called once game ends and winner is found
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

        playerReferences = new GameObject[4];
        findPlayers.Clear();
        _numOfAlivePlayers = 0;
    }
}