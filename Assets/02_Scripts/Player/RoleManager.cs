using System.Collections.Generic;
using _02_Scripts.Alert;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public enum Role { UnManaged = 0, Crewmate = 1, Impostor = 2 }

public class RoleManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private PlayerManager playerManager;
    public static RoleManager Instance { get; private set; }
    private HashSet<int> playersWithRole = new HashSet<int>();

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
            AlertUIManager.Instance.OnAlert("당신의 역할은 살인마입니다. 모든 시민을 죽이고 탈출하세요 ");    
        }
        else
        {
            AlertUIManager.Instance.OnAlert("당신의 역할은 시민입니다. 모든 살인마를 찾고 평화를 찾으세요. ");
        }
        

        if (role == Role.Impostor)
        {
            int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
            GameObject myGO = playerManager.GetPlayersGO(actorNumber);
            if (myGO != null && myGO.GetComponent<ImposterController>() == null)
                myGO.AddComponent<ImposterController>();
        }

        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { PlayerPropKey.Role, roleInt } });
    }
    
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey(PlayerPropKey.Role))
        {
            int actorNumber = targetPlayer.ActorNumber;
            Debug.Log($"[RoleManager] RoleSet: {targetPlayer.NickName} / Actor#{actorNumber}");

            if (PhotonNetwork.IsMasterClient)
            {
                if (!playersWithRole.Contains(actorNumber))
                    playersWithRole.Add(actorNumber);

                Debug.Log($"[RoleManager] 현재 역할 설정된 인원 수: {playersWithRole.Count} / 전체: {PhotonNetwork.PlayerList.Length}");
            }
            if (playersWithRole.Count == PhotonNetwork.PlayerList.Length)
            {
                Debug.Log("[RoleManager] 모든 인원이 역할을 수락했으므로 PlayingStart로 전환");
                GameManager.Instance.ChangeState(GameState.PlayingStart);
            }
            if (targetPlayer.IsLocal)
            {
                PlayerUIManager.Instance.Init(); // 로컬 UI 초기화
            }
        }
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