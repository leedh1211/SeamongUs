using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class EventCodes
{
    public const byte MissionsAssigned = 1;
    public const byte MissionCompleted = 2;
}

public class MissionManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public static MissionManager Instance { get; private set; }

    private List<Mission> allMissions = new List<Mission>();
    private Dictionary<string, List<Mission>> playerMissions = new Dictionary<string, List<Mission>>();

    public IReadOnlyList<Mission> AllMissions => allMissions;
    public IReadOnlyDictionary<string, List<Mission>> PlayerMissions => playerMissions;

    [SerializeField]
    private PlayerController playerController;

    private void Awake()
    {
        Instance = this;
        PhotonNetwork.AddCallbackTarget(this);
        LoadAllMissions();
    }

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
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
            string pid = PhotonNetwork.LocalPlayer.ActorNumber.ToString(); // ✅ ActorNumber 사용
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

        var ids = selected.Select(m => m.MissionID).ToArray();
        PhotonNetwork.RaiseEvent(
            EventCodes.MissionsAssigned,
            new object[] { playerKey, ids },
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            SendOptions.SendReliable);
    }

    public void CompleteMission(string playerKey, string missionID)
    {
        Debug.Log("Mission Completed");
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
            CheckCrewmateVictory();
        }
    }

    public float GetProgress(string playerKey)
    {
        if (!playerMissions.TryGetValue(playerKey, out var list) || list.Count == 0)
            return 0f;

        int done = list.Count(m => m.IsCompleted);
        return (float)done / list.Count;
    }

    public void OnEvent(EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case EventCodes.MissionsAssigned:
                var dataA = (object[])photonEvent.CustomData;
                string pidA = (string)dataA[0];
                string[] mids = (string[])dataA[1];
                var clones = mids
                    .Select(mid => allMissions.First(m => m.MissionID == mid).Clone())
                    .ToList();
                playerMissions[pidA] = clones;
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
                break;
        }
    }

    public void CheckCrewmateVictory()
    {
        foreach (var player in playerMissions)
        {
            if (GetProgress(player.Key) < 1f)
                return;
        }

        GameManager.Instance.EndGame(EndGameCategory.CitizensWin);
    }

    public void Init()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        int missionCount = 1;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("MissionCount", out object missionCountObj))
        {
            missionCount = (int)missionCountObj;
        }

        foreach (var player in PhotonNetwork.PlayerList)
        {
            string playerKey = player.ActorNumber.ToString(); // ✅ 일관된 키
            AssignMissions(playerKey, missionCount);
        }

        Debug.Log($"각 {missionCount}개의 미션 할당 완료.");
    }

    public void RegisterLocalPlayer(PlayerController controller)
    {
        this.playerController = controller;
        controller.OnInteract += HandlePlayerInteract;
    }
}
