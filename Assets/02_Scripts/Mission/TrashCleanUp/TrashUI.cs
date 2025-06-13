using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashUI : MonoBehaviour, IMissionUI
{
    [Header("������ ������")]
    [SerializeField] private GameObject trashPrefab;
    [Header("������ ���� ���� (RectTransform)")]
    [SerializeField] private RectTransform spawnArea;
    [Header("������ ���� ����")]
    [SerializeField] private int spawnCount;


    private TrashCleanup mission;
    private string playerId;
    private List<GameObject> spawned = new List<GameObject>();

    public void Show(Mission missionBase, string playerId)
    {
        if (!(missionBase is TrashCleanup tc)) return;
        this.mission = tc;
        this.playerId = playerId;

        gameObject.SetActive(true);

        SpawnTrashRandomly();
    }

    private void SpawnTrashRandomly()
    {
        // ���� ������ ����
        foreach (var go in spawned)
            if (go) Destroy(go);
        spawned.Clear();

    
        mission.SetTotalTrash(spawnCount);

        // ���� ���� ũ��
        var area = spawnArea.rect;
        float halfW = area.width * 0.5f;
        float halfH = area.height * 0.5f;

        for (int i = 0; i < spawnCount; i++)
        {
            // 1) �θ�� spawnArea, worldPositionStays=false
            var go = Instantiate(trashPrefab, spawnArea, false);
            var rt = go.GetComponent<RectTransform>();

            // 2) ���� ��ġ: pivot�� (0.5,0.5)���
            float x = Random.Range(-halfW, halfW);
            float y = Random.Range(-halfH, halfH);
            rt.anchoredPosition = new Vector2(x, y); // rt�� anchoredPosition�� �θ��� anchor �������� ���� �Ǻ��� �󸶳� �������ִ��� �������ִº���.

            // 3) Ŭ�� �� ������ ���� ������Ʈ ����
            var ti = go.GetComponent<TrashItem>() ?? go.AddComponent<TrashItem>();
            ti.Initialize(mission, playerId, this);

            spawned.Add(go);
        }
    }

    public void Hide()
    {
        foreach (var go in spawned)
        {
            if (go)
            {
                Destroy(go);
            }          
        }
        spawned.Clear();
        gameObject.SetActive(false);
    }
  

}
