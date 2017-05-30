using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientLobbyWindow : GenericWindow {

    private Button _cancelButton;

    private void OnEnable() {
        _cancelButton = GetComponentInChildren<Button>();
        _cancelButton.interactable = true;
    }

    public void OnCanel() {
        ClientConnection.Instance.LeaveLobby();
        ToggleWindows(WindowIDs.ClientLobby, WindowIDs.GameSelect);
    }

    public void CannotDisconnect() {
        _cancelButton.interactable = false;
    }
}
