using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientIO : MonoBehaviour {

    private ControllableObject _inputHandler;
    //private Vector3 _pos;

    private PlayerIO _playerIO = new PlayerIO();
    private bool _buttonPressed = false;

    [HideInInspector] public bool gameInSession = false;

    private void OnEnable() {
        if (_inputHandler == null) {
            _inputHandler = GetComponent<ControllableObject>();
        }

    }

    private void Start() {
        InputManager.Instance.AddPlayer(GetComponent<ControllableObject>(),
            Resources.Load("Input/P1InputList", typeof(SOInputList)) as SOInputList);
    }

    private void Update() {
        if (gameInSession) {
            _playerIO.time = Time.time;


            if (_inputHandler.OnButtonDown(ButtonEnum.DeployBomb)) {
                _playerIO.button = ButtonEnum.DeployBomb;
                _buttonPressed = true;
            }

            //RIGHT
            if (_inputHandler.OnButtonDown(ButtonEnum.MoveRight)) {
                _playerIO.button = ButtonEnum.MoveRight;
                _buttonPressed = true;
            }

            //LEFT
            if (_inputHandler.OnButtonDown(ButtonEnum.MoveLeft)) {
                _playerIO.button = ButtonEnum.MoveLeft;
                _buttonPressed = true;
            }

            //UP
            if (_inputHandler.OnButtonDown(ButtonEnum.MoveUp)) {
                _playerIO.button = ButtonEnum.MoveUp;
                _buttonPressed = true;
            }

            //DOWN
            if (_inputHandler.OnButtonDown(ButtonEnum.MoveDown)) {
                _playerIO.button = ButtonEnum.MoveDown;
                _buttonPressed = true;
            }

            if (_buttonPressed) {
                ClientConnection.Instance.SendMessage(_playerIO);
                _buttonPressed = false;
            }

            //if (ValidPos(_pos)) {
            //    gameObject.transform.position = _pos;
            //}
        }

    }
}
