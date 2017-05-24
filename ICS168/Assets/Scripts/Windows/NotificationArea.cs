using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class NotificationArea : GenericWindow {

    [SerializeField] private Text _incomingMessage;

    [SerializeField] private float _maxTimeToDisplay;
    private float timer;
    private bool isMessageUp = false;

    public float timeToDisplay;
    private void Start()
    {
        timer = _maxTimeToDisplay;
    }
    public void playerEntered(string playerName)
    {
        timer = _maxTimeToDisplay;
        _incomingMessage.text = playerName + " joined the game";
        this.Display(true);
        isMessageUp = true;
    }

    public void playerLeft(string playerName)
    {
        timer = _maxTimeToDisplay;
        _incomingMessage.text = playerName + " left the game";
        this.Display(true);
        isMessageUp = true;
    }

    private void Update()
    {
        if (isMessageUp)
        {
            timer -= Time.deltaTime;
            if (timer <= 0.0f)
            {
                this.Display(false);
                isMessageUp = false;
                timer = _maxTimeToDisplay;
            }
        }
    }

}
