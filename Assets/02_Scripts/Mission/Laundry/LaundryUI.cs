using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LaundryUI : MonoBehaviour, IMissionUI
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
            zone.Initialize(mission, playerId, dragArea, this);  // dragArea 추가
                                                           // 시각화 색은 반투명으로 바꿔 두세요
            clampSlots[i].GetComponent<Image>().color = new Color(1, 1, 1, 0.2f);
        }

        // 오른쪽 드래그 아이콘 생성
        var allPrefabs = Resources.LoadAll<GameObject>("ClothPrefabs");
        foreach (var prefab in allPrefabs)
        {
            var iconGO = Instantiate(dragClothPrefab, dragArea, false);
            var drag = iconGO.GetComponent<DragCloth>();
            drag.clothPrefab = prefab;

            // --- 스프라이트 가져오는 로직 보강 ---
            Sprite sprite = null;
            // 1) 먼저 SpriteRenderer 찾아보기
            var sr = prefab.GetComponent<SpriteRenderer>();
            if (sr != null)
                sprite = sr.sprite;
            else
            {
                // 2) 자식에 있을 수도 있으니 InChildren으로
                sr = prefab.GetComponentInChildren<SpriteRenderer>();
                if (sr != null)
                    sprite = sr.sprite;
                else
                {
                    // 3) UI Image에 들어있다면
                    var img = prefab.GetComponent<Image>();
                    if (img != null)
                        sprite = img.sprite;
                }
            }

            // 4) 최종 할당
            var uiImage = iconGO.GetComponent<Image>();
            if (sprite != null)
                uiImage.sprite = sprite;
            else
                Debug.LogWarning($"[{prefab.name}]에서 Sprite를 못 찾았습니다.");
        }
    }

    public void Show(Mission missionBase, string playerId)
    {
        // Mission 타입 확인
        if (missionBase is Laundry laundry)
        {
            Debug.Log(">> MissionCollider.Show 호출");
            gameObject.SetActive(true);
            Initialize(laundry, playerId);
        }

    }
}
