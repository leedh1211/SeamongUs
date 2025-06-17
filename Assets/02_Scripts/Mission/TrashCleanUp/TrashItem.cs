using UnityEngine;
using UnityEngine.EventSystems;

public class TrashItem : MonoBehaviour, IPointerClickHandler
{
    private TrashCleanup mission;
    private string playerId;
    private TrashUI ui;


    public void Initialize(TrashCleanup mission, string playerId, TrashUI ui)
    {
        this.mission = mission;
        this.playerId = playerId;
        this.ui = ui;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        bool result = mission.CollectOne();
        if (result)
        {
            MissionManager.Instance.CompleteMission(playerId, mission.MissionID);   
        }
        Destroy(gameObject);

        if (mission.IsCompleted)
            ui.Hide();
    }


}
