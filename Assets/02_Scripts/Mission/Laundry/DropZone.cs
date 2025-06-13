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
    private LaundryUI laundryUI;  // UI�� �ٷ� ���� �� �ֵ��� ����

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

        // �� ���Կ� �´� prefab
        var expected = mission.GetorderPrefabs()[slotIndex];
        bool isCorrect = dragCloth.clothPrefab == expected;

        if (isCorrect)
        {
            // ���� ó��
            dragCloth.rect.SetParent(transform, false);
            dragCloth.rect.localPosition = Vector3.zero;
            //dragCloth.rect.anchoredPosition = Vector2.zero;
            dragCloth.enabled = false;

            // ���� slotIndex�� ����
            (mission as Laundry).MarkSlotFilled(slotIndex);

            if (mission.IsCompleted)
            {
                MissionManager.Instance.CompleteMission(playerId, mission.MissionID);
                laundryUI.gameObject.SetActive(false);
            }
        }
        else
        {
            // ���� ����
            dragCloth.rect.SetParent(dragArea, false);
            dragCloth.rect.anchoredPosition = Vector2.zero;
        }
    }
}
