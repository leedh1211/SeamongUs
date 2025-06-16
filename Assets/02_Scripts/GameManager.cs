using System.Collections;
using System.Collections.Generic;
using _02_Scripts.Alert;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { 
    Lobby,
    WaitingRoom,
    RoleAssignment, 
    PlayingStart,
    Playing, 
    Meeting, 
    Voting, 
    Result 
}

public enum EndGameCategory
{
    CitizensWin,
    ImpostorsWin
}

public class GameManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public static GameManager Instance { get; private set; }
    public GameState CurrentState { get; private set; }

    int lastDeadActorNumber;
    
    void Awake()
    {
        Instance = this;
        CurrentState = GameState.Lobby;
        DontDestroyOnLoad(this);
    }

    void OnEnable() // 시체발견,게임엔딩팝업 콜백 구독
    {
        base.OnEnable();
        UIManager.Instance.OnReportPopupClosed += HandleReportClosed;
        UIManager.Instance.OnEndGamePopupClosed += HandleEndGameClosed;
    }

    void OnDisable()
    {
        UIManager.Instance.OnReportPopupClosed -= HandleReportClosed;
        UIManager.Instance.OnEndGamePopupClosed -= HandleEndGameClosed;
        base.OnDisable();
    }
 

    public void ChangeState(GameState newState)
    {
        if (CurrentState == newState) return;
        ActionChangeState(newState);
        CurrentState = newState;
        Debug.Log($"[GameManager] 상태가 {newState}로 변경됨");
    }

    private void ActionChangeState(GameState newState)
    {
        switch (newState)
        {
            case GameState.Lobby:
                break;
            case GameState.WaitingRoom:
                PhotonNetwork.LoadLevel("WaitingScene");
                break;
            case GameState.RoleAssignment:
                PhotonNetwork.LoadLevel("GameScene");
                break;
            case GameState.PlayingStart:
                MissionManager.Instance.Init();
                break;
            case GameState.Playing:
                UIManager.Instance.HideVotingUI();
                
                break;
            case GameState.Meeting:
                // 플레이어 위치 이동, 시체 신고팝업,
                // 팝업 종료시 VoteUI활성화
                UIManager.Instance.RaiseLocalReportPopup(lastDeadActorNumber);
                break;
            case GameState.Voting:
                VoteManager.Instance.StartVotingPhase(() =>
                {
                    ChangeState(GameState.Playing);
                });
                break;
            case GameState.Result:
                PhotonNetwork.LoadLevel("WaitingScene");
                break;
        }
    }


    public void OnEvent(EventData ev)
    {
        byte code = (byte)ev.Code;

        if (code == EventCodes.PlayerReport)
        {
            lastDeadActorNumber = (int)((object[])ev.CustomData)[0];
            ChangeState(GameState.Meeting);
        }
        // GameEnded 팝업은 UIManager.OnEvent에서 처리
        // ReportManager에서 신고이벤트를 일으키기에 일단 여기서 받았습니다, 이걸통해 meeting 전환후 시체신고팝업을 전체에 띄우려고요.
    }

    public void EndGame(EndGameCategory category)
    {
        PhotonNetwork.RaiseEvent(
           EventCodes.GameEnded,
           (byte)category,
           new RaiseEventOptions { Receivers = ReceiverGroup.All },
           SendOptions.SendReliable
       );
    }
    private void HandleReportClosed()
    {
        ChangeState(GameState.Voting);
    }

    private void HandleEndGameClosed()
    {
        ChangeState(GameState.Result);
    }
}