using _02_Scripts.Ung_Managers;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class VoteUI : MonoBehaviour
{
    public static VoteUI Instance { get; private set; }

    [Header("UI 참조")]
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Transform contentParent;
    private List<VoteUISlot> slots;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        slots = contentParent
            .GetComponentsInChildren<VoteUISlot>(includeInactive: true)
            .ToList();
    }

    private void OnEnable()
    {
        PopulateSlots(); // 신고자 표시 포함해서 한 번에 초기화
    }

    // 투표 시간 업데이트
    public void UpdateTimerUI(int seconds)
    {
        timeText.text = $"투표 시간이 {seconds}초 남았습니다...";
    }

    public void PopulateSlots()
    {
        var players = PhotonNetwork.PlayerList;
        int count = players.Length;

        for (int i = 0; i < slots.Count; i++)
        {
            var slot = slots[i];

            if (i < count)
            {
                var p = players[i];

                // 슬롯 데이터 채우기
                bool isDead = (bool)(p.CustomProperties[PlayerPropKey.IsDead] ?? false);
                bool isReporter = p.ActorNumber == ReportManager.Instance.LastReporter;
                bool hasVoted = VoteManager.Instance.VoteResults.ContainsKey(p.ActorNumber);
                Sprite avatar = AvatarManager.Instance.GetSprite(p.ActorNumber);

                slot.Init(
                  p.ActorNumber,
                  p.NickName,
                  avatar,
                  isDead,
                  isReporter,
                  hasVoted
                );

                // 활성화
                slot.gameObject.SetActive(true);
            }
            else
            {
                // 남는 슬롯은 비활성화
                slot.gameObject.SetActive(false);
            }
        }
    }

    // finalCounts: actor→득표수
    public void ShowBallotAnimation(Dictionary<int, int> finalCounts, Action onComplete)
    {
        StartCoroutine(Ballot(finalCounts, onComplete));
    }

    private IEnumerator Ballot(Dictionary<int, int> finalCounts, Action onComplete)
    {
        Debug.Log("Ballot 실행");
        // 모든 슬롯 초기화
        foreach (var slot in slots)
            if (slot.gameObject.activeSelf)
                slot.PrepareForBallot();

        // 최대 득표수만큼 애니메이션 진행
        int max = finalCounts.Values
            .GroupBy(v => v)
            .Max(g => g.Count());
        
        Debug.Log("Ballot Max : " + max);

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