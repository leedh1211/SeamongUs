using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LaundryUI : MonoBehaviour
{
    [Header("정답지")]
    [SerializeField] private List<Transform> referenceSlots;
    [Header("빨래집게")]
    [SerializeField] private List<Transform> clampSlots;
    [Header("옷더미")]
    [SerializeField] private GameObject dragClothPrefab;
    [Header("드래그 영역")]
    [SerializeField] private RectTransform dragArea;

    private Laundry mission;
    private string playerId;

    public void Initialize(Laundry mission, string playerId)
    {
        this.mission = mission;
        this.playerId = playerId;
        RefreshUI();
    }

    public void RefreshUI()
    {
        foreach (Transform slot in referenceSlots)
        {
            for (int i = slot.childCount - 1; i >= 0; i--)
            {
                Destroy(slot.GetChild(i).gameObject);
            }
        }

        foreach (Transform slot in clampSlots)
        {
            for (int i = slot.childCount - 1; i >= 0; i--)
            {
                Destroy(slot.GetChild(i).gameObject);
            }
        }

        for ( int i = transform.childCount -1; i >= 0; i --)
        {
            var child = transform.GetChild(i);
            if(child.CompareTag("DragIcon"))
            {
                Destroy(child.gameObject);
            }
        }
        for (int i = dragArea.childCount - 1; i >= 0; i--)
        {
            Destroy(dragArea.GetChild(i).gameObject);
        }

        //답안지
        var order = mission.GetorderPrefabs();
        for (int i = 0; i < referenceSlots.Count && i < order.Count; i++)
        {
            Instantiate(order[i], referenceSlots[i].position, Quaternion.identity, referenceSlots[i]);
        }

        //빨래집게들
        for (int i = 0; i < clampSlots.Count && i < order.Count; i++)
        {
            var zone = clampSlots[i].GetComponent<DropZone>();
            zone.Initialize(mission, playerId);
            clampSlots[i].GetComponent<Image>().color = Color.clear;
        }

        // 오른쪽 드래그 아이콘 생성
        var allPrefabs = Resources.LoadAll<GameObject>("ClothPrefabs");
        foreach (var prefab in allPrefabs)
        {
            // 1) DragArea를 부모로, worldPositionStays=false 로 생성
            var iconGO = Instantiate(dragClothPrefab, dragArea, false);

            // 2) DragCloth 스크립트에 원본 프리팹 정보 할당
            var drag = iconGO.GetComponent<DragCloth>();
            drag.clothPrefab = prefab;

            // 3) Image 구성 요소에 스프라이트 할당
            var uiImage = iconGO.GetComponent<Image>();
            var sr = prefab.GetComponent<SpriteRenderer>();
            if (sr != null)
                uiImage.sprite = sr.sprite;
        }
    }

}
