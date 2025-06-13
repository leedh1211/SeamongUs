using UnityEngine;

public class Player : MonoBehaviour
{
    public string PlayerID { get; private set; }
    public bool IsDead { get; private set; } = false;

    public void Initialize(string id)
    {
        PlayerID = id;
    }

    public void Die()
    {
        if (IsDead) return;
        IsDead = true;

        // 유령 처리
        GetComponent<PlayerController>().enabled = false;

        // 시체 생성
        DeadBodyManager.Instance.SpawnDeadBody(transform.position, PlayerID);

        // 시각 효과 (투명도 등)
        // TODO: 유령 상태로 시각적 변경
    }
}
