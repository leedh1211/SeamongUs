using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VoteManager : MonoBehaviour
{
    public static VoteManager Instance { get; private set; }

    // 투표 시간
    [Header("투표시간(초)")]
    [SerializeField] private float voteTime = 30f; // 테스트용 시간 30초

    private Dictionary<string, string> voteResults = new Dictionary<string, string>();
    private System.Action onVoteEndCallback;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 투표 시작: UI 보여주고 타이머 돌리고 종료 후 콜백
    /// </summary>
    public void StartVotingPhase(System.Action onVoteEnd)
    {
        // 이전 투표 결과 초기화
        voteResults.Clear();
        onVoteEndCallback = onVoteEnd;

        // UI 띄우기
        UIManager.Instance.ShowVotingUI(); // 가상의 UI 매니저, 실제 구현 필요

        // 게임 상태 전환
        GameManager.Instance.ChangeState(GameState.Voting); // 가상의 게임 매니저, 실제 구현 필요

        // 실제 투표 코루틴 시작
        StartCoroutine(VotingRoutine());
    }

    private IEnumerator VotingRoutine()
    {
        // UIManager.Instance.ShowVotingUI();

        Debug.Log("[VoteManager] 투표시작함, 시간: " + voteTime + "초");
        yield return new WaitForSeconds(voteTime);

        // 투표 종료 처리
        EndVote();

        // UI 닫기
        UIManager.Instance.HideAllUI();

        // 게임매니져 콜백 -> 다시 플레이상태로 전환
        onVoteEndCallback?.Invoke();
    }

    /// <summary>
    /// 한 플레이어가 다른 플레이어에게 투표
    /// </summary>
    public void SubmitVote(string voterID, string targetID)
    {
        voteResults[voterID] = targetID;
        Debug.Log($"{voterID} voted for {targetID}");
    }

    /// <summary>
    /// 투표 결과 집계 및 처리 -> 추방(킬) 처리 -> 승리 조건 체크
    /// </summary>
    public void EndVote()
    {
        if (voteResults.Count == 0)
        {
            Debug.Log("No votes submitted.");
            return;
        }

        // 최다 득표수 찾기
        var groups = voteResults
            .GroupBy(keyValue => keyValue.Value)
            .Select(g => new { PlayerID = g.Key, Count = g.Count() })
            .ToList();

        int maxVotes = groups.Max(g => g.Count);
        var top = groups
            .Where(g => g.Count == maxVotes)
            .Select(g => g.PlayerID)
            .ToList();

        // 동점이면 스킵
        if (top.Count > 1)
        {
            Debug.Log("동점 / 스킵");
            return;
        }

        // 한명이 최다 득표할 경우 추방
        string votedOut = voteResults.GroupBy(kv => kv.Value).OrderByDescending(g => g.Count()).First().Key;
        Debug.Log($"Voted out player: {votedOut}");

        // 실제 제거 처리 로직 추가 요망

        voteResults.Clear();
    }
}
