﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SignpostUI : MonoBehaviour, IMissionUI
{
    [Header("팻말 조각 버튼들")]
    [SerializeField] private List<Button> pieceButtons;

    private string playerId;
    private string missionID;

    private readonly int[] angles = { 90, 135, 180, 225, 270, 315 };
    public PlayerController playerController;
    private void Awake()
    {
        // 각 버튼 클릭 시 회전 이벤트 연결
        foreach (var button in pieceButtons)
        {
            button.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySFX(SFXType.Click);
                OnPieceClicked(button);
            });
        }
    }

    // IMissionUI 구현
    public void Show(Mission mission, string playerId)
    {
        this.playerId = playerId;
        this.missionID = mission.MissionID;
        RandomizePieces();
        gameObject.SetActive(true);
    }

    private void OnPieceClicked(Button button)
    {
        // 45도씩 시계 방향 회전
        var rotation = button.transform as RectTransform;
        rotation.localRotation *= Quaternion.Euler(0, 0, -45f);
        CheckCompletion();
    }

    private void RandomizePieces()
    {
        // 0도(정답)만 빼고 랜덤 초기화
        foreach (var button in pieceButtons)
        {
            int angle = angles[Random.Range(0, angles.Length)];
            button.transform.localRotation = Quaternion.Euler(0, 0, -angle);
        }
    }

    private void CheckCompletion()
    {
        // Mission 객체에 위임 후, 완료되었다면 UI에서만 통보
        bool allAligned = true;
        foreach (var button in pieceButtons)
        {
            if (Mathf.Abs(button.transform.localEulerAngles.z) > 0.1f)
            {
                allAligned = false;
                break;
            }
        }
        if (!allAligned) return;

        // Mission 객체 스스로 IsCompleted 플래그 세팅
        var mission = MissionManager.Instance
            .PlayerMissions[playerId]
            .First(m => m.MissionID == missionID);

        // 서버에 완료 이벤트 전송 및 전체 체크
        MissionManager.Instance.CompleteMission(playerId, missionID);

        // 창닫기
        gameObject.SetActive(false);
    }
}
