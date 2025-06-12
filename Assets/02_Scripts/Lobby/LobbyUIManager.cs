using System.Collections.Generic;
using _02_Scripts.Lobby;
using Photon.Realtime;
using UnityEngine;
using Utill;

public class LobbyUIManager : Singleton<LobbyUIManager>
{
    [SerializeField] private Transform content;
    [SerializeField] private GameObject RoomSlotPrefab;

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