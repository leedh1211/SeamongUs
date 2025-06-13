using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    [SerializeField] private PlayerManager playerManager;
    public string playerPrefabName = "Prefabs/Player"; // Resources/Prefabs/Player.prefab
    public Transform[] spawnPoints;

    private bool hasSpawned = false; // static 제거

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
            SpawnPlayer(); // 씬 로딩 완료 후 스폰
        }
    }
    private IEnumerator WaitAndSpawn()
    {
        yield return new WaitUntil(() => PhotonNetwork.InRoom && PhotonNetwork.NetworkClientState == ClientState.Joined);

        // 자기 자신만 생성
        if (!hasSpawned && PhotonNetwork.IsConnectedAndReady)
        {
            SpawnPlayer();
            hasSpawned = true;
        }
    }

    private void SpawnPlayer()
    {
        int index = PhotonNetwork.LocalPlayer.ActorNumber % spawnPoints.Length;
        Transform basePoint = spawnPoints[index];

        Vector2 randomOffset = Vector2.zero;
        const float radius = 10f;
        const int maxAttempts = 10;

        for (int i = 0; i < maxAttempts; i++)
        {
            randomOffset = Random.insideUnitCircle * radius;
            Vector2 spawnPos = (Vector2)basePoint.position + randomOffset;

            // 주변에 이미 존재하는 플레이어가 있는지 검사
            Collider2D hit = Physics2D.OverlapCircle(spawnPos, 1f, LayerMask.GetMask("Player"));
            if (hit == null)
            {
                // 안전한 위치 찾았으므로 생성
                GameObject player = PhotonNetwork.Instantiate(playerPrefabName, spawnPos, Quaternion.identity);

                PlayerInfo info = player.GetComponent<PlayerInfo>();
                info.IsLocalPlayer = true;

                if (playerManager != null)
                {
                    playerManager.RegisterPlayer(info);
                    PhotonNetwork.LocalPlayer.TagObject = player;
                    var roomUI = FindObjectOfType<RoomUIManager>();
                    if (roomUI != null)
                    {
                        roomUI.UpdatePlayerListFromManager();
                    }
                }
                return;
            }
        }

        // 실패 시 기본 위치에 생성
        GameObject fallbackPlayer = PhotonNetwork.Instantiate(playerPrefabName, basePoint.position, Quaternion.identity);

        PlayerInfo fallbackInfo = fallbackPlayer.GetComponent<PlayerInfo>();
        fallbackInfo.IsLocalPlayer = true;

        if (playerManager != null)
        {
            playerManager.RegisterPlayer(fallbackInfo);
            PhotonNetwork.LocalPlayer.TagObject = fallbackPlayer;
        }
        Debug.Log(fallbackInfo.Nickname);
        var fallroomUI = FindObjectOfType<RoomUIManager>();
        if (fallroomUI != null)
        {
            fallroomUI.UpdatePlayerListFromManager();
        }
    }

}