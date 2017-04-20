using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataLoader : MonoBehaviour {
    public string[] users;

    IEnumerator Start() {
        WWW userData = new WWW("http://localhost/teamnewport/UserData.php");
        yield return userData; //wait for userData to finish downloading
        string userDataString = userData.text; //store text on website in string

        //Debug.Log(userDataString);
        users = userDataString.Split(';');
        Debug.Log(GetDataValue(users[0], "username:"));
    }

    string GetDataValue(string data, string index){
        string value = data.Substring(data.IndexOf(index)+index.Length); //get everything after index
        if (value.Contains("|")){
            value = value.Remove(value.IndexOf("|")); // remove everything after |
        }
        return value;
    }
}
