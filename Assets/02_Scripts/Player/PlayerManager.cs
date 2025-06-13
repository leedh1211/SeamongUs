using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    private Dictionary<string, PlayerInfo> playerDict = new Dictionary<string, PlayerInfo>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
          //  DontDestroyOnLoad(gameObject); // 씬 전환 시에도 유지
        }
        else
        {
            Destroy(gameObject); // 중복 인스턴스 방지
        }
    }

    public void RegisterPlayer(PlayerInfo player)
    {
        if (!playerDict.ContainsKey(player.PlayerID))
        {
            playerDict.Add(player.PlayerID, player);
        }
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
