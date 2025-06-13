using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { 
    Lobby, 
    RoleAssignment, 
    Playing, 
    Meeting, 
    Voting, 
    Result 
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState CurrentState { get; private set; }

    void Awake()
    {
        Instance = this;
        CurrentState = GameState.Lobby;
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

            case GameState.RoleAssignment:
                // - GameState.RoleAssignment 전환되는 시점
                // 1. 게임이 시작되었을 시

                //플레이어 역할을 할당하고 게임 시작 로직
                RoleManager.Instance.AssignRoles();

                ChangeState(GameState.Playing);
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
        Debug.Log($"Game State changed: {CurrentState} -> {newState}");
        CurrentState = newState;
    }

    private void CheckGameResult()
    {
        // 남아있는 임포스터 수와 크루메이트 수를 체크하여 게임 종료 여부 결정

        // 미션 완료 여부도 체크할 것

        // 해당 메서드는 살해 발생, 혹은 투표 방출 이후에 호출할 것.
    }
}