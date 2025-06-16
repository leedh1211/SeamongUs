using UnityEngine;
using UnityEngine.UI;
using TMPro;
using _02_Scripts.Ung_Managers;

public class VoteUISlot : MonoBehaviour
{
    [Header("플레이어정보")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image spriteAvatar;

    [Header("버튼 및 상태표시")]
    [SerializeField] private Button voteButton;
    [SerializeField] private TextMeshProUGUI isDeadText;
    [SerializeField] private GameObject reporterMark;
    [SerializeField] private GameObject votedMark; // 투표 참여완료 표시용

    [Header("개표")]
    [SerializeField] private Transform markParent;
    [SerializeField] private GameObject markPrefab;

    public int TargetPlayerId { get; private set; }

    /// <summary>
    /// playerId  : 이 슬롯이 가리키는 ActorNumber  
    /// playerName: 플레이어 이름  
    /// avatar    : 아바타 스프라이트  
    /// isDead    : 이미 죽은 플레이어인가  
    /// isReporter: 이 플레이어가 시체 신고자(발견자)인가  
    /// hasVoted  : 이 플레이어가 이미 투표했는가  
    /// </summary>

    // 슬롯 초기화
    public void Init(int playerId, string playerName, Sprite avatar, bool isDead, bool isReporter, bool hasVoted)
    {
        TargetPlayerId = playerId;
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
        voteButton.interactable = !isDead && !hasVoted; // 사망자와 투표 완료자는 버튼 비활성화

        // 신고자 표시
        reporterMark.SetActive(playerId == ReportManager.Instance.LastReporter);

        // 투표 참여완료 표시
        votedMark.SetActive(hasVoted);

        // 버튼 클릭 이벤트 등록
        voteButton.onClick.RemoveAllListeners();
        voteButton.onClick.AddListener(OnVoteClicked);
    }

    // 개표 시작 전 UI 초기화
    public void PrepareForBallot()
    {
        // 비활성화 시킬 UI 요소들
        voteButton.gameObject.SetActive(false);
        isDeadText.gameObject.SetActive(false);
        reporterMark.SetActive(false);
        votedMark.SetActive(false);

        // 기존 마크 제거
        foreach (Transform transform in markParent) Destroy(transform.gameObject);
    }

    // 한표당 마크 하나 추가
    public void AddMark()
    {
        Instantiate(markPrefab, markParent);
    }

    // 투표 버튼 클릭 시 호출되는 메서드
    private void OnVoteClicked()
    {
        VoteManager.Instance.OnVoteButtonClicked(TargetPlayerId);
        voteButton.interactable = false; // 투표 후 버튼 비활성화
        votedMark.SetActive(true); // 투표 완료 표시
    }
}
