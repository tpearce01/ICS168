using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginWindow : GenericWindow {

    public delegate void LoginWindowEvent();
    public static event LoginWindowEvent OnLogin;

    public void NewAccount() {
        ToggleWindows(WindowIDs.Login, WindowIDs.NewAccount);
    }

    public void Login(string NewGameScene) {

        ToggleWindows(WindowIDs.Login, WindowIDs.Game);
        SceneManager.LoadScene(NewGameScene);

        //if (OnLogin != null) { OnLogin();
    }
    

    public void BackToMain() {
        ToggleWindows(WindowIDs.Login, WindowIDs.StartWindow);
    }
}
