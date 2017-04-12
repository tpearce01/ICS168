using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This script manages all user input */

// An enum for buttons/keys users can press for action
public enum ButtonEnum {
    MoveRight,
    MoveLeft,
    MoveUp,
    MoveDown,
    DeployBomb
}

// An enum to check if we should continue or stop reading input
public enum ConditionEnum {
    GreaterThanOffValue,
    LessThanOffValue
}

[System.Serializable]
public class Player {

    //Constructor
    public Player(ControllableObject player, SOInputList soInputList) {
        controllableObject = player;
        inputList = soInputList;
    }

    public ControllableObject controllableObject;
    public SOInputList inputList;
    public bool canTakeInput = true;
}

[System.Serializable]
public class InputAxisState {

    [SerializeField]
    private string _axisName;           // Which axis will this input be looking for.
    [SerializeField]
    private ButtonEnum _button;         // Which button will be mapped to the assigned axis
    public ButtonEnum Button {          // Returns this _button
        get { return _button; }
    }

    [SerializeField]
    private float _offValue;            // when should it stop reading input from the player.
    [SerializeField]
    private ConditionEnum _condition;   // When should it starting reading input from the player.

    public bool IsPressed {
        get {
            float isPressed = 0.0f;

            isPressed = Input.GetAxis(_axisName);

            switch (_condition) {
                case ConditionEnum.GreaterThanOffValue:
                    return isPressed > _offValue;
                case ConditionEnum.LessThanOffValue:
                    return isPressed < _offValue;
            }

            return false;
        }
    }
}

public class InputManager : Singleton<InputManager> {

    [SerializeField]
    private List<Player> _players;

    //[SerializeField]
    //private ControllableObject _player;
    //[SerializeField]
    //private InputAxisState[] _inputs;

    public void Update() {

        int length = _players.Count;

        if (length > 0) {
            for (int i = 0; i < length; ++i) {
                if (_players[i].canTakeInput) {
                    foreach (InputAxisState input in _players[i].inputList.inputs) {
                        _players[i].controllableObject.SetButtonStates(input.Button, input.IsPressed);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="player"></param>
    /// <param name="inputList"></param>
    public void AddPlayer(ControllableObject player, SOInputList inputList) {
        _players.Add(new Player(player, inputList));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerNum"></param>
    /// <param name="value"></param>
    public void TogglePlayerInput(int playerNum, bool value) {
        _players[playerNum].canTakeInput = value;
    }
}
