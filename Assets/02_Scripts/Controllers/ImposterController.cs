using UnityEngine;

public class ImposterController : MonoBehaviour
{
    private PlayerInfo self;

    void Update()
    {
        if (self == null)
        {
            self = PlayerManager.Instance.GetLocalPlayer();
            if (self != null)
            {
                Debug.Log($"[ImposterController] self 초기화 성공: {self.Nickname}");
            }
        }

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
        Debug.Log(target);
        if (target != null && !target.IsDead)
        {
            

            Debug.Log($"{self.Nickname} 플레이어가 {target.Nickname} 살해");
            PlayerManager.Instance.KillPlayer(target.PlayerID);
        }
        else
        {
            Debug.Log("살해 가능한 플레이어가 없습니다.");
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
        float killRange = 2.0f;
        foreach (var player in PlayerManager.Instance.GetAlivePlayers())
        {
            if (player == null) continue;
            if (self == null) continue; // 혹은 self 초기화 보장

            if (player.PlayerID == self.PlayerID) continue;  // PlayerID 기준 비교가 더 안전
            if (player.Role != Role.Crewmate) continue;
            if (player.IsDead) continue;

            float dist = Vector3.Distance(self.Transform.position, player.Transform.position);
            if (dist <= killRange)
            {
                return player;
            }
        }
        return null;
    }


    string FindNearestDeadBody()
    {
        return DeadBodyManager.Instance.GetClosestDeadBodyID(transform.position);
    }
}
