using TMPro;
using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

public class PlayerListDisplay : MonoBehaviour
{
    public TextMeshProUGUI playerListText;

    public void UpdatePlayerList(Dictionary<int, GameObject> players)
    {
        foreach (var player in players.Values)
        {
            PhotonView view = player.GetComponent<PhotonView>();
            playerListText.text += "- " + view.Owner.NickName + "\n";
        }
    }
}
