using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;
using UnityEngine;

public class ReportManager : MonoBehaviour
{
    public static ReportManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    [ContextMenu("리포트 바디")]
    public void ReportBody(int deadPlayerActNum = 1)
    {
        int playerID = PhotonNetwork.LocalPlayer.ActorNumber;
        List<DeadBody> deadBodyList = DeadBodyManager.Instance.GetDeadBodies();
        DeadBody deadBody = DeadBodyManager.Instance.GetDeadBody(deadPlayerActNum);
        int findPeopleActorNum = deadBody.PlayerActorNumber;
        
        object[] eventData = new object[] {findPeopleActorNum};
        PhotonNetwork.RaiseEvent(
            EventCodes.PlayerReport,
            eventData,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            SendOptions.SendReliable
        );
    }
}
