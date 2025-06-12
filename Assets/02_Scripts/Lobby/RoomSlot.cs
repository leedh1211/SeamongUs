using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace _02_Scripts.Lobby
{
    public class RoomSlot : MonoBehaviour
    {
        [SerializeField] private TMP_Text roomNameText;
        [SerializeField] private TMP_Text playerCountText;
        private RoomInfo roomInfo;

        public void Init(RoomInfo info)
        {
            roomInfo = info;
            roomNameText.text = roomInfo.Name;
            playerCountText.text = $"{info.PlayerCount} / {info.MaxPlayers}";
        }

        public void OnClickJoinButton()
        {
            NetworkManager.Instance.JoinRoom(roomInfo.Name);
        }
    }
}