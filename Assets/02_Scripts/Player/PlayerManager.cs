using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class PlayerManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    private Dictionary<string, GameObject> playerGODict = new Dictionary<string, GameObject>();

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
                    if (!playerGODict.ContainsKey(actorNumber.ToString()))
                    {
                        playerGODict.Add(actorNumber.ToString(), obj);
                        Debug.Log($"[PlayerManager] 등록됨: ActorNumber={actorNumber}, GameObject={obj.name}");
                    }
                    break; // 해당 플레이어의 오브젝트를 찾았으면 루프 중단
                }
            }
        }
    }

    public void RegisterPlayers(int viewID, string ActorNumber)
    {
        PhotonView view = PhotonView.Find(viewID);
        GameObject playerGO = view.gameObject;
        playerGODict[ActorNumber] = playerGO;
    }

    public Dictionary<string, GameObject> GetAllPlayers()
    {
        return playerGODict;
    }

    public PlayerInfo GetPlayerByID(string ActorNumber)
    {
        return GetPlayerInfo(ActorNumber);
    }

    public PlayerInfo GetLocalPlayer()
    {
        return GetPlayerInfo(PhotonNetwork.LocalPlayer.ActorNumber.ToString());
    }

    public List<PlayerInfo> GetAlivePlayers()
    {
        return playerDict.Values.Where(p => !p.IsDead).ToList();
    }

    public List<string> GetAllPlayerIDs() => playerDict.Keys.ToList();
    public Dictionary<string, PlayerInfo> GetAllPlayersDict() => playerDict;

    public void KillPlayer(string playerID)
    {
        if (playerDict.TryGetValue(playerID, out var player))
        {
            player.IsDead = true;
            Debug.Log($"{player.Nickname} has been killed.");
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        if (!IsPlayerEvent(photonEvent)) return;

        object[] data = photonEvent.CustomData as object[];
        if (data == null || data.Length < 1) return;

        int actorNumber = (int)data[0];
        string actorKey = actorNumber.ToString();
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
                    RegisterPlayers((int)data[1], actorKey);
                }
                
                break;
            case EventCodes.PlayerJump:
                controller = FindPlayerController(actorKey);
                controller.StartCoroutine(controller.JumpCoroutine());
                break;
            case EventCodes.PlayerKill:
                 controller = FindPlayerController(actorKey);
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

    public PlayerController FindPlayerController(string actorNumber)
    {
        return playerGODict.TryGetValue(actorNumber, out var go) ? go.GetComponent<PlayerController>() : null;
    }

    public PlayerInfo GetPlayerInfo(string ActorNumber)
    {
        return playerGODict[ActorNumber].GetComponent<PlayerInfo>();        
    }
}
