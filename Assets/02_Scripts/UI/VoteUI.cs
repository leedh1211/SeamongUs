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
        StartCoroutine(StartTimer());
        PopulateSlots();
    }

    private void OnDisable()
    {
        // 다음에 켜질 때 UI 초기화
        foreach (Transform transform in contentParent)
            Destroy(transform.gameObject);
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

    private void PopulateSlots()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            var gameObject = Instantiate(slotTemplate, contentParent);
            gameObject.SetActive(true);

            var slot = gameObject.GetComponent<VoteUISlot>();
            slot.Init
                (
                    player.ActorNumber.ToString(),
                    player.NickName,
                    PhotonNetwork.LocalPlayer.ActorNumber.ToString()
                );
        }
    }
}
