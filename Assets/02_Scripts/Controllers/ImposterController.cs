using UnityEngine;

public class ImposterController : MonoBehaviour
{
    private PlayerInfo self;

    void Start()
    {
        self = PlayerManager.Instance.GetLocalPlayer();  // LocalPlayer 정보 가져오기
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            TryKill();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            TryReportBody();
        }
    }

    void TryKill()
    {
        PlayerInfo target = FindKillablePlayer();
        Debug.Log($"{target?.Nickname ?? "None"} Player를 살해 시도합니다.");
        if (target != null)
        {
            Debug.Log($"{self.Nickname}플레이어가 {target.Nickname} 살해");
            PlayerManager.Instance.KillPlayer(target.PlayerID);
        }
    }

    void TryReportBody()
    {
        string deadID = FindNearestDeadBody();
        if (deadID != null)
        {
            ReportManager.Instance.ReportBody(self.PlayerID, deadID);
            GameManager.Instance.ChangeState(GameState.Meeting);
        }
    }

    PlayerInfo FindKillablePlayer()
    {
        var alivePlayers = PlayerManager.Instance.GetAlivePlayers();
        foreach (var p in alivePlayers)
        {
            if (p.PlayerID != self.PlayerID)
            {
                float dist = Vector3.Distance(transform.position, p.Transform.position);
                if (dist < 2.0f) return p;
            }
        }
        return null;
    }

    string FindNearestDeadBody()
    {
        return DeadBodyManager.Instance.GetClosestDeadBodyID(transform.position);
    }
}
