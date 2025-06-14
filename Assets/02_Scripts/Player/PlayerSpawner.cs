using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    [SerializeField] private PlayerManager playerManager;
    public string playerPrefabName = "Prefabs/Player"; // Resources/Prefabs/Player.prefab

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
        PlayerInfo info = player.GetComponent<PlayerInfo>();
        info.IsLocalPlayer = true;

        if (playerManager != null)
        {
            playerManager.RegisterPlayer(info);
        }
    }

    private Vector2 GetValidGroundPosition()
    {
        const float radius = 0.5f;
        const int maxAttempts = 10;

        if (groundCollider == null)
        {
            return fallbackSpawnPoint != null ? fallbackSpawnPoint.position : Vector2.zero;
        }

        Bounds bounds = groundCollider.bounds;

        for (int i = 0; i < maxAttempts; i++)
        {
            float x = Random.Range(bounds.min.x, bounds.max.x);
            float y = Random.Range(bounds.min.y, bounds.max.y);
            Vector2 tryPos = new Vector2(x, y);

            // 지면 위인지 확인
            if (groundCollider.OverlapPoint(tryPos))
            {
                // 플레이어 겹침 체크
                Collider2D hit = Physics2D.OverlapCircle(tryPos, radius, LayerMask.GetMask("Player"));
                if (hit == null)
                {
                    return tryPos;
                }
            }
        }
        return fallbackSpawnPoint != null ? fallbackSpawnPoint.position : Vector2.zero;
    }
}
