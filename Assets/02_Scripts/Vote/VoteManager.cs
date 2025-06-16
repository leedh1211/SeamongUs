using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;
using UnityEngine;
using Hashtable = System.Collections.Hashtable;

public class VoteManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public static VoteManager Instance { get; private set; }

    // 투표 시간
    [Header("투표시간(초)")] [SerializeField] private float voteTime = 30f; // 테스트용 시간 30초
    public float VoteTime => voteTime; // VoteUI에서 접근할 수 있도록 공개
    private Dictionary<int, int> voteResults = new Dictionary<int, int>();
    public IReadOnlyDictionary<int, int> VoteResults => voteResults;
    private System.Action onVoteEndCallback;
    [SerializeField] private VoteUI voteUI;
    private float currentVoteTime;
    private bool voteTimeShortened = false;
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
        currentVoteTime = voteTime;
        voteTimeShortened = false;

        while (currentVoteTime > 0f)
        {
            yield return new WaitForSeconds(1f);
            currentVoteTime -= 1f;

            // UI 시간 표시 갱신 등을 하고 싶다면 여기서 가능
            voteUI?.UpdateTimerUI((int)currentVoteTime);

            // 디버그
            Debug.Log($"[Voting] 남은 시간: {currentVoteTime}");
        }

        if (PhotonNetwork.IsMasterClient)
        {
            EndVote();    
        }
        UIManager.Instance.HideVotingUI();
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
        PhotonNetwork.RaiseEvent(
            EventCodes.PlayerVote,
            data, new RaiseEventOptions { Receivers = ReceiverGroup.All },
            SendOptions.SendReliable);
    }

    /// <summary>
    /// 투표 결과 집계 및 처리 -> 추방(킬) 처리 -> 승리 조건 체크
    /// </summary>
    public void EndVote()
    {
        
        Debug.Log("개표 시작");
        
        // 1. 투표 결과 없음
        if (voteResults.Count == 0)
        {
            RaiseVoteResult(-1); // 스킵
            return;
        }
        // 2. 투표 결과를 그룹화하여 득표 수 계산
        var grouped = voteResults
            .GroupBy(kv => kv.Value)
            .Select(g => new { Actor = g.Key, Count = g.Count() })
            .ToList();

        // 3. 최다 득표 수
        int max = grouped.Max(g => g.Count);

        // 4. 최다 득표자가 2명 이상이면 동점 처리
        var top = grouped.Where(g => g.Count == max).ToList();
        if (top.Count > 1)
        {
            Debug.Log("투표 동점");
            RaiseVoteResult(-1); // 스킵
            return;
        }

        // 5. 단일 최다 득표자 → 추방 처리
        int target = top[0].Actor;
        Debug.Log($"추방 대상 ActorNum: {target}");
        RaiseVoteResult(target);

        // 6. 기록 초기화
        voteResults.Clear();
    }

    private void RaiseVoteResult(int actorNum)
    {
        
        Debug.Log("투표결과 이벤트 발행");
        PhotonNetwork.RaiseEvent(
            EventCodes.VoteResult,
            new object[] { actorNum },
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            SendOptions.SendReliable
        );
    }

    private void CheckAllVotesReceived()
    {
        int alivePlayerCount = 0;
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue(PlayerPropKey.IsDead, out object IsDead))
            {
                if (!(bool)IsDead)
                {
                    alivePlayerCount += 1;
                }
            }
        }

        if (voteResults.Count >= alivePlayerCount)
        {
            Debug.Log("투표시간을 6으로 변경합니다");
            SetVoteTime(6);
        }
    }

    private void SetVoteTime(int votetime)
    {
        object[] eventData = new object[] { votetime }; // 투표 다했을 경우, 시간 줄어들게 세팅
        PhotonNetwork.RaiseEvent(
            EventCodes.SetVoteTime,
            eventData,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            SendOptions.SendReliable
        );
    }


    public void OnEvent(EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case EventCodes.PlayerReport:
                Debug.Log("신고이벤트 수신");
                var data = (object[])photonEvent.CustomData;
                int findPeopleActorNum = (int)data[0];
                DeadBodyManager.Instance.RemoveAllDeadBody();
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
            case EventCodes.SetVoteTime:
                Debug.Log("투표시간이 변경됩니다.");
                var voteTimeData = (object[])photonEvent.CustomData;
                int newTime = (int)voteTimeData[0];

                if (currentVoteTime > newTime)
                {
                    currentVoteTime = newTime;
                    Debug.Log($"남은 투표 시간을 {newTime}초로 줄였습니다.");
                }
                break;
        }
    }

    public void SetReportData(int SenderActorNumber, int findPeopleActorNum)
    {
        GameManager.Instance.ChangeState(GameState.Meeting);
    }
}