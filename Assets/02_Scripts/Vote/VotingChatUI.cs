using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class VotingChatUI : MonoBehaviour
{
    public GameObject chatPanel; // 전체 창
    public ScrollRect scrollRect;
    public Transform messageContent; // Scroll View → Viewport → Content
    public GameObject messagePrefab; // ChatMessageUI 프리팹
    public TMP_InputField inputField;
    public Button sendButton, closeButton;

    private void Start()
    {
        sendButton.onClick.AddListener(OnSendClicked);
        closeButton.onClick.AddListener(() => chatPanel.SetActive(false));
    }

    public void Open()
    {
        chatPanel.SetActive(true);
        inputField.text = "";
        inputField.Select();
        inputField.ActivateInputField();
    }

    void OnSendClicked()
    {
        if (string.IsNullOrWhiteSpace(inputField.text)) return;

        string msg = inputField.text;
        inputField.text = "";

        // 메시지를 서버로 전송 (RPC든 RaiseEvent든 사용 중인 방식에 맞게 구현)
        NetworkChatManager.Instance.SendChat(msg);
    }

    public void ReceiveMessage(string senderId, string nickname, string message, Sprite avatar)
    {
        bool isMine = senderId == PhotonNetwork.LocalPlayer.UserId;

        GameObject msgObj = Instantiate(messagePrefab, messageContent);
        ChatMessageUI ui = msgObj.GetComponent<ChatMessageUI>();
        ui.SetMessage(nickname, message, avatar, isMine);

        // 최신 메시지로 스크롤 이동
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }
}