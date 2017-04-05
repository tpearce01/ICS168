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



	// Use this for initialization
	void Start () {
        _numOfAlivePlayers = _numOfPlayers;
        _currTime = _roundTime;
        _isGameOver = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //Call this function to decrement the number of alive players by 1.
    void decAlivePlayers()
    {
        _numOfAlivePlayers -= 1;
    }

    /*
     * Getters
    */

    //Returns the number of players in the game.
    int getNumOfPlayers()
    {
        return _numOfPlayers;
    }

    //Returns the number of players that are still alive in this game.
    int getNumOfAlivePlayers()
    {
        return _numOfAlivePlayers;
    }

    //Returns the round time set.
    float getRoundTime()
    {
        return _roundTime;
    }

    //Returns the current time in the match.
    float getCurrentTime()
    {
        return _currTime;
    }

    //Returns a bool stating whether or not the game is over.
    bool getIsGameOver()
    {
        return _isGameOver;
    }
}

