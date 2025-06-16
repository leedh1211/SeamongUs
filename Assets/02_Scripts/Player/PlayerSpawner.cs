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
        Vector2 spawnPos = GetValidGroundPosition();

        GameObject player = PhotonNetwork.Instantiate(playerPrefabName, spawnPos, Quaternion.identity);

        // 플레이어가 생성된 후 StatManager 가져오기
        StatManager statMgr = player.GetComponent<StatManager>();
        if (statMgr != null)
        {
            PlayerUIManager.Instance.Initialize(statMgr);
        }
        else
        {
            Debug.LogWarning("[SpawnPlayer] 생성된 플레이어에 StatManager가 없습니다.");
        }

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

    // private Vector2 GetValidGroundPosition()
    // {
    //     const float radius = 0.5f;
    //     const int maxAttempts = 10;
    //
    //     if (groundCollider == null)
    //     {
    //         return fallbackSpawnPoint != null ? fallbackSpawnPoint.position : Vector2.zero;
    //     }
    //
    //     Bounds bounds = groundCollider.bounds;
    //
    //     for (int i = 0; i < maxAttempts; i++)
    //     {
    //         float x = Random.Range(bounds.min.x, bounds.max.x);
    //         float y = Random.Range(bounds.min.y, bounds.max.y);
    //         Vector2 tryPos = new Vector2(x, y);
    //
    //         // 지면 위인지 확인
    //         if (groundCollider.OverlapPoint(tryPos))
    //         {
    //             // 플레이어 겹침 체크
    //             Collider2D hit = Physics2D.OverlapCircle(tryPos, radius, LayerMask.GetMask("Player"));
    //             if (hit == null)
    //             {
    //                 return tryPos;
    //             }
    //         }
    //     }
    //     return fallbackSpawnPoint != null ? fallbackSpawnPoint.position : Vector2.zero;
    // }

    private Vector2 GetValidGroundPosition()
    {
        const float searchRadius = 5f;
        const float playerCheckRadius = 0.5f;
        const int maxAttempts = 20;
        
        if (groundObject == null)
        {
            Debug.LogWarning("GroundLevel 오브젝트를 찾을 수 없습니다.");
            return Vector2.zero;
        }

        Tilemap tilemap = groundObject.GetComponent<Tilemap>();
        if (tilemap == null)
        {
            Debug.LogWarning("GroundLevel 오브젝트에 Tilemap 컴포넌트가 없습니다.");
            return Vector2.zero;
        }

        for (int i = 0; i < maxAttempts; i++)
        {
            // 1. 0,0 기준으로 반경 5 이내 랜덤 위치
            Vector2 randomPos = (Vector2)Vector2.zero + Random.insideUnitCircle * searchRadius;

            // 2. 해당 위치에 타일이 있는지 확인
            Vector3Int cellPos = tilemap.WorldToCell(randomPos);
            if (!tilemap.HasTile(cellPos)) continue;

            // 3. 해당 위치에 다른 플레이어가 겹치는지 확인
            Collider2D hit = Physics2D.OverlapCircle(randomPos, playerCheckRadius, LayerMask.GetMask("Player"));
            if (hit == null)
            {
                return randomPos;
            }
        }

        Debug.LogWarning("유효한 위치를 찾지 못했습니다. (기본값 반환)");
        return Vector2.zero; // fallback 위치
    }

}
