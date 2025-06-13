using Photon.Realtime;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class PlayerListDisplay : MonoBehaviour
{
    public TextMeshProUGUI playerListText;

    public void UpdatePlayerList(Dictionary<string, PlayerInfo> players)
    {
        foreach (var player in players.Values)
        {
            playerListText.text += "- " + player.Nickname + "\n";
        }
    }
}
