using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashUI : MonoBehaviour, IMissionUI
{
    [Header("쓰레기 프리팹")]
    [SerializeField] private GameObject trashPrefab;
    [Header("쓰레기 스폰 영역 (RectTransform)")]
    [SerializeField] private RectTransform spawnArea;
    [Header("쓰레기 스폰 갯수")]
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
        // 기존 아이템 정리
        foreach (var go in spawned)
            if (go) Destroy(go);
        spawned.Clear();

    
        mission.SetTotalTrash(spawnCount);

        // 스폰 영역 크기
        var area = spawnArea.rect;
        float halfW = area.width * 0.5f;
        float halfH = area.height * 0.5f;

        for (int i = 0; i < spawnCount; i++)
        {
            // 1) 부모는 spawnArea, worldPositionStays=false
            var go = Instantiate(trashPrefab, spawnArea, false);
            var rt = go.GetComponent<RectTransform>();

            // 2) 랜덤 위치: pivot이 (0.5,0.5)라면
            float x = Random.Range(-halfW, halfW);
            float y = Random.Range(-halfH, halfH);
            rt.anchoredPosition = new Vector2(x, y); // rt의 anchoredPosition은 부모의 anchor 기준으로 본인 피봇이 얼마나 떨어져있는지 설정해주는변수.

            // 3) 클릭 시 동작을 위한 컴포넌트 세팅
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
