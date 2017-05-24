using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnlineStatusWindow : GenericWindow {

    [SerializeField] private float _defaultTimer = 0.0f;
    [SerializeField] private Text _onlineStatus;
    [SerializeField] private Text _message;

    private float _timer = 0.0f;

    private void OnEnable() {
        _timer = _defaultTimer;
    }

    public void UpdateOnlineStatus(bool status) {
        _onlineStatus.text = status ? "ONLINE STATUS: ONLINE" : "ONLINE STATUS: OFFLINE";

        if (!status) { _timer = _defaultTimer; }
        if (status) { _message.gameObject.SetActive(false); }
    }

    private void Update() {
        if (!ClientConnection.Instance.Connected) {

            if (_timer > 0.0f) {
                _timer -= Time.deltaTime;
                if (!_message.gameObject.activeInHierarchy) { _message.gameObject.SetActive(true); }
            }
            else {
                ClientConnection.Instance.Connect();
                _timer = _defaultTimer;
            }
        }
    }
}
