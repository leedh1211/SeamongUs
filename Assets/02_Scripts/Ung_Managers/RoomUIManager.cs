using System.Collections.Generic;
using UnityEngine;

public class RoomUIManager : MonoBehaviour
{
    [SerializeField] private PlayerListDisplay playerListDisplay;

    private void Start()
    {
        UpdatePlayerListFromManager();
    }

    public void UpdatePlayerListFromManager()
    {
        var playerManager = FindObjectOfType<PlayerManager>();
        if (playerManager != null)
        {
            Dictionary<string, PlayerInfo> players = playerManager.GetAllPlayers();
            playerListDisplay.UpdatePlayerList(players);
        }
    }
}
