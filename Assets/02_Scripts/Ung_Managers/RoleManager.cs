using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public enum Role { UnManaged = 0, Crewmate = 1, Impostor = 2 }

public class RoleManager : MonoBehaviour
{
    public static RoleManager Instance { get; private set; }

    private Dictionary<int, Role> playerRoles = new Dictionary<int, Role>(); // ActorNumber 기반

    private void Awake()
    {
        Instance = this;
    }

    public void AssignRoles(int impostorCount)
    {
        Player[] players = PhotonNetwork.PlayerList;
        Debug.Log($"Assigning roles to {players.Length} players with {impostorCount} impostors.");

        List<Player> shuffled = new List<Player>(players);
        Shuffle(shuffled);
        Debug.Log("Shuffled ActorNumbers: " + string.Join(", ", shuffled.ConvertAll(p => p.ActorNumber.ToString())));

        for (int i = 0; i < shuffled.Count; i++)
        {
            Role role = (i < impostorCount) ? Role.Impostor : Role.Crewmate;
            Player targetPlayer = shuffled[i];
            int actorNumber = targetPlayer.ActorNumber;

            // 내부 Dictionary에도 저장
            playerRoles[actorNumber] = role;

            // CustomProperties 설정
            Hashtable roleProp = new Hashtable
            {
                { "Role", (int)role }
            };
            if (targetPlayer.IsLocal) // 본인만 Set 가능
            {
                PhotonNetwork.LocalPlayer.SetCustomProperties(roleProp);
            }

            Debug.Log($"Player {actorNumber} assigned to {role}");
        }
    }

    public Role GetRole(Player player)
    {
        if (player.CustomProperties.TryGetValue("Role", out object roleObj))
        {
            return (Role)(int)roleObj;
        }
        return Role.UnManaged;
    }

    public Role GetRoleByActorNumber(int actorNumber)
    {
        Player target = FindPlayerByActorNumber(actorNumber);
        return target != null ? GetRole(target) : Role.UnManaged;
    }

    private Player FindPlayerByActorNumber(int actorNumber)
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.ActorNumber == actorNumber)
                return player;
        }
        return null;
    }

    private void Shuffle(List<Player> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rnd = Random.Range(i, list.Count);
            (list[i], list[rnd]) = (list[rnd], list[i]);
        }
    }
}
