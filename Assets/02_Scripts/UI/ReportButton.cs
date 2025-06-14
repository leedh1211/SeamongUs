using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ReportButton : MonoBehaviour
{
    private Button reportButton;

    private void Awake()
    {
        reportButton = GetComponent<Button>();
        reportButton.onClick.AddListener(OnReportClicked);
    }

    private void OnReportClicked()
    {
        Debug.Log("[ReportButton] 신고 버튼 클릭됨");
        // 투표 시작
        VoteManager.Instance.StartVotingPhase(OnVotingEnd);
    }

    private void OnVotingEnd()
    {
        foreach (var player in VoteManager.Instance.VoteResults)
        {
            // 플레이어에게 투표 결과 전송
            Debug.Log($"  {player.Key} => {player.Value}");
        }
    }
}
