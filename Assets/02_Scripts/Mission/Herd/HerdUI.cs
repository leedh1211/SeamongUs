using System.Collections.Generic;
using UnityEngine;

public class HerdUI : MonoBehaviour, IMissionUI
{
    [Header("�� ������ (UI Image)")]
    [SerializeField] private GameObject sheepPrefab;
    [Header("���� �ѷ��� ����")]
    [SerializeField] private RectTransform spawnArea;
    [Header("�� ���� ����")]
    [SerializeField] private RectTransform goalArea;
    [Header("��ֹ� �����̳�")]
    [SerializeField] private RectTransform obstacleContainer;
    [Header("�� ���� ��")]
    [SerializeField] private int sheepCount = 5;
    [Header("�� ���󰡴� �ӵ� (������)")]
    [SerializeField] private float herdSpeed = 0.2f;         
    [Header("��ֹ� ������ (UI Image)")]
    [SerializeField] private GameObject obstaclePrefab;
    [Header("��ֹ� ����")]
    [SerializeField] private int obstacleCount = 5;
    [Header("��ֹ� �ּ� ���� (�ȼ�)")]
    [SerializeField] private float obstacleSpacing = 100f;


    private HerdMission mission;
    private string playerId;
    private List<GameObject> spawnedSheep = new List<GameObject>();
    private List<RectTransform> obstacles = new List<RectTransform>();
    private List<Vector2> obstaclePositions = new List<Vector2>();

    public void Show(Mission missionBase, string playerId)
    {
        if (!(missionBase is HerdMission hm)) return;
        this.mission = hm;
        this.playerId = playerId;
        gameObject.SetActive(true);
        SpawnHerdAndObstacles();
    }

    public void Hide()
    {
        foreach (var go in spawnedSheep) Destroy(go);
        spawnedSheep.Clear();
        foreach (Transform t in obstacleContainer) Destroy(t.gameObject);
        obstacles.Clear(); 
        obstaclePositions.Clear();
        gameObject.SetActive(false);
    }

    private void SpawnHerdAndObstacles()
    {
        Vector2 center = spawnArea.anchoredPosition;
        float halfW = spawnArea.rect.width * 0.5f;
        float halfH = spawnArea.rect.height * 0.5f;

        // 1) �� ��ġ�� �״�� ���ϴܿ�
        {
            var rt = goalArea;
            float gx = center.x + halfW - rt.sizeDelta.x * rt.pivot.x;
            float gy = center.y - halfH + rt.sizeDelta.y * (1 - rt.pivot.y);
            rt.anchoredPosition = new Vector2(gx, gy);
        }

        // 2) ��ֹ� �Ѹ���: �߾� 80% ����, spacing �а�
        obstaclePositions.Clear();
        float obsZoneW = halfW * 0.8f;  // 80%
        float obsZoneH = halfH * 0.8f;
        for (int i = 0; i < obstacleCount; i++)
        {
            Vector2 pos;
            int tries = 0;
            do
            {
                float x = center.x + Random.Range(-obsZoneW, obsZoneW);
                float y = center.y + Random.Range(-obsZoneH, obsZoneH);
                pos = new Vector2(x, y);
                tries++;
            }
            while (AttemptsOverlap(pos) && tries < 20);

            var go = Instantiate(obstaclePrefab, obstacleContainer, false);
            var ort = go.GetComponent<RectTransform>();
            ort.anchoredPosition = pos;
            obstacles.Add(ort);
            obstaclePositions.Add(pos);
        }

        // 3) �� �� ����
        mission.SetTotalSheep(sheepCount);

        // 4) �� �Ѹ���: ���� ���
        float sheepW = sheepPrefab.GetComponent<RectTransform>().sizeDelta.x;
        float separation = sheepW * 0.5f;

        float sheepBaseX = center.x - halfW + (sheepW * 0.5f);
        float sheepY = center.y + halfH - (sheepPrefab.GetComponent<RectTransform>().sizeDelta.y * 0.5f);

        var panelRt = GetComponent<RectTransform>();
        Vector2 panelCenter = panelRt.rect.center;

        for (int i = 0; i < sheepCount; i++)
        {
            float spawnX = sheepBaseX + separation * i;

            var go = Instantiate(sheepPrefab, transform, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(spawnX, sheepY);

            Vector2 offset = rt.anchoredPosition - panelCenter;

            var sc = go.AddComponent<SheepController>();
            sc.Initialize(
                panelRect: GetComponent<RectTransform>(),
                goalArea: goalArea,
                obstacles: obstacles,
                mission: mission,
                playerId: playerId,
                ui: this,
                speed: herdSpeed,  // ���� �ӵ��� ����
                followOffset: offset
            );
            spawnedSheep.Add(go);
        }
    }

    private bool AttemptsOverlap(Vector2 pos)
    {
        foreach (var other in obstaclePositions)
            if (Vector2.Distance(pos, other) < obstacleSpacing)
                return true;
        return false;
    }
}
