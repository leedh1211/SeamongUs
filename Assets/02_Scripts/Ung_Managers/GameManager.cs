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
                // 대기 화면 유지, 유저 접속 대기
                break;

            case GameState.RoleAssignment:
                //플레이어 역할을 할당하고 게임 시작
              //  RoleManager.Instance.AssignRoles();
                ChangeState(GameState.Playing);
                break;

            case GameState.Playing:
                // --- 임포스터 킬 ---

                // --- 미션 상호작용 ---
 
                // --- 사체 신고 ---
                break;

            case GameState.Meeting:
                // 회의 화면 표시
                ChangeState(GameState.Voting);
                break;

            case GameState.Voting:
                // 투표 화면 표시
                VoteManager.Instance.StartVotingPhase(() =>
                {
                    ChangeState(GameState.Playing);
                });
                break;

            case GameState.Result:
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