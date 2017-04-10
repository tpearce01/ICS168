using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

    // Based on the player's number, it will color the sprite differently!
    // Player's number should be between 1 and 4.
    [SerializeField]
    [Range(1,4)]
    private int _playerNumber;
    public int PlayerNumber {
        get { return _playerNumber; }
    }

    private SpriteRenderer _sr;

    //This attribute tracks whether or not the player is alive or not.
    private bool _isAlive;
    public bool IsAlive {
        get { return _isAlive; }
    }

    /// <summary>
    /// The username of the player.
    /// </summary>
    public string playerName;


	// Use this for initialization
	void Start () {
        _isAlive = true;
        _sr = gameObject.GetComponent<SpriteRenderer>();
        setPlayerColor(_sr);
	}
	
	// Update is called once per frame
	void Update () {
        if (!_isAlive)
        {
            //Don't do anything!
        }
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_isAlive)
        {
            if (collision.gameObject.name == "Explosion")
            {
                _isAlive = false;
                GetComponent<GameManager>().decAlivePlayers();
            }
        }
    }
    /// <summary>
    /// Sets the color of the player based on the player number. Doesn't return any value.
    /// </summary>
    /// <param name="sr">The sprite renderer that is attached to the player's game object.</param>
    void setPlayerColor(SpriteRenderer sr)
    {
        switch (_playerNumber)
        {
            case 1:
                sr.material.SetColor("_Color", Color.red);
                break;
            case 2:
                sr.material.SetColor("_Color", Color.blue);
                break;
            case 3:
                sr.material.SetColor("_Color", Color.green);
                break;
            case 4:
                sr.material.SetColor("_Color", Color.yellow);
                break;
        }
    }

    ///// <summary>
    ///// Returns a bool that represents whether or not the player is alive.
    ///// </summary>
    ///// <returns>bool</returns>
    //public bool isAlive()
    //{
    //    return _isAlive;
    //}

    ///// <summary>
    ///// Returns the player's number, it is a value between 1 and 4.
    ///// </summary>
    ///// <returns>int</returns>
    //public int getPlayerNumber()
    //{
    //    return playerNumber;
    //}
}
