using System.Collections;
using System.Collections.Generic;
using _02_Scripts.Alert;
using _02_Scripts.Login;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace _02_Scripts.Lobby
{
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        public static NetworkManager Instance;

        public PlayerInfo currentPlayerInfo;
        public Photon.Realtime.Player currentPlayer;

        public void Init()
        {
            Hashtable properties = new Hashtable{
            { "PlayerSeq", LoginSession.loginPlayerInfo.seq},
            { "PlayerID", LoginSession.loginPlayerInfo.id},
            { "Nickname", LoginSession.loginPlayerInfo.name},
            { "PlayerLevel", LoginSession.loginPlayerInfo.level},
            { "PlayerGold", LoginSession.loginPlayerInfo.gold}};
            PhotonNetwork.LocalPlayer.SetCustomProperties(properties);
        }

        private void Awake() // 싱글톤 생성
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 유지 원할 경우
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings();
        }

        void Start() // 포톤네트워크에 연결
        {
            Init();
        }

        public override void OnConnectedToMaster() //마스터에 연결 -> 로비로 진입
        {
            Debug.Log("Connected to Master");
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
                IsVisible = !IsVisible,
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
            GameManager.Instance.ChangeState(GameState.RoleAssignment);
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
            Debug.Log($"RoomList Updated: {roomList.Count}개");
            LobbyUIManager.Instance.UpdateRoomList(roomList);
        }
    }
}