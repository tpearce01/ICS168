using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is for all player actions which includes walking horizontal, wakling vertical, and deploying bombs.
/// </summary>
public class PlayerActions : MonoBehaviour {

    private int _playerNum = 1;

    private Vector3 _pos;

    bool ValidPos(Vector3 pos) {

        Tile[,] tileMap = MapGenerator.Instance.tileMap;

        if (tileMap != null) {
            Tile tile = tileMap[(int)pos.x, (int)pos.y].GetComponent<Tile>();
            if (tile.type == TileType.Wall || tile.type == TileType.Destructable || tile.type == TileType.WallPowerUp) {
                return false;
            }
        }

        return true;
    }

   
    //Al's test code
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PowerUp"))
        {
            Destroy(other.gameObject);
            ReplaceWithBasicTile();
            
            

        }
    }

    void ReplaceWithBasicTile()
    {
        Tile temp = Instantiate(MapGenerator.Instance.tileTypes[(int)TileType.Basic]).GetComponent<Tile>();
        temp.x = (int)gameObject.transform.position.x;
        temp.y = (int)gameObject.transform.position.y;
        temp.SetLocation();
        MapGenerator.Instance.tileMap[temp.x, temp.y] = temp;
    }

    public void RequestAction(PlayerIO command) {

        _pos = gameObject.transform.position;

        switch (command.button) {
            case ButtonEnum.DeployBomb:
                Spawner.Instance.GetComponent<Spawner>().SpawnObject(Prefab.Bomb, new Vector3(Mathf.Round(gameObject.transform.position.x), Mathf.Round(gameObject.transform.position.y), 0));
                break;
            case ButtonEnum.MoveDown:
                _pos.y -= 1;
                break;
            case ButtonEnum.MoveUp:
                _pos.y += 1;
                break;
            case ButtonEnum.MoveLeft:
                _pos.x -= 1;
                break;
            case ButtonEnum.MoveRight:
                _pos.x += 1;
                break;
        }

        if (ValidPos(_pos)) { gameObject.transform.position = _pos; }
    }
}