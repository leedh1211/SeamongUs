using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MissionCollider : MonoBehaviour
{
    [Tooltip("Inspector에서 이 스팟에 할당된 미션 타입을 선택하세요")]
    public MissionType missionType;

    [SerializeField] private LaundryUI laundryUI;

    private void Awake()
    {

        if (laundryUI == null)
            laundryUI = FindObjectOfType<LaundryUI>();
    }

   
    public void HandleInteract(string playerId)
    {
        // 1) Player의 할당된 미션 목록에서 이 타입 미션 가져오기
        var mission = MissionManager.Instance
            .PlayerMissions[playerId]
            .FirstOrDefault(m => m.MissionID == missionType.ToString()) as Laundry;
        if (mission == null) return;

        // 2) UI 열기
        laundryUI.gameObject.SetActive(true);
        laundryUI.Initialize(mission, playerId);
    }
}
