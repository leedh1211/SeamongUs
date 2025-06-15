using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VoteUISlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image spriteBG;
    [SerializeField] private TextMeshProUGUI isDeadText;
    [SerializeField] private Button voteButton;

    private string targetPlayerId;

    // 슬롯 초기화
    public void Init(string playerId, string playerName, string localPlayerId)
    {
        targetPlayerId = playerId;
        nameText.text = playerName;

        // 본인 투표 X
        bool canVote = playerId != localPlayerId;
        voteButton.interactable = canVote;

        // 사망 Text 표시
        isDeadText.gameObject.SetActive(false);

        voteButton.onClick.AddListener(OnVoteClicked);
    }

    private void OnVoteClicked()
    {
        string localPlayerId = Photon.Pun.PhotonNetwork.LocalPlayer.ActorNumber.ToString();
        VoteManager.Instance.SubmitVote(localPlayerId, targetPlayerId);

        voteButton.interactable = false; // 투표 후 버튼 비활성화
    }
}
