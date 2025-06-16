using _02_Scripts.Ung_Managers;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;

public class VoteUI : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject slotTemplate;

    private List<VoteUISlot> slots;

    private void Awake()
    {
        slots = new List<VoteUISlot>();
        foreach (var slot in contentParent.GetComponentsInChildren<VoteUISlot>(true))
            slots.Add(slot);
    }

    // 투표 시간 업데이트
    public void UpdateTimerUI(int seconds)
    {
        timeText.text = $"투표 시간이 {seconds}초 남았습니다...";
        return;
    }

    public void PopulateSlots()
    {
        var players = PhotonNetwork.PlayerList;
        int count = Mathf.Min(slots.Count, players.Length);

        // 우선 모든 슬롯 비활성화
        foreach (var slot in slots)
            slot.gameObject.SetActive(false);

        for (int i = 0; i < count; i++)
        {
            var slot = slots[i];
            var p = players[i];
            slot.gameObject.SetActive(true);

            // 플레이어 정보 가져오기
            bool isDead = (bool)(p.CustomProperties[PlayerPropKey.IsDead] ?? false);
            bool isReporter = p.ActorNumber == ReportManager.Instance.LastReporter;
            bool hasVoted = VoteManager.Instance.VoteResults.ContainsKey(p.ActorNumber);
            Sprite avatar = AvatarManager.Instance.GetSprite(p.ActorNumber);

            // 슬롯 초기화
            slot.Init(p.ActorNumber, p.NickName, avatar, isDead, isReporter, hasVoted);
        }
    }

    // finalCounts: actor→득표수
    public void ShowBallotAnimation(Dictionary<int, int> finalCounts, Action onComplete)
    {
        StartCoroutine(Ballot(finalCounts, onComplete));
    }

    private IEnumerator Ballot(Dictionary<int, int> finalCounts, Action onComplete)
    {
        // 모든 슬롯 초기화
        foreach (var slot in slots)
            if (slot.gameObject.activeSelf)
                slot.PrepareForBallot();

        // 최대 득표수만큼 애니메이션 진행
        int max = 0;
        foreach (var voted in finalCounts.Values)
            if (voted > max) max = voted;

        for (int round = 1; round <= max; round++)
        {
            foreach (var keyV in finalCounts)
            {
                if (keyV.Value >= round)
                {
                    // 해당 actor의 슬롯 찾아서
                    var slot = slots.Find(s => s.TargetPlayerId == keyV.Key);
                    slot?.AddMark();
                }
            }
            yield return new WaitForSeconds(0.5f);
        }

        // 애니 끝나면 VoteCanvas 닫고 콜백까지 구현함. => 이후 추방 UI (양복님께서 구현 진행)
        UIManager.Instance.HideVotingUI();
        onComplete?.Invoke();
    }
}