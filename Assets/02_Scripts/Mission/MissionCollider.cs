using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MissionCollider : MonoBehaviour
{
    [Tooltip("Inspector���� �� ���̿� �Ҵ�� �̼� Ÿ���� �����ϼ���")]
    public MissionType missionType;

    [SerializeField] private LaundryUI laundryUI;

    private void Awake()
    {

        if (laundryUI == null)
            laundryUI = FindObjectOfType<LaundryUI>();
    }

   
    public void HandleInteract(string playerId)
    {
        // 1) Player�� �Ҵ�� �̼� ��Ͽ��� �� Ÿ�� �̼� ��������
        var mission = MissionManager.Instance
            .PlayerMissions[playerId]
            .FirstOrDefault(m => m.MissionID == missionType.ToString()) as Laundry;
        if (mission == null) return;

        // 2) UI ����
        laundryUI.gameObject.SetActive(true);
        laundryUI.Initialize(mission, playerId);
    }
}
