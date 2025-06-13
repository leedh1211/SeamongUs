using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class DropZone : MonoBehaviour, IDropHandler
{
    [SerializeField] private int slotIndex;
    private Laundry mission;
    private string playerId;

    public void Initialize(Laundry mission, string playerId)
    {
        this.mission = mission;
        this.playerId = playerId;
    }

    //public void InitializeLocal()
    //{
    //    var missions = MissionManager.Instance.PlayerMissions.Values.FirstOrDefault();
    //    mission = missions?.Find(mission => mission is Laundry) as Laundry;
    //}

    public void OnDrop(PointerEventData eventData)
    {
        var drag = eventData.pointerDrag.GetComponent<DragCloth>();
        if(drag != null && mission != null)
        {
            if (mission.GetorderPrefabs()[slotIndex] == drag.clothPrefab)
            {
                mission.TryHang(drag.clothPrefab);
                Destroy(drag.gameObject);
                if (mission.IsCompleted)
                    MissionManager.Instance.CompleteMission(playerId, mission.MissionID);
            }
            else
            {
                drag.rect.anchoredPosition = Vector2.zero; // 잘못된 경우 원래 위치로 되돌리기
            }
        }
    }
}
