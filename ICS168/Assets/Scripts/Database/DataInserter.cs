using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataInserter : MonoBehaviour {
    private string _CreateAccountURL = "http://localhost/teamnewport/CreateAccount.php";

    /// <summary>
    /// Takes in a username and password and sends it to the php script
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    IEnumerator CreateUser(string username, string password){
        //WWWForm allows to communicate with php files
        WWWForm form = new WWWForm();
        form.AddField("usernamePost", username);
        form.AddField("passwordPost", password);

        WWW www = new WWW(_CreateAccountURL, form);
        yield return www;
    }
}
