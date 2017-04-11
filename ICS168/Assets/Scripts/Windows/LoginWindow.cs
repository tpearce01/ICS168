using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginWindow : GenericWindow {

    public void NewAccount() {
        ToggleWindows(WindowIDs.Login, WindowIDs.NewAccount);
    }

    public void Login(string NewGameScene) {

        ToggleWindows(WindowIDs.Login, WindowIDs.Game);
        SceneManager.LoadScene(NewGameScene);

    }
    

    public void BackToMain() {
        ToggleWindows(WindowIDs.Login, WindowIDs.StartWindow);
    }
}
