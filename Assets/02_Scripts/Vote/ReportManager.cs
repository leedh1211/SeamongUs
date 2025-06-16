using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class ReportManager : MonoBehaviour
{
    public static ReportManager Instance { get; private set; }

    public int LastReporter { get; private set; }

    void Awake()
    {
        Instance = this;
    }
    
    public void ReportBody(int deadPlayerActNum)
    {
        int reporterActorNum = PhotonNetwork.LocalPlayer.ActorNumber;
        // LastReporter 에 기록
        LastReporter = reporterActorNum;

        // DeadBody 찾아서 신고 이벤트 전파
        DeadBody deadBody = DeadBodyManager.Instance.GetDeadBody(deadPlayerActNum);
        int findPeopleActorNum = deadBody.PlayerActorNumber;
        object[] eventData = new object[] { findPeopleActorNum };
        PhotonNetwork.RaiseEvent(
            EventCodes.PlayerReport,
            eventData,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            SendOptions.SendReliable
        );
    }

    public void OnEvent(EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case EventCodes.PlayerReport:
                break;
        }
    }
}
