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
            // 플레이어 등록
            playerDict.Add(player.PlayerID, player);

            // 첫 번째 등록된 플레이어만 로컬 플레이어로 설정
            if (playerDict.Count == 0)
            {
                player.IsLocalPlayer = true;
                Debug.Log($"[RegisterPlayer] {player.Nickname}을 로컬 플레이어로 지정");
            }
        }
    }

    public PlayerInfo GetPlayerByID(string id)
    {
        return playerDict.TryGetValue(id, out var player) ? player : null;
    }

    public PlayerInfo GetLocalPlayer()
    {
        Debug.Log($"[GetLocalPlayer] 현재 플레이어 수: {playerDict.Count}");
        foreach (var player in playerDict.Values)
        {
            Debug.Log($"[GetLocalPlayer] 체크중: {player.Nickname}, IsLocalPlayer={player.IsLocalPlayer}");
            if (player.IsLocalPlayer)
            {
                Debug.Log($"[PlayerManager] 로컬 플레이어를 찾았습니다! : {player.Nickname}");
                return player;
            }
        }
        return null;
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
            player.GameObject.GetComponent<PlayerInfo>().Die();
            // 사망 애니메이션, 시체 생성, UI 갱신 등 추가
            Debug.Log($"{player.Nickname} has been killed.");
        }
    }
}
