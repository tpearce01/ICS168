using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : GenericWindow {

    [SerializeField] private Text _p1text;
    [SerializeField] private Text _p2text;
    [SerializeField] private Text _p3text;
    [SerializeField] private Text _p4text;

    /// <summary>
    /// Sets the player name text in the player info display
    /// </summary>
    /// <param name="playerName">The name to be assigned to the player.</param>
    /// <param name="playerNumber">The number of the player that is to be set.</param>
    public void setPlayerText(string playerName, int playerNumber)
    {
        if (playerNumber == 1) _p1text.text = playerName;
        else if (playerNumber == 2) _p2text.text = playerName;
        else if (playerNumber == 3) _p3text.text = playerName;
        else if (playerNumber == 4) _p4text.text = playerName;
    }

    /// <summary>
    /// Allows you to enable or disable a player's info
    /// </summary>
    /// <param name="playerNumber">The number of the player info you want to enable or disable. </param>
    /// <param name="toggle">Set to true to enable, set to false to disable.</param>
    public void togglePlayerInfo(int playerNumber, bool toggle)
    {
        if (playerNumber == 1) _p1text.enabled = toggle;
        else if (playerNumber == 2) _p2text.enabled = toggle;
        else if (playerNumber == 3) _p3text.enabled = toggle;
        else if (playerNumber == 4) _p4text.enabled = toggle;
    }

}
