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
            PhotonView targetView = targetGO.GetComponent<PhotonView>();
            int actorNumber = targetView.OwnerActorNr;

            Debug.Log($"{localPlayer.NickName}가 Actor#{actorNumber}를 공격합니다.");

            if (!targetView.IsMine)
            {
                Debug.Log("[TryKill] 대상은 타인입니다. RPC 전송 중...");
                // 타인 소유의 뷰에 RPC 호출
                if (targetView.Owner != null)
                {
                    if (!targetView.IsMine)
                    {
                        Debug.Log($"[TryKill] 대상은 타인입니다. {targetView.Owner.NickName}에게 RPC 전송 시도");
                        targetView.RPC("RPC_ReceiveDamage", targetView.Owner, 50f, localPlayer.ActorNumber);
                    }
                    else
                    {
                        Debug.Log("[TryKill] 자기 자신을 공격합니다.");
                        PlayerDamageReceiver damageReceiver = targetGO.GetComponent<PlayerDamageReceiver>();
                        if (damageReceiver != null)
                        {
                            damageReceiver.RPC_ReceiveDamage(50f, localPlayer.ActorNumber);
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("[TryKill] targetView.Owner가 null입니다. ViewID: " + targetView.ViewID);
                }
            }
            else
            {
                // 본인 테스트용: 자기 자신에게 데미지 처리
                PlayerDamageReceiver damageReceiver = targetGO.GetComponent<PlayerDamageReceiver>();
                if (damageReceiver != null)
                {
                    damageReceiver.RPC_ReceiveDamage(50f, localPlayer.ActorNumber);
                }
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
