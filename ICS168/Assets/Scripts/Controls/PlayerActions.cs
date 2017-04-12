using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is for all player actions which includes walking horizontal, wakling vertical, and deploying bombs.
/// </summary>
public class PlayerActions : MonoBehaviour {

    private ControllableObject _inputHandler;
    private int _playerNum = 1;

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

    private void DeployBomb() {

        if (_inputHandler.OnButtonDown(ButtonEnum.DeployBomb)) {
            Debug.Log("DEPLOY BOMB");
        }
    }

    private void Walking() {

        //RIGHT
        if (_inputHandler.OnButton(ButtonEnum.MoveRight)) {
            Debug.Log("WALKING RIGHT");
        }

        if (_inputHandler.OnButtonUp(ButtonEnum.MoveRight)) {
            Debug.Log("STOP WALKING RIGHT");
        }

        //LEFT
        if (_inputHandler.OnButton(ButtonEnum.MoveLeft)) {
            Debug.Log("WALKING LEFT");
        }

        if (_inputHandler.OnButtonUp(ButtonEnum.MoveLeft)) {
            Debug.Log("STOP WALKING LEFT");
        }

        //UP
        if (_inputHandler.OnButton(ButtonEnum.MoveUp)) {
            Debug.Log("WALKING UP");
        }

        if (_inputHandler.OnButtonUp(ButtonEnum.MoveUp)) {
            Debug.Log("STOP WALKING UP");
        }

        //DOWN
        if (_inputHandler.OnButton(ButtonEnum.MoveDown)) {
            Debug.Log("WALKING DOWN");
        }

        if (_inputHandler.OnButtonUp(ButtonEnum.MoveDown)) {
            Debug.Log("STOP WALKING DOWN");
        }
    }
}