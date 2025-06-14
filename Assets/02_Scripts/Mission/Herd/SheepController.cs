using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SheepController : MonoBehaviour
{
    private RectTransform panelRect;
    private RectTransform goalArea;
    private List<RectTransform> obstacles;
    private HerdMission mission;
    private string playerId;
    private HerdUI ui;
    private Vector2 initialPos;
    private bool inGoal;
    private float speed;

    /// <summary>
    /// panelRect: HerdUI 패널의 RectTransform
    /// goalArea:  패널 기준 좌표계의 목표 영역
    /// obstacles: 패널 기준 좌표계 장애물 목록
    /// </summary>
    public void Initialize(
        RectTransform panelRect,
        RectTransform goalArea,
        List<RectTransform> obstacles,
        HerdMission mission,
        string playerId,
        HerdUI ui,
        float speed
    )
    {
        this.panelRect = panelRect;
        this.goalArea = goalArea;
        this.obstacles = obstacles;
        this.mission = mission;
        this.playerId = playerId;
        this.ui = ui;
        this.speed = speed;

        var rt = GetComponent<RectTransform>();
        initialPos = rt.anchoredPosition;
        inGoal = false;
    }

    void Update()
    {
        var rt = GetComponent<RectTransform>();

        // (1) 마우스 스크린 좌표 → 패널 로컬 좌표
        Vector2 mouseLocal;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            panelRect,
            Input.mousePosition,
            ui.GetComponentInParent<Canvas>().worldCamera,
            out mouseLocal
        );

        // (2) 부드럽게 따라가기
        rt.anchoredPosition = Vector2.Lerp(
            rt.anchoredPosition,
            mouseLocal,
            speed * Time.deltaTime
        );

        // (3) 골 영역 도달 체크
        if (!inGoal && RectIntersects(rt, goalArea))
        {
            inGoal = true;
            mission.CollectOne();
            if (mission.IsCompleted)
            {
                MissionManager.Instance.CompleteMission(playerId, mission.MissionID);
                ui.Hide();
            }
        }

        // (4) 장애물 충돌 체크
        foreach (var obs in obstacles)
        {
            if (RectIntersects(rt, obs))
            {
                // 충돌 즉시 리셋
                rt.anchoredPosition = initialPos;
                inGoal = false;
                break;
            }
        }
    }

    // 두 RectTransform이 같은 패널 좌표계에서 겹치는지 검사
    private bool RectIntersects(RectTransform a, RectTransform b)
    {
        var ap = a.anchoredPosition;
        var bp = b.anchoredPosition;
        var asz = a.sizeDelta * 0.5f;
        var bsz = b.sizeDelta * 0.5f;
        return Mathf.Abs(ap.x - bp.x) < (asz.x + bsz.x)
            && Mathf.Abs(ap.y - bp.y) < (asz.y + bsz.y);
    }
}

