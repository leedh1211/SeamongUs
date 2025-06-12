using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace _02_Scripts.Lobby
{
    public class RoomSlot : MonoBehaviour
    {
        [SerializeField] private TMP_Text roomNameText;
        [SerializeField] private TMP_Text playerCountText;
        private string roomName;

        public void Init(RoomInfo info)
        {
            roomName = info.Name;
            roomNameText.text = roomName;
            playerCountText.text = $"{info.PlayerCount} / {info.MaxPlayers}";
        }

        public void OnClickJoinButton()
        {
            NetworkManager.Instance.JoinRoom(roomName);
        }
    }
}