using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class VoteResultUI : MonoBehaviour
{
    public static VoteResultUI Instance { get; private set; }
    [SerializeField] private GameObject panel;
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject resultSlotPrefab;
    [SerializeField] private TextMeshProUGUI ejectedText;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    // voteResults = 투표자 ID -> 받은 투표수
    // ejected = 추방 대상 ActorNum
    public void ShowResult(Dictionary<int, int> voteResults, int ejected)
    {
        panel.SetActive(true);

        // 슬롯초기화
        foreach (Transform transform in contentParent)
            Destroy(transform.gameObject);

        // 각 플레이어별 득표수 표시
        var slotMap = new Dictionary<int, ResultSlot>();
        foreach (var pair in voteResults)
        {
            var gameObject = Instantiate(resultSlotPrefab, contentParent);
            var slot = gameObject.GetComponent<ResultSlot>();
            slot.SetLabel($"{pair.Key}번 플레이어"); // 플레이어 ID 표시
            slotMap[pair.Key] = slot;
        }

        StartCoroutine(AutoPanel(slotMap, voteResults, ejected));
    }

    IEnumerator AutoPanel(Dictionary<int,ResultSlot> slotMap, Dictionary<int,int> voteResults, int ejected)
    {
        int maxVotes = voteResults.Values.Max();

        // 1~maxVotes 개표 턴마다 득표자 votedMarkPrefab 하나씩 추가
        for (int round = 1; round <= maxVotes; round++)
        {
            foreach (var kv in voteResults.Where(x => x.Value >= round))
            {
                slotMap[kv.Key].AddBubble();
            }
            yield return new WaitForSeconds(0.5f);
        }

        // 추방 대상 텍스트
        ejectedText.text = (ejected >= 0)
          ? $"{ejected}님이 추방당했습니다."
          : "추방 대상 없음";

        yield return new WaitForSeconds(3f);

        panel.SetActive(false);
        GameManager.Instance.ChangeState(GameState.Playing);
    }
}
