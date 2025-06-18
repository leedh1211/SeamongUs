using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerDamageReceiver : MonoBehaviourPunCallbacks
{
    private StatManager stat;
    private Player localPlayer;

    new void OnEnable() => PhotonNetwork.AddCallbackTarget(this);
    new void OnDisable() => PhotonNetwork.RemoveCallbackTarget(this);

    void Awake()
    {
        stat = GetComponent<StatManager>();
        localPlayer = PhotonNetwork.LocalPlayer;
        Debug.Log($"[PlayerDamageReceiver] ViewID: {GetComponent<PhotonView>().ViewID}, IsMine: {photonView.IsMine}");
    }
    
    public void ReceiveDamage(float damage, int attackerActorNumber, int receiverActorNumber)
    {
        if (stat == null) return;

        Debug.Log($"피해 {damage} 받음, 공격자 Actor#{attackerActorNumber}");

        stat.Consume(StatType.CurHp, damage);

        if (stat.GetValue(StatType.CurHp) <= 0 && !stat.isDead)
        {
            // stat.Die();

            // 공격자와 피해자 정보를 모두 RaiseKillEvent에 전달
            RaiseKillEvent(attackerActorNumber, photonView.Owner.ActorNumber);
        }
    }

    void RaiseKillEvent(int attackerActorNumber, int targetActorNumber)
    {
        // 공격자 알림 이벤트
        PhotonNetwork.RaiseEvent(
            EventCodes.PlayerKill,
            new object[] { attackerActorNumber },
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            ExitGames.Client.Photon.SendOptions.SendReliable
        );

        // 피해자 알림 이벤트
        PhotonNetwork.RaiseEvent(
            EventCodes.PlayerDied,
            new object[] { targetActorNumber },
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            ExitGames.Client.Photon.SendOptions.SendReliable
        );
    }
}
