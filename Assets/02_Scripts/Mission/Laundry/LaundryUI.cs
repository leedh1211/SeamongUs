using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LaundryUI : MonoBehaviour, IMissionUI
{
    [Header("������")]
    [SerializeField] private List<Transform> referenceSlots;
    [Header("��������")]
    [SerializeField] private List<Transform> clampSlots;
    [Header("�ʴ���")]
    [SerializeField] private GameObject dragClothPrefab;
    [Header("�巡�� ����")]
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
            zone.Initialize(mission, playerId, dragArea, this);  // dragArea �߰�
                                                           // �ð�ȭ ���� ���������� �ٲ� �μ���
            clampSlots[i].GetComponent<Image>().color = new Color(1, 1, 1, 0.2f);
        }

        // ������ �巡�� ������ ����
        var allPrefabs = Resources.LoadAll<GameObject>("ClothPrefabs");
        foreach (var prefab in allPrefabs)
        {
            var iconGO = Instantiate(dragClothPrefab, dragArea, false);
            var drag = iconGO.GetComponent<DragCloth>();
            drag.clothPrefab = prefab;

            // --- ��������Ʈ �������� ���� ���� ---
            Sprite sprite = null;
            // 1) ���� SpriteRenderer ã�ƺ���
            var sr = prefab.GetComponent<SpriteRenderer>();
            if (sr != null)
                sprite = sr.sprite;
            else
            {
                // 2) �ڽĿ� ���� ���� ������ InChildren����
                sr = prefab.GetComponentInChildren<SpriteRenderer>();
                if (sr != null)
                    sprite = sr.sprite;
                else
                {
                    // 3) UI Image�� ����ִٸ�
                    var img = prefab.GetComponent<Image>();
                    if (img != null)
                        sprite = img.sprite;
                }
            }

            // 4) ���� �Ҵ�
            var uiImage = iconGO.GetComponent<Image>();
            if (sprite != null)
                uiImage.sprite = sprite;
            else
                Debug.LogWarning($"[{prefab.name}]���� Sprite�� �� ã�ҽ��ϴ�.");
        }
    }

    public void Show(Mission missionBase, string playerId)
    {
        // Mission Ÿ�� Ȯ��
        if (missionBase is Laundry laundry)
        {
            Debug.Log(">> MissionCollider.Show ȣ��");
            gameObject.SetActive(true);
            Initialize(laundry, playerId);
        }

    }
}
