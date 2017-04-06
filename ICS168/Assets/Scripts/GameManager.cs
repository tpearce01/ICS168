using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    //Player Tracking Attributes
    //This attribute signifies the maximum amount of players that are playing in the scene.
    [SerializeField]
    private int _numOfPlayers;

    //This variable is used to track the number of players that are still alive internally.
    //It should not be visible for any reason on the inspector.
    private int _numOfAlivePlayers;

    //Time tracking attributes
    //This attribute signifies the maximum time for the round.
    [SerializeField]
    private float _roundTime;

    //This attribute signifies the current time, it shouldn't be seen in the inspector.
    private float _currTime;

    //Game tracking attributes
    //This attribute is used to track if the game is over or not. Should not appear in the inspector.
    private bool _isGameOver;

    //Player attributes.
    /* These attributes are used to track the game objects that will represent the players.
     * I'm guessing that we will be having 4 players in the networked version, similar to the normal bomberman game.
     *
     * I'll have 4 slots in there for now, but we can leave the other three empty OR have them be dummy characters the player needs to kill.
    */
    [SerializeField]
    private GameObject[] _players;

    private int winner = 0;

	// Use this for initialization
	void Start () {
        _numOfAlivePlayers = _numOfPlayers;
        _currTime = _roundTime;
        _isGameOver = false;
	}
	
	// Update is called once per frame
	void Update () {

        //Count down the timer
        _currTime -= Time.deltaTime;

        if (_currTime < 0.0f)
        {
            _isGameOver = true;
        }

        if (_isGameOver)
        {
            //Do whatever you need to do once the game is over. 
            winner = findWinner();
        }
	}

    //Call this function to decrement the number of alive players by 1.
    public void decAlivePlayers()
    {
        _numOfAlivePlayers -= 1;
    }

    /*
     * Getters
    */

    //Returns the number of players in the game.
    public int getNumOfPlayers()
    {
        return _numOfPlayers;
    }

    //Returns the number of players that are still alive in this game.
    public int getNumOfAlivePlayers()
    {
        return _numOfAlivePlayers;
    }

    //Returns the round time set.
    public float getRoundTime()
    {
        return _roundTime;
    }

    //Returns the current time in the match.
    public float getCurrentTime()
    {
        return _currTime;
    }

    //Returns a bool stating whether or not the game is over.
    public bool getIsGameOver()
    {
        return _isGameOver;
    }

    //Returns the player number of the winner, 0 if there is a draw.
    int findWinner()
    {
        int numOfWinners = 0;
        int winnerNumber = 0;
        for (int i = 0; i < _players.Length; i++)
        {
            if (_players[i].GetComponent<PlayerScript>().isAlive())
            {
                ++numOfWinners;
                winnerNumber = _players[i].GetComponent<PlayerScript>().getPlayerNumber();
            }
            if (numOfWinners > 1)
            {
                winnerNumber = 0;
                break;
            }
        }
        return winnerNumber;
    }
}

