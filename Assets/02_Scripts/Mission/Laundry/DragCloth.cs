using UnityEngine;
using UnityEngine.EventSystems;

public class DragCloth : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject clothPrefab;
    public RectTransform rect;
    private CanvasGroup group;
    private Canvas canvas;

    // 드래그 시작 시의 부모(DragArea)와 로컬 위치 저장
    private RectTransform originalParent;
    private Vector2 originalAnchoredPos;

    // 마우스 클릭 위치와 rect 중심 간 오프셋
    private Vector2 pointerOffset;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        group = gameObject.AddComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 1) 투명도 낮추고 Raycast 차단
        group.alpha = 0.6f;
        group.blocksRaycasts = false;

        // 2) 시작 부모·위치 저장
        originalParent = rect.parent as RectTransform;
        originalAnchoredPos = rect.anchoredPosition;

        // 3) 클릭 지점과 rect 중심 간 오프셋 계산
        Vector2 localPointerPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out localPointerPos);
        pointerOffset = localPointerPos - rect.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 1) 마우스 위치를 캔버스 로컬 좌표로
        Vector2 localPointerPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out localPointerPos);

        // 2) 오프셋 보정해서 이미지 붙이기
        rect.anchoredPosition = localPointerPos - pointerOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 1) 불투명 복귀, Raycast 재허용
        group.alpha = 1f;
        group.blocksRaycasts = true;

        // 2) 드롭된 부모가 DropZone이 아니라면 원위치로 복귀
        bool droppedOnZone = rect.parent.GetComponent<DropZone>() != null;
        if (!droppedOnZone)
        {
            rect.SetParent(originalParent, worldPositionStays: false);
            rect.anchoredPosition = originalAnchoredPos;
        }
        // 만약 DropZone 쪽에서 성공적으로 부모 변경했다면,
        // 그대로 그 위치에 고정됩니다.
    }
}