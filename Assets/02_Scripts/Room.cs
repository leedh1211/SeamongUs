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
            int index = i + 1; // 1 ~ 9
            spriteButtons[i].onClick.AddListener(() =>
            {
                ChangeMySprite("CharacterSprite0" + index);
            });
        }
    }

    public void ChangeMySprite(string spriteName)
    {
        GameObject localPlayerObj = PhotonNetwork.LocalPlayer.TagObject as GameObject;
        if (localPlayerObj == null)
        {
            Debug.LogError("Local player GameObject is null!");
            return;
        }

        PhotonView pv = PhotonView.Get(localPlayerObj);
        if (pv == null)
        {
            Debug.LogError("PhotonView of local player not found!");
            return;
        }

        // 내 PhotonView에 RPC 호출 -> 네트워크상의 모든 클라이언트에 동기화됨
        pv.RPC("SetSprite", RpcTarget.AllBuffered, spriteName);
    }

    void SetupUIByHost()
    {
        bool isHost = PhotonNetwork.IsMasterClient;
        startButton.gameObject.SetActive(isHost);
        readyButton.gameObject.SetActive(!isHost);
    }

    void RefreshPlayerSlots()
    {
        //foreach (Transform child in playerSlotContainer)
        //{
        //    Destroy(child.gameObject);
        //}

        //playerSlots.Clear();

        //foreach (Player p in PhotonNetwork.PlayerList)
        //{
        //    GameObject slot = Instantiate(playerSlotPrefab, playerSlotContainer);
        //    slot.GetComponentInChildren<TextMeshProUGUI>().text = p.NickName + (p.IsMasterClient ? " (Host)" : "");
        //    playerSlots[p.ActorNumber] = slot;
        //}
    }

    void OnClickReady()
    {
        bool currentReady = GetIsReady(); // 현재 상태
        bool nextReady = !currentReady;   // 토글된 상태

        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable { { "IsReady", nextReady } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        // UI 업데이트 예시
        string buttonText = nextReady ? "Wait" : "Ready";
        readyButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = buttonText;
    }
    bool GetIsReady()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("IsReady", out object ready))
        {
            Debug.Log($"IsReady for {PhotonNetwork.LocalPlayer.NickName}: {ready}");
            return (bool)ready;
        }
        return false;
    }
    bool IsAllPlayersReady()
    {
        Debug.Log("count - "+PhotonNetwork.PlayerList.Length);
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                p.CustomProperties.TryGetValue("Nickname", out object playerName);
                Debug.Log($"NickName: {playerName}");
                if (!p.CustomProperties.TryGetValue("IsReady", out object isReady) || !(isReady is bool ready && ready))
                {
                    if (playerName is string)
                    {
                        Debug.LogWarning($"Player {playerName} is not ready.");    
                    }
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
            Debug.Log("All players ready. Starting game...");
            PhotonNetwork.RaiseEvent(
                eventCode: EventCodes.ChangeState, // 커스텀 코드. 100번 예시
                eventContent: null,
                raiseEventOptions: new RaiseEventOptions { Receivers = ReceiverGroup.All },
                sendOptions: SendOptions.SendReliable
            );
        }
        else
        {
            Debug.LogWarning("Not all players are ready.");
        }
    }


    public void OnChatSubmit(string message)
    {
        if (!string.IsNullOrWhiteSpace(message))
        {
            Debug.Log($"[Chat] Submitted message: {message}");
            photonView.RPC("ReceiveChatMessage", RpcTarget.All, PhotonNetwork.NickName, message);
            chatInput.text = "";
        }
    }

    [PunRPC]
    void ReceiveChatMessage(string sender, string message)
    {
        Debug.Log($"[RPC] {sender}: {message}");
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
        yield return null; // 한 프레임 대기
        scrollRect.verticalNormalizedPosition = 0f;
    }

    public void UpdateCharacterSelection(string selectedCharacter)
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable { { "SelectedCharacter", selectedCharacter } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
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
            case EventCodes.ChangeState: // Start Game State 변경 이벤트
                GameManager.Instance.ChangeState(GameState.RoleAssignment); // 각 클라이언트에서 실행됨
                break;
        }
    }
}
