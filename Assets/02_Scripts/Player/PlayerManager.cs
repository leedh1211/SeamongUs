using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    private Dictionary<int, GameObject> playerGODict = new Dictionary<int, GameObject>();
    private HashSet<int> spawnedActorNumbers = new HashSet<int>();

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

        // ★ 추가
        view.Owner.TagObject = playerGO;   // 혹시 빠졌던 경우를 대비
    }


    public Dictionary<int, GameObject> GetAllPlayers()
    {
        return playerGODict;
    }
    
    public GameObject GetPlayersGO(int actorNumber)
    {
        return playerGODict[actorNumber];
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
                if (PhotonNetwork.IsMasterClient && SceneManager.GetActiveScene().name == "GameScene")
                {
                    if (!spawnedActorNumbers.Contains(actorNumber))
                    {
                        spawnedActorNumbers.Add(actorNumber);
                        Debug.Log($"[PlayerManager] 마스터: {actorNumber} 스폰 완료 감지 ({spawnedActorNumbers.Count}/{PhotonNetwork.CurrentRoom.PlayerCount})");

                        if (spawnedActorNumbers.Count == PhotonNetwork.CurrentRoom.PlayerCount &&
                            PhotonNetwork.PlayerList.All(p => playerGODict.ContainsKey(p.ActorNumber)))
                        {
                            Debug.Log("[PlayerManager] 모든 플레이어 스폰 완료. 역할 할당 시작.");
                            int impostorCount = 1;
                            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("ImpostorCount", out object impostorObj))
                            {
                                impostorCount = (int)impostorObj;
                            }

                            Debug.Log("[GameManager] 역할 할당 시작");
                            
                            RoleManager.Instance.AssignRoles(impostorCount);
                        }
                    }
                }
                break;
            case EventCodes.PlayerJump:
                controller = FindPlayerController(actorNumber);
                controller.StartCoroutine(controller.JumpCoroutine());
                break;
            case EventCodes.PlayerAttacked:
            {
                controller = FindPlayerController(photonEvent.Sender);
                controller.SetKillCooldown(3f);
                int receiverActnum = (int)data[0];
                int attackerActorNumber = photonEvent.Sender;
                int receiveDamage = (int)data[1];
                GetPlayersGO(receiverActnum).GetComponent<PlayerDamageReceiver>().ReceiveDamage(receiveDamage, attackerActorNumber, receiverActnum);
            }
                break;
            case EventCodes.PlayerKill: //임포스터 actorNum = data[0]
                controller = FindPlayerController(actorNumber);
                controller.SetKillCooldown(30f);
                break;
            case EventCodes.PlayerDied:
                if (PhotonNetwork.IsMasterClient)
                {
                    object[] datas = photonEvent.CustomData as object[];
                    if (datas == null || data.Length == 0)
                    {
                        Debug.LogWarning("[Event] PlayerDied 이벤트에 데이터가 없음");
                        break;
                    }

                    int actorNumbers = (int)data[0];
                    Debug.Log($"[Event] PlayerDied 수신: Actor#{actorNumbers}");
                    Player targetPlayer = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);
                    if (targetPlayer != null)
                    {
                        Debug.Log($"[Event] MasterClient: {targetPlayer.NickName} 사망 표시 설정 중");
                        targetPlayer.SetCustomProperties(new Hashtable { { PlayerPropKey.IsDead, true } });
                    }
                }

                PlayerController playerController = FindPlayerController(actorNumber);
                if (playerController != null)
                {
                    Debug.Log($"[Event] PlayerController 찾음: {playerController.name}, 사망 처리 시작");
                    playerController.Die();
                }
                else
                {
                    Debug.LogWarning($"[Event] PlayerController를 찾지 못함 (Actor#{actorNumber})");
                }

                break;
            case EventCodes.VoteResult:
            {
                Debug.Log("투표 결과 들어옴");
                // 1. 죽이기 (죽지 않는 값: -1)
                if (actorNumber != -1)
                {
                    var player = FindPlayerController(actorNumber);
                    if (player != null)
                    {
                        player.Die("vote");
                    }
                }
            }
                break;
            case EventCodes.PlayerReport:
            {

                break;
            }
        }
    }
    
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey(PlayerPropKey.IsDead) && SceneManager.GetActiveScene().name == "GameScene")
        {
             CheckEndGame();
        }
    }

    public bool IsPlayerEvent(EventData photonEvent)
    {
        return photonEvent.Code == EventCodes.PlayerSpawn || 
               photonEvent.Code == EventCodes.PlayerJump ||
               photonEvent.Code == EventCodes.PlayerKill ||
               photonEvent.Code == EventCodes.PlayerAttacked ||
               photonEvent.Code == EventCodes.PlayerDied ||
               photonEvent.Code == EventCodes.VoteResult ||
               photonEvent.Code == EventCodes.PlayerReport
               ;
        
    }

    public PlayerController FindPlayerController(int actorNumber)
    {
        return playerGODict.TryGetValue(actorNumber, out var go) ? go.GetComponent<PlayerController>() : null;
    }

    public void CheckEndGame()
    {
        int AliveCrewmate = 0;
        int AliveImposter = 0;
        foreach (var player in PhotonNetwork.PlayerList)
        {
            player.CustomProperties.TryGetValue(PlayerPropKey.IsDead, out object IsDead);
            if (!(bool)IsDead)
            {
                player.CustomProperties.TryGetValue(PlayerPropKey.Role, out object role);
                if ((int)role == 1)
                {
                    AliveCrewmate += 1;
                }else if ((int)role == 2)
                {
                    AliveImposter += 1;
                }
            }
        }
        
        Debug.Log("살아있는 크루원:" + AliveCrewmate);
        Debug.Log("살아있는 임포스터:" + AliveImposter);
        if (AliveCrewmate <= AliveImposter) // 임포스터 승리
        {
            GameManager.Instance.EndGame(EndGameCategory.ImpostorsWin);
        }

        if (AliveImposter == 0) // 생존자 승리
        {
            GameManager.Instance.EndGame(EndGameCategory.CitizensWin);
        }
    }
}
