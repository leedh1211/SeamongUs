using UnityEngine;

public class DeadBody : MonoBehaviour
{
    public string ownerID;
    public string BodyID;             // 유니크 ID
    public string PlayerID;           // 죽은 플레이어 ID

    public void Initialize(string playerID)
    {
        ownerID = playerID;
        // 색상, 닉네임 등 표현 가능
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Player otherPlayer = other.GetComponent<Player>();
        if (otherPlayer != null && !otherPlayer.IsDead)
        {
            Debug.Log($"{otherPlayer.PlayerID}의 시체가 근처에 있어, Report버튼이 활성화됩니다.");
            // UIManager.Instance.ShowReportButton(this);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Player otherPlayer = other.GetComponent<Player>();
        if (otherPlayer != null && !otherPlayer.IsDead)
        {
            Debug.Log($"{otherPlayer.PlayerID}의 시체가 멀어져, Report버튼이 비활성화됩니다.");
            // UIManager.Instance.HideReportButton();
        }
    }
}
