using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class MissionManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public static MissionManager Instance { get; private set; }

    private List<Mission> allMissions = new List<Mission>();
    private Dictionary<string, List<Mission>> playerMissions = new Dictionary<string, List<Mission>>();

    public IReadOnlyList<Mission> AllMissions => allMissions;
    public IReadOnlyDictionary<string, List<Mission>> PlayerMissions => playerMissions;

    [SerializeField]
    public PlayerController playerController;

    new void OnEnable() => PhotonNetwork.AddCallbackTarget(this);
    new void OnDisable() => PhotonNetwork.RemoveCallbackTarget(this);
    

    private void Awake()
    {
        Instance = this;
        LoadAllMissions();
    }

    private void OnDestroy()
    {
        if (playerController != null)
            playerController.OnInteract -= HandlePlayerInteract;
    }

    private void HandlePlayerInteract()
    {
        Collider2D hit = Physics2D.OverlapCircle(
            playerController.transform.position,
            playerController.interactRange,
            playerController.interactLayer
        );
        if (hit != null && hit.TryGetComponent<MissionCollider>(out var trigger))
        {
            string pid = PhotonNetwork.LocalPlayer.ActorNumber.ToString();
            trigger.HandleInteract(pid);
        }
    }

    private void LoadAllMissions()
    {
        allMissions.Clear();
        foreach (var type in MissionFactory.GetAllTypes())
        {
            allMissions.Add(MissionFactory.Create(type));
        }
    }

    public void AssignMissions(string playerKey, int count)
    {
        if (playerMissions.ContainsKey(playerKey)) return;

        var rng = new System.Random();
        var selected = allMissions
            .OrderBy(_ => rng.Next())
            .Take(count)
            .Select(proto => proto.Clone())
            .ToList();

        playerMissions[playerKey] = selected;
        Debug.Log("playerMissions["+playerKey+"]="+playerMissions[playerKey]);
        var ids = selected.Select(m => m.MissionID).ToArray();
        PhotonNetwork.RaiseEvent(
            EventCodes.MissionsAssigned,
            new object[] { playerKey, ids },
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            SendOptions.SendReliable);
    }

    public void CompleteMission(string playerKey, string missionID)
    {
        SoundManager.Instance.PlaySFX(SFXType.MissionComplete);
        Debug.Log("Mission Completed");
        playerController.SetInteraction(false);
        if (!playerMissions.TryGetValue(playerKey, out var list))
            return;

        var mission = list.Find(m => m.MissionID == missionID);
        if (mission != null && !mission.IsCompleted)
        {
            mission.Complete();
            PhotonNetwork.RaiseEvent(
                EventCodes.MissionCompleted,
                new object[] { playerKey, missionID },
                new RaiseEventOptions { Receivers = ReceiverGroup.All },
                SendOptions.SendReliable);
            
            PhotonNetwork.RaiseEvent(
                EventCodes.MissionCompletedUIRefresh,
                new object[] {},
                new RaiseEventOptions { Receivers = ReceiverGroup.All },
                SendOptions.SendReliable);
            CheckCrewmateVictory();
        }
    }

    public float GetProgress(string playerKey)
    {
        if (!playerMissions.TryGetValue(playerKey, out var list) || list.Count == 0)
        {
            Debug.Log(list.Count);
            return 0f;
        }
        
        int done = list.Count(m => m.IsCompleted);
        Debug.Log(done+"/"+list.Count);
        return (float)done / list.Count;
    }

    public void OnEvent(EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case EventCodes.MissionsAssigned:
                var dataA = (object[])photonEvent.CustomData;
                string playerId = (string)dataA[0];
                string[] missionList = (string[])dataA[1];
                var clones = missionList
                    .Select(mid => allMissions.First(m => m.MissionID == mid).Clone())
                    .ToList();
                Debug.Log("테스트 이벤트 할당 중 플레이어는 : "+playerId);
                playerMissions[playerId] = clones;
                Debug.Log("테스트 이벤트 할당 중"+playerMissions[playerId]);
                break;

            case EventCodes.MissionCompleted:
                var dataC = (object[])photonEvent.CustomData;
                string pidC = (string)dataC[0];
                string midC = (string)dataC[1];
                if (playerMissions.TryGetValue(pidC, out var listC))
                {
                    var ms = listC.FirstOrDefault(m => m.MissionID == midC);
                    if (ms != null && !ms.IsCompleted)
                    {
                        ms.Complete();
                        CheckCrewmateVictory();
                    }
                }
                CheckCrewmateVictory();
                break;
        }
    }

    public void CheckCrewmateVictory()
    {
        if (GetTotalProgress() < 1f)
            return;
        Debug.Log("[MissionManager] 모든 미션 완료! 승리 처리 호출");
        GameManager.Instance.EndGame(EndGameCategory.CitizensWin);
    }
    
    public float GetTotalProgress()
    {
        int totalMissions = 0;
        int totalCompleted = 0;

        foreach (var targetList in playerMissions)
        {
            var missionList = targetList.Value;
            totalMissions += missionList.Count;
            totalCompleted += missionList.Count(m => m.IsCompleted);
        }

        if (totalMissions == 0)
            return 0f;
        Debug.Log("totalCompleted : "+totalCompleted+"totalMissions"+totalMissions);
        return (float)totalCompleted / totalMissions;
    }

    public void Init()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        int missionCount = 4;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("MissionCount", out object missionCountObj))
        {
            missionCount = (int)missionCountObj;
        }

        foreach (var player in PhotonNetwork.PlayerList)
        {
            string playerKey = player.ActorNumber.ToString();
            Role role = Role.UnManaged;
            if (player.CustomProperties.TryGetValue(PlayerPropKey.Role, out object roleObj))
            {
                role = (Role)Convert.ToInt32(roleObj);
            }
            Debug.Log("미션할당 직전 플레이어의 ActorNumber : " + player.ActorNumber + "플레이어의 role" + role.ToString());
            if (role == Role.Crewmate)
            {
                // 크루원은 미션 할당
                AssignMissions(playerKey, missionCount);
            }
        }
        PhotonNetwork.RaiseEvent(
            EventCodes.MissionsAssignedCompleted,
            new object[] {},
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            SendOptions.SendReliable);
        
        Debug.Log($"각 {missionCount}개의 미션 할당 완료.");
    }

    public void RegisterLocalPlayer(PlayerController controller)
    {
        this.playerController = controller;
        controller.OnInteract += HandlePlayerInteract;
    }

    public void CloseAllMissionUIs()
    {
        foreach (var mc in FindObjectsOfType<MissionCollider>())
        {
            mc.CloseUI();
        }
    }
}
