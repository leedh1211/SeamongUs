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
    
    public void ReportBody(int deadPlayerActNum)
    {
        int playerID = PhotonNetwork.LocalPlayer.ActorNumber;
        List<DeadBody> deadBodyList = DeadBodyManager.Instance.GetDeadBodies();
        DeadBody deadBody = DeadBodyManager.Instance.GetDeadBody(deadPlayerActNum);
        int findPeopleActorNum = deadBody.PlayerActorNumber; // 시체의 주인 PlayerActorNumber
        object[] eventData = new object[] {findPeopleActorNum};
        PhotonNetwork.RaiseEvent(
            EventCodes.PlayerReport,
            eventData,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            SendOptions.SendReliable
        );
    }
}
