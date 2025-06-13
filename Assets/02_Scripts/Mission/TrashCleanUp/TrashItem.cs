using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrashItem : MonoBehaviour
{
    private TrashCleanup mission;
    private string playerId;
    private TrashUI ui;


    public void Initialize(TrashCleanup mission, string playerId, TrashUI ui)
    {
        this.mission = mission;
        this.playerId = playerId;
        this.ui = ui;
    }

    private void OnMouseDown() // Unity 내장 메서드, 콜라이더있는 애한테 마우스버튼클릭시 자동호출
    {
        // 1) 미션 로직에 수거 알림
        mission.CollectOne();

        // 2) MissionManager에게 완료 알림 (Photon 연동 시에도 이 호출로 동기화)
        if (mission.IsCompleted)
            MissionManager.Instance.CompleteMission(playerId, mission.MissionID);

        // 3) 이 게임오브젝트(쓰레기) 제거
        Destroy(gameObject);

        // 4) 클리어 시 UI 닫기
        if (mission.IsCompleted)
            ui.Hide();
    }

}
