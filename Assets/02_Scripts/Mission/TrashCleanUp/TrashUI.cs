using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashUI : MonoBehaviour, IMissionUI
{
    [Header("쓰레기 프리팹")]
    [SerializeField] private GameObject trashPrefab;
    [Header("쓰레기 스폰 위치들")]
    [SerializeField] private List<Transform> spawnPoints;

    private TrashCleanup mission;
    private string playerId;
    private List<GameObject> spawned = new List<GameObject>();

    public void Show(Mission missionBase, string playerId)
    {
        if (!(missionBase is TrashCleanup tc)) return;
        this.mission = tc;
        this.playerId = playerId;

        gameObject.SetActive(true);

        SpawnTrash();
    }

    private void SpawnTrash()
    {
        foreach (var go in spawned)
        {
            if (go)
            {
                Destroy(go);
            }
        }
        spawned.Clear();

        int count = spawnPoints.Count;
        mission.SetTotalTrash(count);

        foreach(var pt in spawnPoints)
        {
            var go = Instantiate(trashPrefab, pt.position, Quaternion.identity);
            var ti = go.AddComponent<TrashItem>();
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
