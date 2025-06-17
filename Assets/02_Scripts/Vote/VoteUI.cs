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

    // 투표 시간 업데이트
    public void UpdateTimerUI(int seconds)
    {
        timeText.text = $"투표 시간이 {seconds}초 남았습니다...";
    }

    public void ResetVoteUI()
    {
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(PlayerPropKey.IsDead, out object isDead);
        
        foreach (VoteUISlot slot in slots)
        {
            slot.PrepareForVote();
            if ((bool)isDead)
            {
                slot.IsDeadPeopleUI();
            }
        }
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
        if (this.gameObject.activeInHierarchy)
        {
            StartCoroutine(Ballot(finalCounts, onComplete));    
        }
    }

    private IEnumerator Ballot(Dictionary<int, int> finalCounts, Action onComplete)
    {

        if (finalCounts == null || finalCounts.Count == 0)
        {
            Debug.LogWarning("[Ballot] finalCounts가 비어 있습니다.");
            UIManager.Instance.HideVotingUI();
            onComplete?.Invoke();
            yield break;
        }

        foreach (var slot in slots)
            if (slot.gameObject.activeSelf)
                slot.PrepareForBallot();

        // 각 투표 대상자에게 받은 표 수만큼 AddMark를 한 번씩 실행
        // 이를 라운드별로 나눠서 순차적으로 실행
        
        //{누가, 누구를 뽑았다}
        //{누가, 몇표씩 받았는가}
        Dictionary<int, int> voteResults = new Dictionary<int, int>();
        foreach (var vote in finalCounts)
        {
            if (voteResults.ContainsKey(vote.Value))
            {
                voteResults[vote.Value]++;   
            }
            else
            {
                voteResults[vote.Value] = 1;
            }
        }

        int maxVote = finalCounts.Values.Max();
        for (int i = 0; i < maxVote; i++)
        {
            foreach (var pair in voteResults)
            {
                if (pair.Value > i)
                {
                    var slot = slots.Find(s => s.TargetPlayerId == pair.Key);
                    slot?.AddMark(); // 한 번에 하나씩 추가
                }
            }
            yield return new WaitForSeconds(1f);
        }
        yield return new WaitForSeconds(1f);
        UIManager.Instance.HideVotingUI();
        onComplete?.Invoke();
    }
}