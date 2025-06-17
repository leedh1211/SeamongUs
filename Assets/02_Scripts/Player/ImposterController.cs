using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using ExitGames.Client.Photon;

public class ImposterController : MonoBehaviour
{
    private Player localPlayer;
    private PhotonView view;

    [SerializeField] private float killRange = 2.0f;
    [SerializeField] private float AttackDamage = 50f;
    
    void Start()
    {
        view = GetComponent<PhotonView>();
        if (view.IsMine)
        {
            localPlayer = PhotonNetwork.LocalPlayer;
            Debug.Log($"[ImposterController] 로컬 플레이어 등록됨: {localPlayer.NickName}");
        }
        else
        {
            this.enabled = false;
        }
    }
    public void TryKill()
    {
        GameObject targetGO = FindKillablePlayer();
        if (targetGO != null)
        {
            PhotonView targetView = targetGO.GetComponent<PhotonView>();
            int actorNumber = targetView.OwnerActorNr;

            Debug.Log($"{localPlayer.NickName}가 Actor#{actorNumber}를 공격합니다.");

            object[] eventData = new object[] { targetView.Owner.ActorNumber, AttackDamage};
                PhotonNetwork.RaiseEvent(
                    EventCodes.PlayerAttacked,
                    eventData,
                    new RaiseEventOptions { Receivers = ReceiverGroup.All },
                    SendOptions.SendReliable
                );
        }
        else
        {
            Debug.Log("살해 가능한 플레이어가 없습니다.");
        }
    }
    
    // void TryReportBody()
    // {
    //     string deadID = DeadBodyManager.Instance.GetClosestDeadBodyID(transform.position);
    //     if (deadID != null)
    //     {
    //         ReportManager.Instance.ReportBody(localPlayer.ActorNumber.ToString(), deadID);
    //         GameManager.Instance.ChangeState(GameState.Meeting);
    //     }
    // }

    GameObject FindKillablePlayer()
    {
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject playerGO in allPlayers)
        {
            PhotonView pv = playerGO.GetComponent<PhotonView>();
            if (pv == null || pv.OwnerActorNr == localPlayer.ActorNumber)
                continue;

            Player otherPlayer = FindPlayerByActorNumber(pv.OwnerActorNr);
            if (otherPlayer == null || otherPlayer.CustomProperties == null)
                continue;

            // 조건: 크루메이트 + 살아있음
            if ((int)(otherPlayer.CustomProperties[PlayerPropKey.Role] ?? 0) != (int)Role.Crewmate)
                continue;
            if ((bool)(otherPlayer.CustomProperties[PlayerPropKey.IsDead] ?? false))
                continue;

            float dist = Vector3.Distance(transform.position, playerGO.transform.position);
            if (dist <= killRange)
            {
                return playerGO;
            }
        }

        return null;
    }

    Player FindPlayerByActorNumber(int actorNumber)
    {
        foreach (var p in PhotonNetwork.PlayerList)
        {
            if (p.ActorNumber == actorNumber)
                return p;
        }
        return null;
    }
}
