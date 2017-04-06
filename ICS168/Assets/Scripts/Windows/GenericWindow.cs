using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum WindowIDs {
    StartWindow = 0,
    Controls = 1,
    Credits = 2,
    None = -1
}

public class GenericWindow : MonoBehaviour {

    public delegate void GenericWindowEvent(WindowIDs close, WindowIDs open);
    public static event GenericWindowEvent OnToggleWindows;

    private Button[] _buttons;

    protected virtual void Display(bool value) {
        gameObject.SetActive(value);
    }

    public virtual void Open() {
        Display(true);

        _buttons = GetComponentsInChildren<Button>();
        float defaultWidth = _buttons[0].GetComponent<LayoutElement>().minWidth;

        for (int i = 0; i < _buttons.Length; ++i) {
            _buttons[i].GetComponent<LayoutElement>().preferredWidth = defaultWidth;
        }
    }

    public virtual void Close() {
        Display(false);
    }

    protected virtual void ToggleWindows(WindowIDs close, WindowIDs open) {
        if (OnToggleWindows != null) { OnToggleWindows(close, open); }
    }
}
