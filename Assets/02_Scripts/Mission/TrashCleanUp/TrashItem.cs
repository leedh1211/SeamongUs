using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrashItem : MonoBehaviour
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

    private void OnMouseDown() // Unity ���� �޼���, �ݶ��̴��ִ� ������ ���콺��ưŬ���� �ڵ�ȣ��
    {
        // 1) �̼� ������ ���� �˸�
        mission.CollectOne();

        // 2) MissionManager���� �Ϸ� �˸� (Photon ���� �ÿ��� �� ȣ��� ����ȭ)
        if (mission.IsCompleted)
            MissionManager.Instance.CompleteMission(playerId, mission.MissionID);

        // 3) �� ���ӿ�����Ʈ(������) ����
        Destroy(gameObject);

        // 4) Ŭ���� �� UI �ݱ�
        if (mission.IsCompleted)
            ui.Hide();
    }

}
