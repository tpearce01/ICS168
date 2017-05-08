using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class NewAccountWindow : GenericWindow {
    //private string _CreateAccountURL = "http://localhost/teamnewport/CreateAccount.php";
    private LoginInfo _createAccount = new LoginInfo();

    public void BackToMain() {
        ToggleWindows(WindowIDs.NewAccount, WindowIDs.StartWindow);
    }

    public void BackToLogin() {
        ToggleWindows(WindowIDs.NewAccount, WindowIDs.Login);
    }

    public void CreateAccount()
    {
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
                        if(password == "")
                        {
                            GameObject.Find("PasswordError").GetComponent<Text>().text = "Password is empty, please enter a password.";
                            return;
                        }
                        break;
                    case 2:
                        if (password != input[i].text)
                        {
                            Debug.Log("Password Mismatch");
                            GameObject.Find("PasswordError").GetComponent<Text>().text = "Passwords don't match.";
                            return;
                        }else
                        {
                            GameObject.Find("PasswordError").GetComponent<Text>().text = "";
                        }
                        break;
                }
                count++;
            }
        }
        //Debug.Log(username + " " + password);
        _createAccount.username = username;
        _createAccount.password = password;

        ClientConnection.Instance.SendMessageAccount(_createAccount);
        //StartCoroutine(CreateUser(username, password));
        
    }

    //IEnumerator CreateUser(string username, string password)
    //{
    //    Debug.Log(username + " " + password);
    //    WWWForm form = new WWWForm();
    //    form.AddField("usernamePost", username);
    //    form.AddField("passwordPost", password);

    //    WWW verify = new WWW(_CreateAccountURL, form);
    //    yield return verify;;

    //    if (verify.text == "username exists")
    //    {
    //        GameObject.Find("UsernameError").GetComponent<Text>().text = "Username already exists. Choose a different username.";
    //    }
    //    else if (verify.text == "account created")
    //    {
    //        Debug.Log("account was created");
    //        ToggleWindows(WindowIDs.NewAccount, WindowIDs.NewAccountSuccess);
    //    }
    //}
}
