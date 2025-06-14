using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public enum Role { UnManaged = 0, Crewmate = 1, Impostor = 2 }

public class RoleManager : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    public static RoleManager Instance { get; private set; }

    private void Awake() => Instance = this;

    public void AssignRoles(int impostorCount)
    {
        List<Player> shuffled = new List<Player>(PhotonNetwork.PlayerList);
        Shuffle(shuffled);

        for (int i = 0; i < shuffled.Count; i++)
        {
            Role role = (i < impostorCount) ? Role.Impostor : Role.Crewmate;
            Player player = shuffled[i];
            int actorNumber = player.ActorNumber;

            GetComponent<PhotonView>().RPC(nameof(ReceiveRole), player, (int)role);
        }
    }

    [PunRPC]
    public void ReceiveRole(int roleInt)
    {
        Role role = (Role)roleInt;
        Debug.Log($"[RoleManager] My Role: {role}");

        if (role == Role.Impostor)
        {
            int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
            GameObject myGO = playerManager.GetPlayersGO(actorNumber);
            if (myGO != null && myGO.GetComponent<ImposterController>() == null)
                myGO.AddComponent<ImposterController>();
        }

        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "Role", roleInt } });
        GameManager.Instance.ChangeState(GameState.Playing);
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