using System.Collections;
using System.Collections.Generic;
using _02_Scripts.Alert;
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
    
    void Awake()
    {
        Instance = this;
        CurrentState = GameState.Lobby;
        DontDestroyOnLoad(this);
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
                GameManager.Instance.ChangeState(GameState.Voting);
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

    public void EndGame(EndGameCategory category)
    {
        string message = category switch
        {
            EndGameCategory.CitizensWin  => "시민이 승리하였습니다.",
            EndGameCategory.ImpostorsWin => "살인마가 승리하였습니다.",
            _ => "게임이 종료되었습니다."
        };
        // 테스트용
        AlertUIManager.Instance.OnAlert(message);

        ChangeState(GameState.Result);
    }
}