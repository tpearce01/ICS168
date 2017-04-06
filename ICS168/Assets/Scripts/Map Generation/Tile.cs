using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int x;
    public int y;
    public TileType type;

    public void SetLocation()
    {
        gameObject.transform.position = new Vector2(x, y);
    }
}

public enum TileType
{
    Wall = 0,
    StartLocation = 1,
    Basic = 2,
}
