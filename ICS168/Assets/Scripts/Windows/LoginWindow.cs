using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginWindow : GenericWindow {

    private string _LoginURL = "http://localhost/teamnewport/LoginManager.php";

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
        StartCoroutine(verifyLogin(username, password, NewGameScene));
    }
    
    IEnumerator verifyLogin(string username, string password, string NewGameScene)
    {
        WWWForm form = new WWWForm();
        form.AddField("usernamePost", username);
        form.AddField("passwordPost", password);

        WWW verify = new WWW(_LoginURL, form);
        yield return verify;
        Debug.Log(verify.text);

        if(verify.text == "valid")
        {
            ToggleWindows(WindowIDs.Login, WindowIDs.Game);
            SceneManager.LoadScene(NewGameScene);
        }
    }

    public void BackToMain() {
        ToggleWindows(WindowIDs.Login, WindowIDs.StartWindow);
    }
}
