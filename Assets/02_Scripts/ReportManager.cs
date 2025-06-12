using UnityEngine;

public class ReportManager : MonoBehaviour
{
    public static ReportManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public void ReportBody(string reporterID, string deadPlayerID)
    {
        Debug.Log($"{reporterID} reported {deadPlayerID}'s body.");
        UIManager.Instance.ShowMeetingUI(reporterID);
        // 실제 로직에서는 회의 참여자 목록 등도 세팅해야 함
    }
}
