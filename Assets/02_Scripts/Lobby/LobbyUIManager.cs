using System.Collections.Generic;
using _02_Scripts.Lobby;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utill;

public class LobbyUIManager : Singleton<LobbyUIManager>
{
    [SerializeField] private Transform content;
    [SerializeField] private GameObject RoomSlotPrefab;
    [SerializeField] private GameObject RoomListPanel;
    
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
        if (scene.name != "LobbyScene")
        {
            RoomListPanel.SetActive(false);
        }
        else
        {
            RoomListPanel.SetActive(true);
        }
    } 

    public void UpdateRoomList(List<RoomInfo> roomList)
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        foreach (var room in roomList)
        {
            GameObject roomObject = Instantiate(RoomSlotPrefab, content);
            roomObject.GetComponent<RoomSlot>().Init(room);
        }
    }
    
}