using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This script, when attached to an object, allows that object to be controlled using user input. */

// Class which keeps track of whether or not a button has been pressed and for the duration it has been pressed.
public class ButtonState {

    public bool isPressed = false;
    public bool onButtonDown = false;
    public bool onButton = false;
    public bool onButtonUp = false;
    public float pressTime = 0.0f;
}

public class ControllableObject : MonoBehaviour {

    // Other Action scripts will use this dictionary to query a button's state
    private Dictionary<ButtonEnum, ButtonState> _buttonStates = new Dictionary<ButtonEnum, ButtonState>();

    public virtual void SetButtonStates(ButtonEnum button, bool isPressed) {

        // Create a new button and check if it is not already in the dictionary
        if (!_buttonStates.ContainsKey(button)) {
            _buttonStates.Add(button, new ButtonState());
        }

        ButtonState buttonState = _buttonStates[button];

        // When the input has ceased / stopped, reset the pressTime and activate OnButtonUp.
        // Also reset OnButtonDown and OnButton to false.
        if (buttonState.isPressed && !isPressed) {

            _buttonStates[button].pressTime = 0.0f;
            _buttonStates[button].onButtonUp = true;
            _buttonStates[button].onButtonDown = false;
            _buttonStates[button].onButton = false;

        }

        // Ensure that OnButtonUp will only ever be seen as true for one frame.
        else if (!buttonState.isPressed && !isPressed) {
            _buttonStates[button].onButtonUp = false;
        }

        // When a button is being initially pressed.
        else if (!buttonState.isPressed && isPressed) {
            _buttonStates[button].onButtonDown = true;
        }

        // When a button is being initially and continually pressed.
        if ( (!buttonState.isPressed && isPressed) || (buttonState.isPressed && isPressed) ) {

            _buttonStates[button].onButton = true;

            // When the input is continuous
            if (buttonState.pressTime > 0.0f) {
                _buttonStates[button].onButtonDown = false;
            }

            _buttonStates[button].pressTime += Time.deltaTime;
        }

        //Update the status of the input
        _buttonStates[button].isPressed = isPressed;
    }

    public bool OnButtonDown(ButtonEnum button) {
        if (_buttonStates.ContainsKey(button)) {
            return _buttonStates[button].onButtonDown;
        }

        return false;
    }
    public bool OnButton(ButtonEnum button) {
        if (_buttonStates.ContainsKey(button)) {
            return _buttonStates[button].onButton;
        }

        return false;
    }
    public bool OnButtonUp(ButtonEnum button) {
        if (_buttonStates.ContainsKey(button)) {
            return _buttonStates[button].onButtonUp;
        }

        return false;
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
