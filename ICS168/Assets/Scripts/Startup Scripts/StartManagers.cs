using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartManagers : MonoBehaviour {

    [SerializeField]
    private bool _startGameManager = false;
    [SerializeField]
    private bool _startWindowManager = true;
    [SerializeField]
    private bool _startInputManager = false;
    [SerializeField]
    private bool _startMapGenerator = false;

    // We can use this script to start up any and all singleton managers here.
    private void Start() {

        if (_startGameManager) { GameManager GM = GameManager.Instance; }
        if (_startWindowManager) { WindowManager WM = WindowManager.Instance; }
        if (_startInputManager) { InputManager IM = InputManager.Instance; }
        if (_startMapGenerator) { MapGenerator MG = MapGenerator.Instance; }
    }
}
