using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Login : MonoBehaviour {

    public string inputUsername;
    public string inputPassword;

    private string LoginURL = "http://localhost/teamnewport/LoginManager.php";
    
	void Update () {
        if (Input.GetKeyDown(KeyCode.L)){
            StartCoroutine(userLogin(inputUsername, inputPassword));
        }
    }

    IEnumerator userLogin(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("usernamePost", username);
        form.AddField("passwordPost", password);

        WWW verify = new WWW(LoginURL, form);
        yield return verify;
        Debug.Log(verify.text);
    }
}
