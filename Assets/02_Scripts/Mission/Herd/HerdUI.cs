using System.Collections.Generic;
using UnityEngine;

public class HerdUI : MonoBehaviour, IMissionUI
{
    [Header("양 프리팹 (UI Image)")]
    [SerializeField] private GameObject sheepPrefab;
    [Header("양이 뿌려질 영역")]
    [SerializeField] private RectTransform spawnArea;
    [Header("골 지점 영역")]
    [SerializeField] private RectTransform goalArea;
    [Header("장애물 컨테이너")]
    [SerializeField] private RectTransform obstacleContainer;
    [Header("양 마리 수")]
    [SerializeField] private int sheepCount = 5;
    [Header("양 따라가는 속도 (느리게)")]
    [SerializeField] private float herdSpeed = 0.2f;         
    [Header("장애물 프리팹 (UI Image)")]
    [SerializeField] private GameObject obstaclePrefab;
    [Header("장애물 개수")]
    [SerializeField] private int obstacleCount = 5;
    [Header("장애물 최소 간격 (픽셀)")]
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

        // 1) 골 위치는 그대로 우하단에
        {
            var rt = goalArea;
            float gx = center.x + halfW - rt.sizeDelta.x * rt.pivot.x;
            float gy = center.y - halfH + rt.sizeDelta.y * (1 - rt.pivot.y);
            rt.anchoredPosition = new Vector2(gx, gy);
        }

        // 2) 장애물 뿌리기: 중앙 80% 구역, spacing 넓게
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

        // 3) 양 수 설정
        mission.SetTotalSheep(sheepCount);

        // 4) 양 뿌리기: 왼쪽 상단
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
                speed: herdSpeed,  // 느린 속도로 전달
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
