using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectToServer : MonoBehaviour {

	public void Update() {

        if (Input.anyKeyDown) {
            TransportManager.Instance.Connect();
        }
    }
}
