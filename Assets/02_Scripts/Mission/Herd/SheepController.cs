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
    private Vector2 followOffset;

    public void Initialize(
        RectTransform panelRect,
        RectTransform goalArea,
        List<RectTransform> obstacles,
        HerdMission mission,
        string playerId,
        HerdUI ui,
        float speed,
        Vector2 followOffset
    )
    {
        this.panelRect = panelRect;
        this.goalArea = goalArea;
        this.obstacles = obstacles;
        this.mission = mission;
        this.playerId = playerId;
        this.ui = ui;
        this.speed = speed;
        this.followOffset = followOffset;

        var rt = GetComponent<RectTransform>();
        initialPos = rt.anchoredPosition;
        inGoal = false;
    }

    void Update()
    {
        var rt = GetComponent<RectTransform>();

        // 마우스포인터 위치를 로컬 좌표로 변환
        Vector2 mouseLocal;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            panelRect,
            Input.mousePosition,
            ui.GetComponentInParent<Canvas>().worldCamera,
            out mouseLocal
        );
        // 양 안삐져나오도록 패널 영역 내로 제한
        var r = panelRect.rect;
        mouseLocal.x = Mathf.Clamp(mouseLocal.x, r.xMin, r.xMax);
        mouseLocal.y = Mathf.Clamp(mouseLocal.y, r.yMin, r.yMax);

        // 천천히 마우스포인터 따라붙도록
        rt.anchoredPosition = Vector2.Lerp(
            rt.anchoredPosition,
            mouseLocal,
            speed * Time.deltaTime
        );

        // 미션 클리어
        if (!inGoal && RectIntersects(rt, goalArea))
        {
            inGoal = true;
            MissionManager.Instance.CompleteMission(playerId, mission.MissionID);
            ui.Hide();
        }

        // 겹칠시 초기위치로
        foreach (var obs in obstacles)
        {
            if (RectIntersects(rt, obs))
            {
                // �浹 ��� ����
                rt.anchoredPosition = initialPos;
                inGoal = false;
                break;
            }
        }
    }

    // 겹침 방지 bool 메서드
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

