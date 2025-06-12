using System.Collections.Generic;
using _02_Scripts.Alert;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace _02_Scripts.Lobby
{
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        public static NetworkManager Instance;

        private void Awake() // 싱글톤 생성
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 유지 원할 경우
        }

        void Start() // 포톤네트워크에 연결
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster() //마스터에 연결 -> 로비로 진입
        {
            PhotonNetwork.JoinLobby();
        }
        
        public void JoinRoom(string roomName) // 방으로 들어가기
        {
            PhotonNetwork.JoinRoom(roomName);
        }

        public void MakeRoom(string roomName, int MaxPlayers, bool IsVisible) // 방만들기
        {
            RoomOptions roomOptions = new RoomOptions()
            {
                MaxPlayers = MaxPlayers,
                IsVisible = IsVisible,
                IsOpen = true
            };
            
            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }

        public override void OnCreatedRoom()
        {
            Debug.Log("방이 생성되었습니다.");
        }
        
        public override void OnJoinedRoom()
        {
            Debug.Log("방에 입장했습니다.");
            // UI 전환 또는 캐릭터 생성 등
        }
        
        public override void OnCreateRoomFailed(short returnCode, string message) // 방 생성실패 콜백
        {
            AlertUIManager.Instance.OnAlert(message);
        }
        
        public override void OnJoinRoomFailed(short returnCode, string message) // 방 진입 실패 롤백
        {
            AlertUIManager.Instance.OnAlert(message);
        }

        public void LeaveRoom()  // 방 나가기
        {
            PhotonNetwork.LeaveRoom();
        }

        public override void OnLeftRoom() // 방 나간 후 콜백
        {
            PhotonNetwork.JoinLobby();
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList) // 방 리스트 업데이트 함수
        {
            LobbyUIManager.Instance.UpdateRoomList(roomList);
        }
    }
}