using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragCloth : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject clothPrefab;
    public RectTransform rect;
    private CanvasGroup group;
    private Canvas canvas;


    void Awake()
    {
        rect = GetComponent<RectTransform>();
        group = gameObject.AddComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        group.alpha = 0.6f;
        group.blocksRaycasts = false;
    
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out pos);
        rect.anchoredPosition = pos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        group.alpha = 1f;
        group.blocksRaycasts = true;
    }
}
