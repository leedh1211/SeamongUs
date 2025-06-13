using UnityEngine;
using UnityEngine.EventSystems;

public class DragCloth : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject clothPrefab;
    public RectTransform rect;
    private CanvasGroup group;
    private Canvas canvas;

    // �巡�� ���� ���� �θ�(DragArea)�� ���� ��ġ ����
    private RectTransform originalParent;
    private Vector2 originalAnchoredPos;

    // ���콺 Ŭ�� ��ġ�� rect �߽� �� ������
    private Vector2 pointerOffset;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        group = gameObject.AddComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 1) ���� ���߰� Raycast ����
        group.alpha = 0.6f;
        group.blocksRaycasts = false;

        // 2) ���� �θ���ġ ����
        originalParent = rect.parent as RectTransform;
        originalAnchoredPos = rect.anchoredPosition;

        // 3) Ŭ�� ������ rect �߽� �� ������ ���
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
        // 1) ���콺 ��ġ�� ĵ���� ���� ��ǥ��
        Vector2 localPointerPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out localPointerPos);

        // 2) ������ �����ؼ� �̹��� ���̱�
        rect.anchoredPosition = localPointerPos - pointerOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 1) ������ ����, Raycast �����
        group.alpha = 1f;
        group.blocksRaycasts = true;

        // 2) ��ӵ� �θ� DropZone�� �ƴ϶�� ����ġ�� ����
        bool droppedOnZone = rect.parent.GetComponent<DropZone>() != null;
        if (!droppedOnZone)
        {
            rect.SetParent(originalParent, worldPositionStays: false);
            rect.anchoredPosition = originalAnchoredPos;
        }
        // ���� DropZone �ʿ��� ���������� �θ� �����ߴٸ�,
        // �״�� �� ��ġ�� �����˴ϴ�.
    }
}