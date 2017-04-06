using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartManagers : MonoBehaviour {

    // We can use this script to start up any and all singleton managers here.
    private void Start() {
        GameObject WM = WindowManager.Instance;
    }
}
