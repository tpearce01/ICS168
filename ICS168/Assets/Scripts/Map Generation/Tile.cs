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
        gameObject.transform.position = new Vector2(x, y);
    }
}

public enum TileType
{
    Wall = 0,
    Basic = 1,
}
