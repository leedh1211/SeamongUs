using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class PlayerManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    private Dictionary<int, GameObject> playerGODict = new Dictionary<int, GameObject>();

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
    
    public void RegisterAllPlayers()
    {
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");

        foreach (var player in PhotonNetwork.PlayerList)
        {
            int actorNumber = player.ActorNumber;

            foreach (var obj in allPlayers)
            {
                PhotonView view = obj.GetComponent<PhotonView>();
                if (view != null && view.Owner != null && view.Owner.ActorNumber == actorNumber)
                {
                    if (!playerGODict.ContainsKey(actorNumber))
                    {
                        playerGODict.Add(actorNumber, obj);
                        Debug.Log($"[PlayerManager] 등록됨: ActorNumber={actorNumber}, GameObject={obj.name}");
                    }
                    break; // 해당 플레이어의 오브젝트를 찾았으면 루프 중단
                }
            }
        }
    }

    public void RegisterPlayers(int viewID, int actorNumber)
    {
        PhotonView view = PhotonView.Find(viewID);
        GameObject playerGO = view.gameObject;
        playerGODict[actorNumber] = playerGO;
    }

    public Dictionary<int, GameObject> GetAllPlayers()
    {
        return playerGODict;
    }

    public void KillPlayer(int actorNumber)
    {
        if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.Players.TryGetValue(actorNumber, out Player player))
        {
            Debug.Log($"플레이어 닉네임: {player.NickName}");
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        if (!IsPlayerEvent(photonEvent)) return;

        object[] data = photonEvent.CustomData as object[];
        if (data == null || data.Length < 1) return;

        int actorNumber = (int)data[0];
        PlayerController controller;

        switch (photonEvent.Code)
        {
            case EventCodes.PlayerSpawn:
                if (actorNumber == PhotonNetwork.LocalPlayer.ActorNumber) // 이벤트 수신자가 본인일때 (방에 입장한 새로운 인원)
                {
                    RegisterAllPlayers();    
                }
                else
                {
                    RegisterPlayers((int)data[1], actorNumber);
                }
                
                break;
            case EventCodes.PlayerJump:
                controller = FindPlayerController(actorNumber);
                controller.StartCoroutine(controller.JumpCoroutine());
                break;
            case EventCodes.PlayerKill:
                 controller = FindPlayerController(actorNumber);
                controller.OnKill?.Invoke();
                break;
        }
    }

    public bool IsPlayerEvent(EventData photonEvent)
    {
        return photonEvent.Code == EventCodes.PlayerSpawn || 
               photonEvent.Code == EventCodes.PlayerJump ||
               photonEvent.Code == EventCodes.PlayerKill ||
               photonEvent.Code == EventCodes.PlayerAttacked ||
               photonEvent.Code == EventCodes.PlayerDied;
    }

    public PlayerController FindPlayerController(int actorNumber)
    {
        return playerGODict.TryGetValue(actorNumber, out var go) ? go.GetComponent<PlayerController>() : null;
    }
}
