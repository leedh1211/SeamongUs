using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class VoteCountUI : MonoBehaviour
{
    public static VoteCountUI Instance { get; private set; }
    [SerializeField] private GameObject panel;
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject countSlotPrefab;
    [SerializeField] private float interval = 0.5f;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    // voteResults = 투표자 ID -> 받은 투표수
    // ejected = 추방 대상 ActorNum
    public void ShowCount(Dictionary<int, int> voteResults)
    {
        panel.SetActive(true);

        // 이전에 생성된 슬롯들 삭제
        foreach (Transform t in contentParent) Destroy(t.gameObject);

        // 슬롯 생성 & 맵에 저장
        var slotMap = new Dictionary<int, CountSlot>();
        foreach (var kv in voteResults)
        {
            var go = Instantiate(countSlotPrefab, contentParent);
            var slot = go.GetComponent<CountSlot>();
            slot.SetLabel($"{kv.Key}번 플레이어");
            slotMap[kv.Key] = slot;
        }

        StartCoroutine(PlayCountSequence(slotMap, voteResults));
    }

    private IEnumerator PlayCountSequence(
        Dictionary<int, CountSlot> slotMap,
        Dictionary<int, int> voteResults
    )
    {
        int maxVotes = voteResults.Values.DefaultIfEmpty().Max();

        // 1표씩 차오르는 애니메이션
        for (int round = 1; round <= maxVotes; round++)
        {
            foreach (var kv in voteResults.Where(x => x.Value >= round))
                slotMap[kv.Key].AddMark();
            yield return new WaitForSeconds(interval);
        }

        // 애니 끝나면 패널만 닫는다
        panel.SetActive(false);
    }
}