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

        // (1) ���콺 ��ũ�� ��ǥ �� �г� ���� ��ǥ
        Vector2 mouseLocal;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            panelRect,
            Input.mousePosition,
            ui.GetComponentInParent<Canvas>().worldCamera,
            out mouseLocal
        );
        var r = panelRect.rect;
        mouseLocal.x = Mathf.Clamp(mouseLocal.x, r.xMin, r.xMax);
        mouseLocal.y = Mathf.Clamp(mouseLocal.y, r.yMin, r.yMax);

        // (2) �ε巴�� ���󰡱�
        rt.anchoredPosition = Vector2.Lerp(
            rt.anchoredPosition,
            mouseLocal,
            speed * Time.deltaTime
        );

        // (3) �� ���� ���� üũ
        if (!inGoal && RectIntersects(rt, goalArea))
        {
            inGoal = true;
            MissionManager.Instance.CompleteMission(playerId, mission.MissionID);
            ui.Hide();
        }

        // (4) ��ֹ� �浹 üũ
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

    // �� RectTransform�� ���� �г� ��ǥ�迡�� ��ġ���� �˻�
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

