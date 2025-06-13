using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private Dictionary<string, PlayerInfo> playerDict = new Dictionary<string, PlayerInfo>();

    /// <summary>
    /// 플레이어 등록
    /// </summary>
    public void RegisterPlayer(PlayerInfo player)
    {
        if (!playerDict.ContainsKey(player.PlayerID))
        {
            playerDict.Add(player.PlayerID, player);

            // 첫 번째 등록된 플레이어를 로컬 플레이어로 설정
            if (playerDict.Count == 1)
            {
                player.IsLocalPlayer = true;
                Debug.Log($"[RegisterPlayer] {player.Nickname}을 로컬 플레이어로 지정");
            }
        }
    }

    /// <summary>
    /// ID로 플레이어 조회
    /// </summary>
    public PlayerInfo GetPlayerByID(string id)
    {
        return playerDict.TryGetValue(id, out var player) ? player : null;
    }

    /// <summary>
    /// 로컬 플레이어 반환
    /// </summary>
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

    /// <summary>
    /// 생존한 플레이어 목록
    /// </summary>
    public List<PlayerInfo> GetAlivePlayers()
    {
        return playerDict.Values.Where(p => !p.IsDead).ToList();
    }

    /// <summary>
    /// 모든 플레이어 ID 목록
    /// </summary>
    public List<string> GetAllPlayerIDs()
    {
        return playerDict.Keys.ToList();
    }

    /// <summary>
    /// 플레이어 사망 처리
    /// </summary>
    public void KillPlayer(string playerID)
    {
        if (playerDict.TryGetValue(playerID, out var player))
        {
            player.IsDead = true;

            // 사망 처리 메서드 호출
            // player.GameObject.GetComponent<PlayerInfo>().Die();

            // 로그 출력
            Debug.Log($"{player.Nickname} has been killed.");
        }
    }
}
