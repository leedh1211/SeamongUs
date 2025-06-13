using System.Collections.Generic;
using UnityEngine;

public enum Role { Crewmate, Impostor }

public class RoleManager : MonoBehaviour
{
    public static RoleManager Instance { get; private set; }

    private Dictionary<string, Role> playerRoles = new Dictionary<string, Role>();

    void Awake()
    {
        Instance = this;
    }

    /*
    public void AssignRoles()
    {
        // 예시: 임포스터 1명, 나머지 크루메이트
        List<string> players = PlayerManager.Instance.GetAllPlayerIDs(); // 플레이어 ID 목록 가져오기
        int impostorCount = 1; // 임포스터 수 (예시로 1명), 나중에 UI에서 설정 가능

        List<string> shuffled = new List<string>(players);
        Shuffle(shuffled);

        for (int i = 0; i < players.Count; i++)
        {
            Role role = (i < impostorCount) ? Role.Impostor : Role.Crewmate;
            playerRoles[shuffled[i]] = role;
            Debug.Log($"{shuffled[i]} assigned to {role}");
        }
    }
    */

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