using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginWindow : GenericWindow {

    public delegate void LoginWindowEvent();
    public static event LoginWindowEvent OnLogin;

    public void NewAccount() {
        ToggleWindows(WindowIDs.Login, WindowIDs.NewAccount);
    }

    public void Login() {
        if (OnLogin != null) { OnLogin(); }
    }

    public void BackToMain() {
        ToggleWindows(WindowIDs.Login, WindowIDs.StartWindow);
    }
}
