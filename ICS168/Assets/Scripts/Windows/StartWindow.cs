using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartWindow : GenericWindow {

    private WindowIDs _toClose = WindowIDs.StartWindow;
    private Button[] _buttons;  // used to store the buttons in a window

    public void Update() {

        if (_buttons != null) {
            if (!ClientConnection.Instance.Connected) {
                if (_buttons[0].gameObject.GetComponent<Button>().interactable) { _buttons[0].gameObject.GetComponent<Button>().interactable = false; }
            }
            else if (ClientConnection.Instance.Connected) {
                if (!_buttons[0].gameObject.GetComponent<Button>().interactable) { _buttons[0].gameObject.GetComponent<Button>().interactable = true; }
            }
        }
        else {
            _buttons = GetComponentsInChildren<Button>();
        }
    }

    public override void Open() {
        base.Open();

        // This block of code does the resizing of the buttons when selected and de-selected
        _buttons = GetComponentsInChildren<Button>();
        float defaultWidth = _buttons[0].GetComponent<LayoutElement>().minWidth;

        for (int i = 0; i < _buttons.Length; ++i) {
            _buttons[i].GetComponent<LayoutElement>().preferredWidth = defaultWidth;
        }
    }

    public void PlayOnline() {
        ToggleWindows(WindowIDs.StartWindow, WindowIDs.Login);
    }

    // Closes StartWindow, and opens ControlsWindow
    public void Controls() {
        ToggleWindows(_toClose, WindowIDs.Controls);
    }

    // Closes StartWindow, and opens CreditsWindow
    public void Credits() {
        ToggleWindows(_toClose, WindowIDs.Credits);
    }

    // Exits the game application
    public void Quit() {
        Application.Quit();
    }
}
