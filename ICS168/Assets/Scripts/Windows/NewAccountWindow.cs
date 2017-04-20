using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class NewAccountWindow : GenericWindow {

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
                        break;
                    case 2:
                        if (password != input[i].text)
                        {
                            Debug.Log("Password Mismatch");
                            return;
                        }
                        break;
                }
                count++;
            }
        }
        Debug.Log(username + " " + password);
        gameObject.GetComponent<DataInserter>().CreateUser(username, password);
    }
}
