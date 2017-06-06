using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginWindow : GenericWindow {

    //private string _LoginURL = "http://localhost/teamnewport/LoginManager.php";
    private LoginInfo _loginInfo = new LoginInfo();

    public void OnEnable() {
        GameObject.Find("LoginUsernameError").GetComponent<Text>().text = "";
    }

    public void NewAccount() {
        ToggleWindows(WindowIDs.Login, WindowIDs.NewAccount);
    }

    public void Login(string NewGameScene) {
        //Verify login
        Text[] input = gameObject.GetComponentsInChildren<Text>();
        string username = "";
        string password = "";
        int count = 0;

        for (int i = 0; i < input.Length; ++i)
        {
            if (input[i].name == "UserInput")
            {
                switch (count)
                {
                    case 0:
                        username = input[i].text;
                        break;
                    case 1:
                        password = input[i].text;
                        break;
                }
                count++;
            }
        }
        Debug.Log(username + " " + password);
        _loginInfo.username = username;
        _loginInfo.password = password;

        ClientConnection.Instance.SendMessage(_loginInfo);
    }

    public void BackToMain() {
        ToggleWindows(WindowIDs.Login, WindowIDs.StartWindow);
    }

    public void goToLobby() {
        ToggleWindows(WindowIDs.Login, WindowIDs.Game);
    }
}
