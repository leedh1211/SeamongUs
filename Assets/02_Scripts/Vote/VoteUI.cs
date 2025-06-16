using System.Collections;
using UnityEngine;
using TMPro;
using Photon.Pun;
using _02_Scripts.Ung_Managers;

public class VoteUI : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject slotTemplate;

    private void OnEnable()
    {
        PopulateSlots();
    }

    private void OnDisable()
    {
        // 다음에 켜질 때 UI 초기화
        foreach (var slot in contentParent.GetComponentsInChildren<VoteUISlot>())
            slot.gameObject.SetActive(false);
    }

    private IEnumerator StartTimer()
    {
        float time = VoteManager.Instance.VoteTime;
        while (time > 0)
        {
            timeText.text = $"회의 및 투표시간이 {Mathf.CeilToInt(time)}초 남았습니다...";
            yield return new WaitForSeconds(1f);
            time -= 1f;
        }
    }

    public void UpdateTimerUI(int currentVoteTime)
    {
        timeText.text = $"{Mathf.CeilToInt(currentVoteTime)}초 남았습니다...";
    }

    // contentParent에 배치된 자식 슬롯들 사용으로 변경
    public void PopulateSlots()
    {
        var slots = contentParent.GetComponentsInChildren<VoteUISlot>(includeInactive: true);
        var players = PhotonNetwork.PlayerList;
        int count = Mathf.Min(slots.Length, players.Length);

        int reporterID = ReportManager.Instance.LastReporter;

        for (int i = 0; i < count; i++)
        {
            var slot = slots[i];
            var player = players[i];

            // 슬롯 활성화
            slot.gameObject.SetActive(true);

            // 사망여부
            bool isDead = false;
            if (player.CustomProperties.TryGetValue(PlayerPropKey.IsDead, out var deadObj))
                isDead = (bool)deadObj;

            // 신고자 확인
            bool isReporter = (player.ActorNumber == reporterID);

            // 투표참여여부 확인
            bool hasVoted = VoteManager.Instance.VoteResults.ContainsKey(player.ActorNumber);

            // 슬롯 초기화 호출
            // 스프라이트 가져오도록 추가 연결
            Sprite avatar = AvatarManager.Instance.GetSprite(player.ActorNumber);
            slot.Init(
            player.ActorNumber,
            player.NickName,
            avatar,
            isDead,
            isReporter,
            hasVoted
            );
        }
    }
}
