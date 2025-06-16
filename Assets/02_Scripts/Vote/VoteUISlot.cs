using UnityEngine;
using UnityEngine.UI;
using TMPro;
using _02_Scripts.Ung_Managers;

public class VoteUISlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image spriteAvatar;
    [SerializeField] private Button voteButton;
    [SerializeField] private TextMeshProUGUI isDeadText;
    [SerializeField] private GameObject reporterMark;

    private int targetPlayerId;

    // 슬롯 초기화
    public void Init(int playerId, string playerName, Sprite avatar, bool isDead, bool isReporter)
    {
        targetPlayerId = playerId;
        nameText.text = playerName;
        spriteAvatar.sprite = avatar;

        // 아바타 표시
        avatar = AvatarManager.Instance.GetSprite(playerId);
        if (avatar != null)
        {
            spriteAvatar.sprite = avatar;
        }

        // 사망자 처리
        isDeadText.gameObject.SetActive(isDead);
        voteButton.interactable = !isDead; // 사망자는 투표 불가

        // 신고자 표시
        reporterMark.SetActive(playerId == ReportManager.Instance.LastReporter);

        // 버튼 클릭 이벤트 등록
        voteButton.onClick.RemoveAllListeners();
        voteButton.onClick.AddListener(OnVoteClicked);
    }

    private void OnVoteClicked()
    {
        VoteManager.Instance.OnVoteButtonClicked(targetPlayerId);
        voteButton.interactable = false; // 투표 후 버튼 비활성화
    }
}
