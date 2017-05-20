using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

    // Based on the player's number, it will color the sprite differently!
    // Player's number should be between 1 and 4.
    [SerializeField]
    [Range(0,3)]
    private int _playerNumber;
    public int PlayerNumber {
        get { return _playerNumber; }
        set { _playerNumber = value; }
    }

    private SpriteRenderer _sr;

    /// <summary>
    /// The username of the player.
    /// </summary>
    public string playerName;
    public string PlayerName {
        get { return playerName;  }
        set { playerName = value; }
    }


	// Use this for initialization
	void Start () {
        _sr = gameObject.GetComponent<SpriteRenderer>();
        setPlayerColor(_sr);
	}
		
    /// <summary>
    /// Sets the color of the player based on the player number. Doesn't return any value.
    /// </summary>
    /// <param name="sr">The sprite renderer that is attached to the player's game object.</param>
    void setPlayerColor(SpriteRenderer sr)
    {
        switch (_playerNumber)
        {
            case 0:
                sr.material.SetColor("_Color", Color.red);
                break;
            case 1:
                sr.material.SetColor("_Color", Color.blue);
                break;
            case 2:
                sr.material.SetColor("_Color", Color.green);
                break;
            case 3:
                sr.material.SetColor("_Color", Color.yellow);
                break;
        }
    }
}
