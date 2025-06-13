using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    // 싱글톤
    public static MissionManager Instance { get; private set; }

    // 전체 미션 프로토타입 (Clone 복제용)
    private List<Mission> allMissions = new List<Mission>();

    // 플레이어별 할당 미션
    private Dictionary<string, List<Mission>> playerMissions = new Dictionary<string, List<Mission>>();

    public IReadOnlyList<Mission> AllMissions => allMissions;
    public IReadOnlyDictionary<string, List<Mission>> PlayerMissions => playerMissions;

    // 생명주기
    private void Awake()
    {
        if (Instance = null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAllMissions();
        }
        else
        {
            Destroy(gameObject);
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

    // 플레이어에게 미션 할당
    public void AssignMissions(string playerID)
    {
        if (playerMissions.ContainsKey(playerID))
        {
            return; // 이미 할당된 플레이어는 무시
        }

        var clones = new List<Mission>();
        foreach (var prototype in allMissions)
        {
            clones.Add(prototype.Clone()); // 각 플레이어에게 개별 인스턴스 할당
        }

        playerMissions.Add(playerID, clones);
        //미션 UI 업데이트
    }

    // 특정 미션 완료 처리
    public void CompleteMission(string playerID, string missionID)
    {
        if (!playerMissions.TryGetValue(playerID, out var list))
        {
            return;
        }

        // 해당 플레이어의 미션 목록에서 미션 찾기
        var mission = list.Find(m => m.MissionID == missionID);
        if (mission != null && !mission.IsCompleted)
        {
            mission.Complete();
            // 미션 UI 업데이트
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
    }
}
