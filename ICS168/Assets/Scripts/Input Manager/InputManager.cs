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
    private ControllableObject _player;
    [SerializeField]
    private InputAxisState[] _inputs;

    public void Update() {

        foreach(InputAxisState input in _inputs) {
            _player.SetButtonStates(input.Button, input.IsPressed);
        }
    }
}
