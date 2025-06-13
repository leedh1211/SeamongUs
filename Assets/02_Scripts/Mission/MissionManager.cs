using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun.Demo.PunBasics;

public static class EventCodes
{
    //미션 할당 이벤트 코드
    public const byte MissionsAssigned = 1;
    // 미션 완료 이벤트 코드
    public const byte MissionCompleted = 2;
}

public class MissionManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    // 싱글톤
    public static MissionManager Instance { get; private set; }

    // 전체 미션 프로토타입 (Clone 복제용)
    private List<Mission> allMissions = new List<Mission>();

    // 플레이어별 할당 미션
    private Dictionary<string, List<Mission>> playerMissions = new Dictionary<string, List<Mission>>();

    public IReadOnlyList<Mission> AllMissions => allMissions;
    public IReadOnlyDictionary<string, List<Mission>> PlayerMissions => playerMissions;
    //테스트용
    [SerializeField]
    private PlayerController playerController;

    // 생명주기
    private void Awake()
    {
        Instance = this;
        PhotonNetwork.AddCallbackTarget(this);
        LoadAllMissions();
    }

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
        //테스트용
        if (playerController != null)
            playerController.OnInteract -= HandlePlayerInteract;
    }

    //테스트용
    private void HandlePlayerInteract()
    {
        // E 키 눌렀을 때 OverlapCircle → MissionCollider 호출 로직
        Collider2D hit = Physics2D.OverlapCircle(
            playerController.transform.position,
            playerController.interactRange,
            playerController.interactLayer
        );
        if (hit != null && hit.TryGetComponent<MissionCollider>(out var trigger))
        {
            // 에디터 테스트용 혹은 PhotonNetwork.LocalPlayer.UserId
        string pid = PhotonNetwork.LocalPlayer.UserId;
        trigger.HandleInteract(pid);
        }
    }



    // 전체 미션 한번에 로드
    private void LoadAllMissions()
    {
        allMissions.Clear();
        foreach (var type in MissionFactory.GetAllTypes())
        {
            allMissions.Add(MissionFactory.Create(type));
        }
    }

    // public override void OnJoinedRoom()
    // {
    //     string pid = PhotonNetwork.LocalPlayer.UserId;
    //     AssignMissions(pid);
    // }

    // 플레이어에게 미션 할당
    public void AssignMissions(string playerId, int count)
    {
        if (playerMissions.ContainsKey(playerId)) return;

        var rng = new System.Random();
        var selected = allMissions
            .OrderBy(_ => rng.Next())
            .Take(count)
            .Select(proto => proto.Clone())
            .ToList();

        playerMissions[playerId] = selected;

        var ids = selected.Select(m => m.MissionID).ToArray();
        PhotonNetwork.RaiseEvent(
            EventCodes.MissionsAssigned,
            new object[] { playerId, ids },
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            SendOptions.SendReliable);
    }

    // 특정 미션 완료 처리
    public void CompleteMission(string playerID, string missionID)
    {
        Debug.Log("Mission Completed");
        if (!playerMissions.TryGetValue(playerID, out var list))
        {
            return;
        }

        // 해당 플레이어의 미션 목록에서 미션 찾기
        var mission = list.Find(m => m.MissionID == missionID);
        if (mission != null && !mission.IsCompleted)
        {
            mission.Complete();
            PhotonNetwork.RaiseEvent(
                EventCodes.MissionCompleted,
                new object[] { playerID, missionID },
                new RaiseEventOptions { Receivers = ReceiverGroup.All },
                SendOptions.SendReliable);
            CheckCrewmateVictory();
        }
    }

    public float GetProgress(string playerID)
    {
        if (!playerMissions.TryGetValue(playerID, out var list) || list.Count == 0)
        {
            return 0f; // 할당된 미션이 없으면 0% 진행률
        }

        int done = 0; // 완료된 미션 개수
        foreach (var mission in list)
        {
            if (mission.IsCompleted)
            {
                done++;
            }
        }

        return (float)done / list.Count; // 완료된 미션 개수를 전체 미션 개수로 나누어 진행률 계산
    }
    public void OnEvent(EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case EventCodes.MissionsAssigned:
                var dataA = (object[])photonEvent.CustomData;
                string pidA = (string)dataA[0];
                string[] mids = (string[])dataA[1];
                var clones = new List<Mission>();
                foreach (var mid in mids)
                {
                    var proto = allMissions.First(m => m.MissionID == mid);
                    clones.Add(proto.Clone());
                }
                playerMissions[pidA] = clones;
                break;

            case EventCodes.MissionCompleted:
                var dataC = (object[])photonEvent.CustomData;
                string pidC = (string)dataC[0];
                string midC = (string)dataC[1];
                if (playerMissions.TryGetValue(pidC, out var listC))
                {
                    var ms = listC.FirstOrDefault(m => m.MissionID == midC);
                    if (ms != null && !ms.IsCompleted) ms.Complete();
                    CheckCrewmateVictory();
                }
                break;
        }
    }

    // 일반 시민 승리 조건 체크
    // 전원 클리어시 승리 처리 상황( 나중에 조건 변경 가능 )
    public void CheckCrewmateVictory()
    {
        // 모든 플레이어의 미션이 완료되었는지 확인
        foreach (var player in playerMissions)
        {
            if (GetProgress(player.Key) < 1f)
            {
                return; // 아직 완료되지 않은 플레이어가 있으면 승리 조건 미달
            }
        }

        // 모든 플레이어의 미션이 완료되었으므로 승리 처리
        GameManager.Instance.EndGame(EndGameCategory.CitizensWin);
        //GameManager.Instance.EndGame(false); 같은 느낌. (false은 일반 시민 승리, true는 임포스터 승리로 가정)
    }

    public void Init()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            int missionCount = 1; // 기본값
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("MissionCount", out object missionCountObj))
            {
                missionCount = (int)missionCountObj;
            }
            
            foreach (var player in PhotonNetwork.PlayerList)
            {
                AssignMissions(player.UserId, missionCount);
            }
            Debug.Log($"각 {missionCount}개의 미션 할당");
        }
    }
    
    public void RegisterLocalPlayer(PlayerController controller)
    {
        this.playerController = controller;
        controller.OnInteract += HandlePlayerInteract;
    }
}
