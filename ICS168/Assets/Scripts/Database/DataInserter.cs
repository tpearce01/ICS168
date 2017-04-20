using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataInserter : MonoBehaviour {
    public string inputUsername;
    public string inputPassword;

    private string CreateAccountURL = "http://localhost/teamnewport/CreateAccount.php";
    
    void Update () {
        if (Input.GetKeyDown(KeyCode.Space)) CreateUser(inputUsername, inputPassword);
	}

    /// <summary>
    /// Takes in a username and password and sends it to the php script
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    public void CreateUser(string username, string password){
        //WWWForm allows to communicate with php files
        WWWForm form = new WWWForm();
        form.AddField("usernamePost", username);
        form.AddField("passwordPost", password);

        WWW www = new WWW(CreateAccountURL, form);
    }
}
