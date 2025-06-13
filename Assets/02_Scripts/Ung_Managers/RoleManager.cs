using System.Collections.Generic;
using UnityEngine;

public enum Role { Crewmate, Impostor }

public class RoleManager : MonoBehaviour
{
    public static RoleManager Instance { get; private set; }

    private Dictionary<string, Role> playerRoles = new Dictionary<string, Role>();
    [SerializeField] private PlayerManager _playerManager;

    void Awake()
    {
        Instance = this;
    }
    public void AssignRoles(int impostorCount)
    {
        // 예시: 임포스터 1명, 나머지 크루메이트
        List<string> players = _playerManager.GetAllPlayerIDs(); // 플레이어 ID 목록 가져오기
        // int impostorCount = 1; // 임포스터 수 (예시로 1명), 나중에 UI에서 설정 가능
        Debug.Log($"Assigning roles to {players.Count} players with {impostorCount} impostors.");

        List<string> shuffled = new List<string>(players);
        Shuffle(shuffled);
        Debug.Log("Shuffled player IDs: " + string.Join(", ", shuffled));

        for (int i = 0; i < players.Count; i++)
        {
            Role role = (i < impostorCount) ? Role.Impostor : Role.Crewmate;
            playerRoles[shuffled[i]] = role;
            Debug.Log($"{shuffled[i]} assigned to {role}");

            if (role == Role.Impostor)
            {
                PlayerInfo info = _playerManager.GetPlayerByID(shuffled[i]);
                if (info != null && info.GetComponent<ImposterController>() == null)
                {
                    info.gameObject.AddComponent<ImposterController>();
                    Debug.Log($"{shuffled[i]}에게 ImposterController 스크립트를 추가했습니다.");
                }
            }
        }
        foreach (string playerID in players)
    {
        PlayerInfo info = _playerManager.GetPlayerByID(playerID);
        if (info != null)
        {
            info.Role = GetRole(playerID);
            Debug.Log($"{info.Nickname} 역할: {info.Role}");
        }
    }
    }

    public Role GetRole(string playerID)
    {
        return playerRoles.ContainsKey(playerID) ? playerRoles[playerID] : Role.Crewmate;
    }

    private void Shuffle(List<string> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rnd = Random.Range(i, list.Count);
            (list[i], list[rnd]) = (list[rnd], list[i]);
        }
    }
}