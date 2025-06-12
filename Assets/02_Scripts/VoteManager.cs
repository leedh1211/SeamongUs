using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VoteManager : MonoBehaviour
{
    public static VoteManager Instance { get; private set; }

    private Dictionary<string, string> voteResults = new Dictionary<string, string>();
    private System.Action onVoteEndCallback;

    void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 투표 시작: UI 보여주고 타이머 돌리고 종료 후 콜백
    /// </summary>
    public void StartVotingPhase(System.Action onVoteEnd)
    {
        onVoteEndCallback = onVoteEnd;
        StartCoroutine(VotingRoutine());
    }

    private IEnumerator VotingRoutine()
    {
       // UIManager.Instance.ShowVotingUI();

        Debug.Log("Voting started!");
        yield return new WaitForSeconds(60f); // 60초간 투표 가능, 해당 시간은 추후 float 필드화를 통한 설정 가능하게끔 수정 예정

        EndVote();
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
    /// 투표 결과 집계 및 처리
    /// </summary>
    public void EndVote()
    {
        if (voteResults.Count == 0)
        {
            Debug.Log("No votes submitted.");
            return;
        }

        string votedOut = voteResults.GroupBy(kv => kv.Value).OrderByDescending(g => g.Count()).First().Key;

        Debug.Log($"Voted out player: {votedOut}");
        PlayerManager.Instance.KillPlayer(votedOut); // 실제 제거 처리

        voteResults.Clear();
    }
}
