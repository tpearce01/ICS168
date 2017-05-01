using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int x;   //X coordinate
    public int y;   //Y coordinate
    public TileType type;

    //Set the tiles location to it's specified coordinate
    public void SetLocation()
    {
        gameObject.transform.position = new Vector3(x, y, 1);
    }

    public void SetLocation(int z)
    {
        gameObject.transform.position = new Vector3(x, y, z);
    }
}

public enum TileType
{
    Wall = 0,
    Basic = 1,
    Destructable = 2,
    PowerUp = 3,
    WallPowerUp = 4,
    Player1 = 5,
    Player2 = 6,
    Player3 = 7,
    Player4 = 8
}
