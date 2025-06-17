using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
///   채팅 메시지를 Photon RPC로 주고받고,
///   수신 시 상대의 닉네임·아바타를 Custom-Property에서 꺼내 UI에 넘겨준다.
/// </summary>
[RequireComponent(typeof(PhotonView))]
public class NetworkChatManager : MonoBehaviourPun
{
    public static NetworkChatManager Instance;

    [Header("Inspector References")] public VotingChatUI chatUI; // 채팅창 관리 스크립트
    public Sprite defaultAvatar; // 스프라이트 못 찾을 때 대체용

    /*──────────────────────────────  싱글톤  */
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    private void Start()
    {
        // 개발 중 혼자 돌릴 땐 이걸 추가
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.OfflineMode = true;
            PhotonNetwork.NickName = "TestUser";
            Debug.Log("오프라인 모드로 전환됨");

            RoomOptions options = new RoomOptions { MaxPlayers = 1 };
            PhotonNetwork.CreateRoom("OfflineTestRoom", options);
        }
    }

    /*──────────────────────────────  전송  */
    public void SendChat(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return;
        // → 모든 클라이언트에게 메시지 문자열 하나만 전파
        photonView.RPC(nameof(ReceiveChatRPC), RpcTarget.All, message);
    }

    /*──────────────────────────────  수신  */
    [PunRPC]
    private void ReceiveChatRPC(string message, PhotonMessageInfo info)
    {
        Photon.Realtime.Player sender = info.Sender; // 보낸 플레이어
        string nickname = sender.NickName; // 닉네임

        /* 아바타 스프라이트 인덱스 가져오기  */
        Sprite avatar = defaultAvatar;
        if (sender.CustomProperties != null &&
            sender.CustomProperties.TryGetValue(PlayerPropKey.Spr, out var sprObj))
        {
            int idx = (int)sprObj;
            if (AvatarManager.Instance != null)
                avatar = AvatarManager.Instance.GetSprite(idx); // 인덱스 → 스프라이트
        }

        /* UI로 전달 */
        chatUI.ReceiveMessage(sender.UserId, nickname, message, avatar);
        // chatUI 내부에서  senderId == LocalPlayer.UserId  → isMine 계산
    }
}