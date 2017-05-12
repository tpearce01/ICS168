﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is for all player actions which includes walking horizontal, wakling vertical, and deploying bombs.
/// </summary>
public class PlayerActions : MonoBehaviour {

    //possibly vectors in the future for each specific client accessing different index
    public int range = 2;
    
    //private ControllableObject _inputHandler;
    private int _playerNum = 1;
    private Vector3 _pos;

    //private void OnEnable() {
    //    if (_inputHandler == null) {
    //        _inputHandler = GetComponent<ControllableObject>();
    //    }
    //}

    //private void Start() {

    //    InputManager.Instance.AddPlayer(GetComponent<ControllableObject>(),
    //        Resources.Load("Input/P" + _playerNum + "InputList", typeof(SOInputList)) as SOInputList);
    //}

    //private void Update() {
    //    DeployBomb();
    //    Walking();
    //}

    bool ValidPos(Vector3 pos) {

        Tile[,] tileMap = MapGenerator.Instance.tileMap;

        if (tileMap != null) {
            //MissingReferenceException: The object of type 'Tile' has been destroyed but you are still trying to access it.
            Tile tile = tileMap[(int)pos.x, (int)pos.y].GetComponent<Tile>();
            if (tile.type == TileType.Wall || tile.type == TileType.Destructable || tile.type == TileType.WallPowerUp) {
                return false;
            }
        }

        return true;
    }

    public void RequestAction(PlayerIO command) {
        //MissingReferenceException: The object of type 'PlayerActions' has been destroyed but you are still trying to access it.
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


    //Al's test code
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PowerUp"))
        {
            Destroy(other.gameObject);
            ReplaceWithBasicTile();
            range++;
        }
    }

    void ReplaceWithBasicTile()
    {
        //Debug.Log("ReplaceWithBasicTile");
        Tile temp = Instantiate(MapGenerator.Instance.tileTypes[(int)TileType.Basic]).GetComponent<Tile>();
        temp.x = (int)gameObject.transform.position.x;
        temp.y = (int)gameObject.transform.position.y;
        temp.SetLocation();
        MapGenerator.Instance.tileMap[temp.x, temp.y] = temp;
    }


    //private void DeployBomb() {

    //    if (_inputHandler.OnButtonDown(ButtonEnum.DeployBomb)) {
    //        Spawner.Instance.GetComponent<Spawner>().SpawnObject(Prefab.Bomb, new Vector3(Mathf.Round(gameObject.transform.position.x), Mathf.Round(gameObject.transform.position.y), 0)).GetComponent<Bomb>().range = range;
    //    }
    //}

    //private void Walking() {
    //    _pos = gameObject.transform.position;

    //    //RIGHT
    //    if (_inputHandler.OnButtonDown(ButtonEnum.MoveRight)) {
    //        _pos.x += 1;
    //    }

    //    //if (_inputHandler.OnButtonUp(ButtonEnum.MoveRight)) {
    //    //    Debug.Log("STOP WALKING RIGHT");
    //    //}

    //    //LEFT
    //    if (_inputHandler.OnButtonDown(ButtonEnum.MoveLeft)) {
    //        _pos.x -= 1;
    //    }

    //    //if (_inputHandler.OnButtonUp(ButtonEnum.MoveLeft)) {
    //    //    Debug.Log("STOP WALKING LEFT");
    //    //}

    //    //UP
    //    if (_inputHandler.OnButtonDown(ButtonEnum.MoveUp)) {
    //        _pos.y += 1;
    //    }

    //    //if (_inputHandler.OnButtonUp(ButtonEnum.MoveUp)) {
    //    //    Debug.Log("STOP WALKING UP");
    //    //}

    //    //DOWN
    //    if (_inputHandler.OnButtonDown(ButtonEnum.MoveDown)) {
    //        _pos.y -= 1;
    //    }

    //    //if (_inputHandler.OnButtonUp(ButtonEnum.MoveDown)) {
    //    //    Debug.Log("STOP WALKING DOWN");
    //    //}
    //    //Debug.Log(_pos);
    //    if (ValidPos(_pos)) {
    //        gameObject.transform.position = _pos;
    //    }
    //}
}
