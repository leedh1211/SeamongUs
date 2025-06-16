using Photon.Pun;
using System;
using System.Linq;
using UnityEngine;

public class MissionCollider : MonoBehaviour
{
    [Tooltip("Inspector에 이 미션에 대응하는 UI 패널 GameObject를 드래그하세요")]
    [SerializeField] private GameObject missionUIPanel;

    private IMissionUI missionUI;
    public MissionType missionType;

    private void Awake()
    {
        if (missionUIPanel == null)
        {
            Debug.LogError($"{name}: missionUIPanel에 UI 패널 GameObject를 할당해야 합니다.");
            return;
        }

        // GameObject에서 IMissionUI 구현체를 찾아서 할당
        missionUI = missionUIPanel.GetComponent<IMissionUI>();
        if (missionUI == null)
            Debug.LogError($"{name}: missionUIPanel에 IMissionUI를 구현한 컴포넌트(LaundryUI 등)를 붙여야 합니다.");
    }

    public void HandleInteract(string playerId)
    {

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(PlayerPropKey.Role, out object roleObj))
        {
            Role myRole = (Role)Convert.ToInt32(roleObj);
            if (myRole == Role.Impostor)
            {
                Debug.Log("[MissionCollider] 임포스터는 미션을 수행할 수 없습니다.");
                return;
            }
        }
        if (missionUI == null) return;

        var mission = MissionManager.Instance
            .PlayerMissions[playerId]
            .FirstOrDefault(m => m.MissionID == missionType.ToString());
        if (mission == null || mission.IsCompleted) return;

        missionUI.Show(mission, playerId);
    }
}