using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This script, when attached to an object, allows that object to be controlled using user input. */

// Class which keeps track of whether or not a button has been pressed and for the duration it has been pressed.
public class ButtonState {

    public bool isPressed = false;
    public float pressTime = 0.0f;
}

public class ControllableObject : MonoBehaviour {

    public delegate void ControllableObjectEvent(ButtonEnum button);
    public static event ControllableObjectEvent OnButton;
    public static event ControllableObjectEvent OnButtonDown;
    public static event ControllableObjectEvent OnButtonUp;

    // Other Action scripts will use this dictionary to query a button's state
    private Dictionary<ButtonEnum, ButtonState> _buttonStates = new Dictionary<ButtonEnum, ButtonState>();

    public void SetButtonStates(ButtonEnum button, bool isPressed) {

        // Create a new button and check if it is not already in the dictionary
        if (!_buttonStates.ContainsKey(button)) {
            _buttonStates.Add(button, new ButtonState());
        }

        // When the input has ceased / stopped
        if (_buttonStates[button].isPressed && !isPressed) {
            _buttonStates[button].pressTime = 0.0f;

            if (OnButtonUp != null) { OnButtonUp(button); }
        }
        else if (_buttonStates[button].isPressed && isPressed) {

            // When the input has initially begun
            if (_buttonStates[button].pressTime == 0.0f) {
                if (OnButtonDown != null) { OnButtonDown(button); }
            }

            // When the input is continuous
            if (_buttonStates[button].pressTime >= 0.0f) {
                if (OnButton != null) { OnButton(button); }
            }

            _buttonStates[button].pressTime += Time.deltaTime;
        }

        //Update the status of the input
        _buttonStates[button].isPressed = isPressed;
    }

    // Returns true if the button being passed in is pressed, false otherwise
    public bool GetButtonPress(ButtonEnum button) {
        if (_buttonStates.ContainsKey(button)) {
            return _buttonStates[button].isPressed;
        }

        return false;
    }

    // Returns the duration of how long the button being passed in has been pressed, 0.0 if not pressed
    public float GetButtonPressTime(ButtonEnum button) {
        if (_buttonStates.ContainsKey(button)) {
            return _buttonStates[button].pressTime;
        }

        return 0.0f;
    }
}
