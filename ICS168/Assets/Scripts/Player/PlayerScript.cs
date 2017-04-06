using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

    // Based on the player's number, it will color the sprite differently!
    // Player's number should be between 1 and 4.
    [Range(1,4)]
    public int playerNumber;

    private SpriteRenderer _sr;

    //This attribute tracks whether or not the player is alive or not.
    private bool _isAlive;

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
        if (collision.gameObject.name == "Explosion")
        {
            _isAlive = false;
        }
    }
    //Sets the color of the player based on the player number.
    void setPlayerColor(SpriteRenderer sr)
    {
        switch (playerNumber)
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

    //Returns if the player is alive.
    public bool isAlive()
    {
        return _isAlive;
    }

    public int getPlayerNumber()
    {
        return playerNumber;
    }
}
