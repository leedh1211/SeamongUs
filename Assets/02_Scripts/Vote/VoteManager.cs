using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System;

public class VoteManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public static VoteManager Instance { get; private set; }

    // 투표 시간
    [Header("투표시간(초)")][SerializeField] private float voteTime = 30f; // 테스트용 시간 30초
    public float VoteTime => voteTime; // VoteUI에서 접근할 수 있도록 공개
    private Dictionary<int, int> voteResults = new Dictionary<int, int>();

    public IReadOnlyDictionary<int, int> VoteResults => voteResults;
    private Action onVoteEndCallback;

    [SerializeField] private VoteUI voteUI;
    private float currentVoteTime;

    private new void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
    }

    private new void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
        base.OnDisable();
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
    public void StartVotingPhase(Action onVoteEnd)
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
        float currentTime = voteTime;

        while (currentTime > 0f)
        {
            yield return new WaitForSeconds(1f);
            currentTime -= 1f;

            // UI 시간 표시 갱신 등을 하고 싶다면 여기서 가능
            voteUI?.UpdateTimerUI((int)currentTime);
        }

        if (PhotonNetwork.IsMasterClient)
            EndVote();
        //UIManager.Instance.HideVotingUI();
        //onVoteEndCallback?.Invoke();
    }

    /// <summary>
    /// 한 플레이어가 다른 플레이어에게 투표
    /// </summary>
    public void SubmitVote(int voterID, int targetID)
    {
        voteResults[voterID] = targetID;
        CheckAllVotesReceived();
        Debug.Log($"{voterID} voted for {targetID}");

        voteUI.PopulateSlots(); // UI 갱신
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
    private void CheckAllVotesReceived()
    {
        // 살아있는 플레이어 수 계산
        int alivePlayerCount = PhotonNetwork.PlayerList
            .Count
            (
            player => !(bool)
            (player.CustomProperties[PlayerPropKey.IsDead] ?? false)
            );

        if (voteResults.Count >= alivePlayerCount)
        {
            // 전원 투표했으니 타이머 즉시 종료
            EndVote();
        }
    }

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
        }
        // 5. 단일 최다 득표자 → 추방 처리
        else
        {
            RaiseVoteResult(top[0].Actor);
        }

        //int target = top[0].Actor;
        //Debug.Log($"추방 대상 ActorNum: {target}");
        //RaiseVoteResult(target);

        // 6. 기록 초기화
        voteResults.Clear();
    }

    private void RaiseVoteResult(int ejectedActorNum)
    {
        Debug.Log("투표결과 이벤트 발행");
        PhotonNetwork.RaiseEvent(
            EventCodes.VoteResult,
            new object[] { ejectedActorNum }, // -1은 스킵
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            SendOptions.SendReliable
        );
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
                //if (PhotonNetwork.IsMasterClient) // 마스터 여부 상관없이 모두 기록
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
            case EventCodes.VoteResult:
                // 투표창 숨기기, playing 상태로 변경
                UIManager.Instance.HideVotingUI();
                onVoteEndCallback?.Invoke();

                // 다음 투표 대비 집계 초기화
                voteResults.Clear();
                break;
        }
    }

    public void SetReportData(int SenderActorNumber, int findPeopleActorNum)
    {
        GameManager.Instance.SetLastDeadActor(findPeopleActorNum);
        GameManager.Instance.ChangeState(GameState.Meeting);
    }
}