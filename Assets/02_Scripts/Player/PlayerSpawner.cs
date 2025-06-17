using System.Collections;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    [SerializeField] private PlayerManager playerManager;
    public string playerPrefabName = "Prefabs/Player"; // Resources/Prefabs/Player.prefab
    [SerializeField] private GameObject groundObject;

    public Transform fallbackSpawnPoint;
    public LayerMask groundLayer;

    private Collider2D groundCollider;
    private bool hasSpawned = false;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (PhotonNetwork.InRoom && PhotonNetwork.IsConnectedAndReady)
        {
            GameObject groundObj = GameObject.Find("GroundLevel");
            if (groundObj != null)
            {
                groundCollider = groundObj.GetComponent<Collider2D>();
            }
            SpawnPlayer();
        }
    }

    private void SpawnPlayer()
    {
        Vector2 spawnPos = fallbackSpawnPoint.position;

        GameObject player = PhotonNetwork.Instantiate(playerPrefabName, spawnPos, Quaternion.identity);
        
        PlayerController playerController = player.GetComponent<PlayerController>();
        playerController.SetStartArea();
        
        // 플레이어가 생성된 후 StatManager 가져오기
        StatManager statMgr = player.GetComponent<StatManager>();
        if (statMgr != null && SceneManager.GetActiveScene().name == "GameScene")
        {
            PlayerUIManager.Instance.Initialize(statMgr);
        }
        else
        {
            Debug.LogWarning("[SpawnPlayer] 생성된 플레이어에 StatManager가 없습니다.");
        }

        //  1) PhotonView 가져오기
        PhotonView view = player.GetComponent<PhotonView>();
        //  2) 이 뷰의 Owner(TagObject)에 자기 GameObject를 등록
        view.Owner.TagObject = player;               // ★ 핵심 한 줄

        // 기존 RaiseEvent 로직
        if (playerManager != null)
        {
            BroadcastPlayerSpawn(player);
        }
    }
    
    private void BroadcastPlayerSpawn(GameObject playerObject)
    {
        PhotonView view = playerObject.GetComponent<PhotonView>();
        if (view == null)
        {
            Debug.LogError("PhotonView가 존재하지 않는 오브젝트입니다.");
            return;
        }

        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        int viewID = view.ViewID;

        object[] eventData = new object[] { actorNumber, viewID };

        PhotonNetwork.RaiseEvent(
            EventCodes.PlayerSpawn,
            eventData,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            SendOptions.SendReliable
        );

        Debug.Log($"[PlayerSpawner] RaiseEvent로 PlayerSpawn 전송됨: Actor={actorNumber}, ViewID={viewID}");
    }

}
