using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using ExitGames.Client.Photon;

public class Room : MonoBehaviourPunCallbacks, IOnEventCallback
{
    [Header("UI References")]
    public Button startButton;
    public Button readyButton;
    public TextMeshProUGUI chatContent;
    public TMP_InputField chatInput;
    public Transform playerSlotContainer;
    public GameObject playerSlotPrefab;

    private Dictionary<int, GameObject> playerSlots = new Dictionary<int, GameObject>();

    [SerializeField] private Button[] spriteButtons; // 9개 버튼 배열

    void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    void Start()
    {
        SetupUIByHost();
        RefreshPlayerSlots();

        readyButton.onClick.AddListener(OnClickReady);
        startButton.onClick.AddListener(OnClickStart);
        chatInput.onSubmit.AddListener(OnChatSubmit);

        // 각 버튼에 클릭 이벤트 등록
        for (int i = 0; i < spriteButtons.Length; i++)
        {
            int index = i; // 0 ~ 8
            spriteButtons[i].onClick.AddListener(() =>
            {
                ChangeMySprite(index);
            });
        }
    }

    public void ChangeMySprite(int avatarIndex)
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            { PlayerPropKey.Spr, avatarIndex }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    void SetupUIByHost()
    {
        bool isHost = PhotonNetwork.IsMasterClient;
        startButton.gameObject.SetActive(isHost);
        readyButton.gameObject.SetActive(!isHost);
    }

    void RefreshPlayerSlots()
    {
        // 필요한 경우 플레이어 슬롯 UI 업데이트 코드 작성
    }

    void OnClickReady()
    {
        bool currentReady = GetIsReady();
        bool nextReady = !currentReady;

        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable { { "IsReady", nextReady } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        string buttonText = nextReady ? "Wait" : "Ready";
        readyButton.GetComponentInChildren<TextMeshProUGUI>().text = buttonText;
    }

    bool GetIsReady()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(PlayerPropKey.IsReady, out object ready))
        {
            return (bool)ready;
        }
        return false;
    }

    bool IsAllPlayersReady()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if (!p.CustomProperties.TryGetValue(PlayerPropKey.IsReady, out object isReady) || !(isReady is bool ready && ready))
                {
                    return false;
                }
            }
        }
        return true;
    }

    void OnClickStart()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (IsAllPlayersReady())
        {
            PhotonNetwork.RaiseEvent(
                eventCode: EventCodes.ChangeState,
                eventContent: null,
                raiseEventOptions: new RaiseEventOptions { Receivers = ReceiverGroup.All },
                sendOptions: SendOptions.SendReliable
            );
        }
    }

    public void OnChatSubmit(string message)
    {
        if (!string.IsNullOrWhiteSpace(message))
        {
            photonView.RPC("ReceiveChatMessage", RpcTarget.All, PhotonNetwork.NickName, message);
            chatInput.text = "";
        }
    }

    [PunRPC]
    void ReceiveChatMessage(string sender, string message)
    {
        AddChatMessage(sender, message);
    }

    [SerializeField] private Transform chatContentParent;
    [SerializeField] private GameObject chatMessagePrefab;
    [SerializeField] private ScrollRect scrollRect;

    public void AddChatMessage(string sender, string message)
    {
        GameObject chatObj = Instantiate(chatMessagePrefab, chatContentParent);
        TextMeshProUGUI text = chatObj.GetComponent<TextMeshProUGUI>();
        text.text = $"<b>{sender}:</b> {message}";
        StartCoroutine(ScrollToBottom());
    }

    private IEnumerator ScrollToBottom()
    {
        yield return null;
        scrollRect.verticalNormalizedPosition = 0f;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RefreshPlayerSlots();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RefreshPlayerSlots();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        RefreshPlayerSlots();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        SetupUIByHost();
        RefreshPlayerSlots();
    }

    public void OnEvent(EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case EventCodes.ChangeState:
                GameManager.Instance.ChangeState(GameState.RoleAssignment);
                break;
        }
    }
}
