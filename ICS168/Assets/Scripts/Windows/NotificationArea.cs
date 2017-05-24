using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class NotificationArea : GenericWindow {

    [SerializeField] private Text _incomingMessage;
    public float timeToDisplay;

    public void playerEntered(string playerName)
    {
        _incomingMessage.text = playerName + " joined the game";
    }

    public void playerLeft(string playerName)
    {
        _incomingMessage.text = playerName + " left the game";
    }

}
