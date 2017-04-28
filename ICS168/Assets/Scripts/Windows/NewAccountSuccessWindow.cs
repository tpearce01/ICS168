using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewAccountSuccessWindow : GenericWindow {

    public void BackToMain()
    {
        ToggleWindows(WindowIDs.NewAccountSuccess, WindowIDs.StartWindow);
    }

    public void BackToLogin()
    {
        ToggleWindows(WindowIDs.NewAccountSuccess, WindowIDs.Login);
    }
}
