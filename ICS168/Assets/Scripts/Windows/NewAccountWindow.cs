using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewAccountWindow : GenericWindow {

	public void BackToMain() {
        ToggleWindows(WindowIDs.NewAccount, WindowIDs.StartWindow);
    }

    public void BackToLogin() {
        ToggleWindows(WindowIDs.NewAccount, WindowIDs.Login);
    }

    public void CreateAccount() {
        // TODO
    }
}
