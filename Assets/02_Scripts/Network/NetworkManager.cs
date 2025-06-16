using System.Collections;
using System.Collections.Generic;
using _02_Scripts.Alert;
using _02_Scripts.Login;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun.UtilityScripts;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace _02_Scripts.Lobby
{
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        public static NetworkManager Instance;
        
        public Photon.Realtime.Player currentPlayer;

        public void Init()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.AuthValues = new AuthenticationValues
            {
                UserId = LoginSession.loginPlayerInfo.id
            };
            PhotonNetwork.NickName = LoginSession.loginPlayerInfo.name;
            PhotonNetwork.ConnectUsingSettings();
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

            // ① loginPlayerInfo 같은 커스텀 클래스를 통째로 넣지 않는다.
            // ② Sprite, Color, List 같은 복잡한 타입도 넣지 않는다.
            // ③ Photon이 기본 지원하는 타입( string, int, bool, byte … )만 넣는다.
            Hashtable props = new Hashtable
            {
                { PlayerPropKey.Seq  , LoginSession.loginPlayerInfo.seq   }, // int
                { PlayerPropKey.Id   , LoginSession.loginPlayerInfo.id    }, // string
                { PlayerPropKey.Nick , LoginSession.loginPlayerInfo.name  }, // string
                { PlayerPropKey.Level, LoginSession.loginPlayerInfo.level }, // int
                { PlayerPropKey.Gold , LoginSession.loginPlayerInfo.gold  }, // int
                { PlayerPropKey.Spr  , 0               }, // int (스프라이트 인덱스만)
                { PlayerPropKey.IsDead , false           }, // bool
                { PlayerPropKey.Role , (byte)0         },  // byte
                { PlayerPropKey.IsReady , false         },  // bool
                { PlayerPropKey.HP , 100         },  // bool
                { PlayerPropKey.Stamina , 100         }  // bool
            };

            PhotonNetwork.LocalPlayer.SetCustomProperties(props);

            GameManager.Instance.ChangeState(GameState.WaitingRoom);
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