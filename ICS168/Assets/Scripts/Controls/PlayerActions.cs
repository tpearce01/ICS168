using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is for all player actions which includes walking horizontal, wakling vertical, and deploying bombs.
/// </summary>
public class PlayerActions : MonoBehaviour {

    private ControllableObject _inputHandler;
    private int _playerNum = 1;

    private Vector3 _pos;

    private void OnEnable() {
        if (_inputHandler == null) {
            _inputHandler = GetComponent<ControllableObject>();
        }
    }

    private void Start() {

        InputManager.Instance.AddPlayer(GetComponent<ControllableObject>(),
            Resources.Load("Input/P" + _playerNum + "InputList", typeof(SOInputList)) as SOInputList);
    }

    private void Update() {
        DeployBomb();
        Walking();
    }

    bool ValidPos(Vector3 pos) {

        //Debug.Log(pos);
        Tile[,] tileMap = MapGenerator.GO.GetComponent<MapGenerator>().tileMap;

        Tile tile = tileMap[(int)pos.x, (int)pos.y].GetComponent<Tile>();
        if (tile.type == TileType.Wall || tile.type == TileType.Destructable) {
            return false;
        }

        return true;
    }

    private void DeployBomb() {

        if (_inputHandler.OnButtonDown(ButtonEnum.DeployBomb)) {
            Spawner.Instance.GetComponent<Spawner>().SpawnObject(Prefab.Bomb, new Vector3(Mathf.Round(gameObject.transform.position.x), Mathf.Round(gameObject.transform.position.y), 0));
        }
    }

    private void Walking() {

        _pos = gameObject.transform.position;

        //RIGHT
        if (_inputHandler.OnButtonDown(ButtonEnum.MoveRight)) {
            _pos.x += 1;
        }

        //if (_inputHandler.OnButtonUp(ButtonEnum.MoveRight)) {
        //    Debug.Log("STOP WALKING RIGHT");
        //}

        //LEFT
        if (_inputHandler.OnButtonDown(ButtonEnum.MoveLeft)) {
            _pos.x -= 1;
        }

        //if (_inputHandler.OnButtonUp(ButtonEnum.MoveLeft)) {
        //    Debug.Log("STOP WALKING LEFT");
        //}

        //UP
        if (_inputHandler.OnButtonDown(ButtonEnum.MoveUp)) {
            _pos.y += 1;
        }

        //if (_inputHandler.OnButtonUp(ButtonEnum.MoveUp)) {
        //    Debug.Log("STOP WALKING UP");
        //}

        //DOWN
        if (_inputHandler.OnButtonDown(ButtonEnum.MoveDown)) {
            _pos.y -= 1;
        }

        //if (_inputHandler.OnButtonUp(ButtonEnum.MoveDown)) {
        //    Debug.Log("STOP WALKING DOWN");
        //}
        //Debug.Log(_pos);
        if (ValidPos(_pos)) {
            gameObject.transform.position = _pos;
        }
    }
}