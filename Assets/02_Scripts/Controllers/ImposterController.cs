using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class ImposterController : MonoBehaviour
{
    private Player localPlayer;
    private PhotonView view;

    [SerializeField] private float killRange = 2.0f;

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

    // void Update()
    // {
    //     if (!view.IsMine) return;
    //
    //     if (Input.GetKeyDown(KeyCode.F))
    //     {
    //         TryKill();
    //     }
    //
    //     if (Input.GetKeyDown(KeyCode.R))
    //     {
    //         TryReportBody();
    //     }
    // }

    public void TryKill()
    {
        GameObject targetGO = FindKillablePlayer();
        if (targetGO != null)
        {
            int actorNumber = targetGO.GetComponent<PhotonView>().OwnerActorNr;
            Player targetPlayer = FindPlayerByActorNumber(actorNumber);
            if (targetPlayer != null)
            {
                Debug.Log($"{localPlayer.NickName}가 {targetPlayer.NickName}를 살해했습니다.");
                RaiseKillEvent(actorNumber); // 서버 전체에 킬 이벤트 전송
            }
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

    void RaiseKillEvent(int targetActorNumber)
    {
        object[] content = new object[] { localPlayer.ActorNumber };
        PhotonNetwork.RaiseEvent(
            EventCodes.PlayerKill,
            content,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            ExitGames.Client.Photon.SendOptions.SendReliable
        );
        PhotonNetwork.RaiseEvent(
            EventCodes.PlayerDied,
            new object[] { targetActorNumber },
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            ExitGames.Client.Photon.SendOptions.SendReliable
        );
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
