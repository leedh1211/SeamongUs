using System.Collections;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class VoteUI : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject slotTemplate;

    private void OnEnable()
    {
        // 타이머, 슬롯 초기화
        // StartCoroutine(StartTimer());
        PopulateSlots();
    }

    private void OnDisable()
    {
        // 다음에 켜질 때 UI 초기화
        foreach (var slot in contentParent.GetComponentsInChildren<VoteUISlot>())
            slot.gameObject.SetActive(false);
    }

    private IEnumerator StartTimer()
    {
        float time = VoteManager.Instance.VoteTime;
        while (time > 0)
        {
            timeText.text = $"{Mathf.CeilToInt(time)}초 남았습니다...";
            yield return new WaitForSeconds(1f);
            time -= 1f;
        }
    }

    public void UpdateTimerUI(int currentVoteTime)
    {
        timeText.text = $"{Mathf.CeilToInt(currentVoteTime)}초 남았습니다...";
    }

    // contentParent에 배치된 자식 슬롯들 사용으로 변경
    private void PopulateSlots()
    {
        var slots = contentParent.GetComponentsInChildren<VoteUISlot>(includeInactive: true);
        var players = Photon.Pun.PhotonNetwork.PlayerList;
        int count = Mathf.Min(slots.Length, players.Length);

        for (int i = 0; i < count; i++)
        {
            var slot = slots[i];
            slot.gameObject.SetActive(true);

            var player = players[i];

            slot.Init
                (
                player.ActorNumber, // 플레이어 ID
                player.NickName, // 플레이어 이름
                Photon.Pun.PhotonNetwork.LocalPlayer.ActorNumber // 로컬 플레이어 ID
                );
        }
    }
}
