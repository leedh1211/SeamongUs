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

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameState CurrentState { get; private set; }

    int lastDeadActorNumber;
    
    void Awake()
    {
        Instance = this;
        CurrentState = GameState.Lobby;
        DontDestroyOnLoad(this);
        StartCoroutine(SubscribeToUI());
    }
    IEnumerator SubscribeToUI()
    {
        // UIManager 인스턴스가 생성될 때까지 대기
        yield return new WaitUntil(() => UIManager.Instance != null);
        UIManager.Instance.OnReportPopupClosed += HandleReportClosed;
        UIManager.Instance.OnEndGamePopupClosed += HandleEndGameClosed;
    }

    void OnDestroy()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.OnReportPopupClosed -= HandleReportClosed;
            UIManager.Instance.OnEndGamePopupClosed -= HandleEndGameClosed;
        }
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
        if(newState== GameState.Meeting || newState == GameState.Voting)
            MissionManager.Instance.CloseAllMissionUIs();
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
                SoundManager.Instance.StopBGM();
                SoundManager.Instance.PlayBGM(BGMType.Play);
                UIManager.Instance.HideVotingUI();
                
                break;
            case GameState.Meeting:
                // 플레이어 위치 이동, 시체 신고팝업,
                // 팝업 종료시 VoteUI활성화
                UIManager.Instance.RaiseLocalReportPopup(lastDeadActorNumber);
                break;
            case GameState.Voting:
                SoundManager.Instance.StopBGM();
                SoundManager.Instance.PlayBGM(BGMType.Voting);
                VoteManager.Instance.StartVotingPhase(() =>
                {
                });
                break;
            case GameState.Result:
                PhotonNetwork.LoadLevel("WaitingScene");
                break;
        }
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

    public void SetLastDeadActor(int actorNumber)
    {
        lastDeadActorNumber = actorNumber;
    }
}