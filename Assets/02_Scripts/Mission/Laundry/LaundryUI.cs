using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LaundryUI : MonoBehaviour
{
    [Header("������")]
    [SerializeField] private List<Transform> referenceSlots;
    [Header("��������")]
    [SerializeField] private List<Transform> clampSlots;
    [Header("�ʴ���")]
    [SerializeField] private GameObject dragClothPrefab;

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
        //�����
        var order = mission.GetorderPrefabs();
        for (int i = 0; i < referenceSlots.Count && i < order.Count; i++)
        {
            Instantiate(order[i], referenceSlots[i].position, Quaternion.identity, referenceSlots[i]);
        }

        //�������Ե�
        for (int i = 0; i < clampSlots.Count && i < order.Count; i++)
        {
            var zone = clampSlots[i].GetComponent<DropZone>();
            zone.Initialize(mission, playerId);
            clampSlots[i].GetComponent<Image>().color = Color.clear;
        }

        // ������ �巡�� ������ ����
        var allPrefabs = Resources.LoadAll<GameObject>("ClothPrefabs");
        foreach (var prefab in allPrefabs)
        {
            var icon = Instantiate(dragClothPrefab, transform);
            icon.tag = "DragIcon";
            var drag = icon.GetComponent<DragCloth>();
            drag.clothPrefab = prefab;
        }
    }

}
