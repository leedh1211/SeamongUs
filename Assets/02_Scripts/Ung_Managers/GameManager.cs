using System.Collections;
using System.Collections.Generic;
using _02_Scripts.Alert;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { 
    Lobby,
    WaitingRoom,
    RoleAssignment, 
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
    [SerializeField] private GameObject missionManager;
    public static GameManager Instance { get; private set; }
    public GameState CurrentState { get; private set; }
    
    void Awake()
    {
        Instance = this;
        CurrentState = GameState.Lobby;
        DontDestroyOnLoad(this);
    }

    void Update()
    {
        switch (CurrentState)
        {
            case GameState.Lobby:
                // - GameState.Lobby 전환되는 시점
                // 1. 플레이어가 방에 입장 시
                // 2. 게임 종료 시

                // 대기 화면 유지, 유저 접속 대기 로직
                break;
            case GameState.WaitingRoom:
                // -웨이팅룸에 입장하는 시점
                // 1. 시작버튼 클릭시까지 대기
                // 2. 시작버튼 클릭시 -> 임포스터 수, 미션 수와같은 정보를 포톤룸에 저장(포톤 커스텀 프로퍼티)
                break;
            case GameState.RoleAssignment:
                // - GameState.RoleAssignment 전환되는 시점
                // 1. 게임이 시작되었을 시
                break;

            case GameState.Playing:
                // - GameState.Playing 전환되는 시점
                // 1. RoleAssignment(역할 할당) 직후
                // 2. 투표가 종료된 시점에서 누군가의 승리 조건이 아직 만족되지 않았을 시
                
                // --- 임포스터 킬 로직---

                // --- 미션 상호작용 로직---

                // --- 사체 신고 로직---
                break;

            case GameState.Meeting:
                // - GameState.Meeting 전환되는 시점
                // 1. 사체 신고가 발생했을 때
                // 2. 가운데 종을 울려 회의를 소집하였을 때

                // 회의 화면 표시
                ChangeState(GameState.Voting);
                break;

            case GameState.Voting:
                // GameState.Voting 전환되는 시점
                // 1. 모든 플레이어가 회의를 시작했을 때

                // 투표 화면 표시
                VoteManager.Instance.StartVotingPhase(() =>
                {
                    ChangeState(GameState.Playing);
                });
                break;

            case GameState.Result:
                // - GameState.Result 전환되는 시점
                // 1. 임포스터와 크루원의 수가 동등해져 임포스터의 승리조건이 만족되었을 시
                // 2. 크루원이 모든 미션을 완료했을 시
                // 3. 투표를 통해 모든 임포스터가 제거되었을 시

                // 게임 종료화면 표시
                break;
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
            case GameState.Playing:
                MissionManager.Instance.Init();
                break;
            case GameState.Meeting:
                break;
            case GameState.Voting:
                break;
            case GameState.Result:
                break;
        }
    }

    private void CheckGameResult() // 이동헌 -> 게임매니저는 스테이트에 관한것만 관리하고, 투표Manager, PlayerManager, ImposterManager쪽에서 승패 결과 판단해서 게임매니저쪽에 EndGame만 호출해주는게 좋아보입니다.
    {
        // 남아있는 임포스터 수와 크루메이트 수를 체크하여 게임 종료 여부 결정

        // 미션 완료 여부도 체크할 것

        // 해당 메서드는 살해 발생, 혹은 투표 방출 이후에 호출할 것.
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