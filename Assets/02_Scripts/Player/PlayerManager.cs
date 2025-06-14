using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerManager : MonoBehaviour
{
    private Dictionary<string, PlayerInfo> playerDict = new Dictionary<string, PlayerInfo>();
    
    public void RegisterPlayer(PlayerInfo player)
    {
        if (!playerDict.ContainsKey(player.PlayerID))
        {
            playerDict.Add(player.PlayerID, player);
        }
    }

    public Dictionary<string, PlayerInfo> GetAllPlayers()
    {
        return playerDict;
    }

    public PlayerInfo GetPlayerByID(string id)
    {
        return playerDict.TryGetValue(id, out var player) ? player : null;
    }

    public PlayerInfo GetLocalPlayer()
    {
        return playerDict.Values.FirstOrDefault(p => p.IsLocalPlayer);
    }

    public List<PlayerInfo> GetAlivePlayers()
    {
        return playerDict.Values.Where(p => !p.IsDead).ToList();
    }

    public List<string> GetAllPlayerIDs()
    {
        return playerDict.Keys.ToList();
    }

    public Dictionary<string, PlayerInfo> GetAllPlayersDict()
    {
        return playerDict;
    }

    public void KillPlayer(string playerID)
    {
        if (playerDict.TryGetValue(playerID, out var player))
        {
            player.IsDead = true;
            // 사망 애니메이션, 시체 생성, UI 갱신 등 추가
            Debug.Log($"{player.Nickname} has been killed.");
        }
    }
}
