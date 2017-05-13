using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientLobbyWindow : GenericWindow {

	public void OnCanel() {
        ClientConnection.Instance.LeaveLobby();
        ToggleWindows(WindowIDs.ClientLobby, WindowIDs.Login);
    }
}
