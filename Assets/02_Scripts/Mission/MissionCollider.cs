using Photon.Pun;
using System;
using System.Linq;
using UnityEngine;

public class MissionCollider : MonoBehaviour
{
    [Tooltip("Inspector�� �� �̼ǿ� �����ϴ� UI �г� GameObject�� �巡���ϼ���")]
    [SerializeField] private GameObject missionUIPanel;

    private IMissionUI missionUI;
    public MissionType missionType;
    public PlayerController playerController;

    private void Awake()
    {
        if (missionUIPanel == null)
        {
            Debug.LogError($"{name}: missionUIPanel�� UI �г� GameObject�� �Ҵ��ؾ� �մϴ�.");
            return;
        }

        // GameObject���� IMissionUI ����ü�� ã�Ƽ� �Ҵ�
        missionUI = missionUIPanel.GetComponent<IMissionUI>();
        if (missionUI == null)
            Debug.LogError($"{name}: missionUIPanel�� IMissionUI�� ������ ������Ʈ(LaundryUI ��)�� �ٿ��� �մϴ�.");
    }

    public void HandleInteract(string playerId)
    {

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(PlayerPropKey.Role, out object roleObj))
        {
            Role myRole = (Role)Convert.ToInt32(roleObj);
            if (myRole == Role.Impostor)
            {
                Debug.Log("[MissionCollider] �������ʹ� �̼��� ������ �� �����ϴ�.");
                return;
            }
        }
        if (missionUI == null) return;

        var mission = MissionManager.Instance
            .PlayerMissions[playerId]
            .FirstOrDefault(m => m.MissionID == missionType.ToString());
        if (mission == null || mission.IsCompleted) return;

        missionUI.Show(mission, playerId);
        playerController.SetInteraction(true);
    }
}