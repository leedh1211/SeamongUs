using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class VoteManager : MonoBehaviourPunCallbacks
{
    public static VoteManager Instance { get; private set; }

    // 투표 시간
    [Header("투표시간(초)")]
    [SerializeField] private float voteTime = 30f; // 테스트용 시간 30초
    public float VoteTime => voteTime; // VoteUI에서 접근할 수 있도록 공개

    private Dictionary<int, int> voteResults = new Dictionary<int, int>();
    public IReadOnlyDictionary<int, int> VoteResults => voteResults;

    private System.Action onVoteEndCallback;
    [SerializeField] private VoteUI voteUI;
    
    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }


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
        UIManager.Instance.ShowVotingUI();

        // 실제 투표 코루틴 시작
        StartCoroutine(VotingRoutine());
    }

    private IEnumerator VotingRoutine()
    {
        // UIManager.Instance.ShowVotingUI();
        yield return new WaitForSeconds(voteTime);

        // 투표 종료 처리
        EndVote();

        // UI 닫기
        UIManager.Instance.HideVotingUI();

        // 게임매니져 콜백 -> 다시 플레이상태로 전환
        onVoteEndCallback?.Invoke();
    }

    /// <summary>
    /// 한 플레이어가 다른 플레이어에게 투표
    /// </summary>
    public void SubmitVote(int voterID, int targetID)
    {
        voteResults[voterID] = targetID;
        CheckAllVotesReceived();
        Debug.Log($"{voterID} voted for {targetID}");
    }
    
    public void OnVoteButtonClicked(int targetActorNumber)
    {
        object[] data = new object[] { PhotonNetwork.LocalPlayer.ActorNumber, targetActorNumber };
        PhotonNetwork.RaiseEvent(EventCodes.PlayerVote, data, RaiseEventOptions.Default, SendOptions.SendReliable);
    }

    /// <summary>
    /// 투표 결과 집계 및 처리 -> 추방(킬) 처리 -> 승리 조건 체크
    /// </summary>
    public void EndVote()
    {
        if (voteResults.Count == 0) return;

        var grouped = voteResults
            .GroupBy(kv => kv.Value)
            .Select(g => new { Actor = g.Key, Count = g.Count() })
            .ToList();

        int max = grouped.Max(g => g.Count);
        var top = grouped.Where(g => g.Count == max).ToList();

        if (top.Count > 1)
        {
            Debug.Log("투표 동점");
            RaiseVoteResult(-1); // 스킵
            return;
        }

        int target = top[0].Actor;
        Debug.Log($"추방 대상 ActorNum: {target}");
        RaiseVoteResult(target);
        voteResults.Clear();
    }
    
    private void RaiseVoteResult(int actorNum)
    {
        PhotonNetwork.RaiseEvent(
            EventCodes.VoteResult,
            actorNum,
            RaiseEventOptions.Default,
            SendOptions.SendReliable
            );
    }
    
    private void CheckAllVotesReceived()
    {
        if (voteResults.Count >= PhotonNetwork.CurrentRoom.PlayerCount)
        {
            EndVote();
        }
    }
    

    public void OnEvent(EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case EventCodes.PlayerReport :
                Debug.Log("신고이벤트 수신");
                var data = (object[])photonEvent.CustomData;
                int findPeopleActorNum = (int)data[0];
                SetReportData(photonEvent.Sender, findPeopleActorNum);
                break;
            case EventCodes.PlayerVote:
                Debug.Log("투표이벤트 수신");
                var voteData = (object[])photonEvent.CustomData;
                int voter = (int)voteData[0];
                int target = (int)voteData[1];
                if (PhotonNetwork.IsMasterClient)
                    SubmitVote(voter, target);
                break;
        }
    }

    public void SetReportData(int SenderActorNumber, int findPeopleActorNum)
    {
        GameManager.Instance.ChangeState(GameState.Meeting);
    }
}
