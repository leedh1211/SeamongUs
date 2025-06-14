using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class DropZone : MonoBehaviour, IDropHandler
{
    [SerializeField] private int slotIndex;
    private Laundry mission;
    private string playerId;
    private RectTransform dragArea;
    private LaundryUI laundryUI;  // UI도 바로 닫을 수 있도록 참조

    public void Initialize(Laundry mission, string playerId, RectTransform dragArea, LaundryUI ui)
    {
        this.mission = mission;
        this.playerId = playerId;
        this.dragArea = dragArea;
        this.laundryUI = ui;
    }

    public void OnDrop(PointerEventData eventData)
    {
        var dragCloth = eventData.pointerDrag.GetComponent<DragCloth>();
        if (dragCloth == null || mission == null) return;

        // 이 슬롯에 맞는 prefab
        var expected = mission.GetorderPrefabs()[slotIndex];
        bool isCorrect = dragCloth.clothPrefab == expected;

        if (isCorrect)
        {
            // 스냅 처리
            dragCloth.rect.SetParent(transform, false);
            dragCloth.rect.localPosition = Vector3.zero;
            //dragCloth.rect.anchoredPosition = Vector2.zero;
            dragCloth.enabled = false;

            // 이제 slotIndex만 전달
            bool isFilled = (mission as Laundry).MarkSlotFilled(slotIndex);

            if (isFilled)
            {
                MissionManager.Instance.CompleteMission(playerId, mission.MissionID);
                laundryUI.gameObject.SetActive(false);
            }
        }
        else
        {
            // 오답 복귀
            dragCloth.rect.SetParent(dragArea, false);
            dragCloth.rect.anchoredPosition = Vector2.zero;
        }
    }
}