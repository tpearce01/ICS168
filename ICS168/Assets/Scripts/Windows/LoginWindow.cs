using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginWindow : GenericWindow {

    //private string _LoginURL = "http://localhost/teamnewport/LoginManager.php";
    private LoginInfo _loginInfo = new LoginInfo();

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
        //StartCoroutine(verifyLogin(username, password, NewGameScene));
    }
    

    //IEnumerator verifyLogin(string username, string password, string NewGameScene)
    //{
    //    WWWForm form = new WWWForm();
    //    form.AddField("usernamePost", username);
    //    form.AddField("passwordPost", password);

    //    WWW verify = new WWW(_LoginURL, form);
    //    yield return verify;

    //    if(verify.text == "valid")
    //    {
    //        //GameObject.Find("GameManager").GetComponent<ServerConnection>();
    //        ToggleWindows(WindowIDs.Login, WindowIDs.Game);
    //        SceneManager.LoadScene(NewGameScene);
    //    }else if(verify.text == "invalid")
    //    {
    //        GameObject.Find("LoginUsernameError").GetComponent<Text>().text = "Invalid username or password.";
    //    }else if(verify.text == "user not found")
    //    {
    //        GameObject.Find("LoginUsernameError").GetComponent<Text>().text = "Username does not exist in the database.";
    //    }
    //}

    public void BackToMain() {
        ToggleWindows(WindowIDs.Login, WindowIDs.StartWindow);
    }

    public void goToLobby() {
        ToggleWindows(WindowIDs.Login, WindowIDs.Game);
    }
}
