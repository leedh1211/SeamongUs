using Photon.Pun;
using System;
using System.Linq;
using UnityEngine;

public class MissionCollider : MonoBehaviour
{
    [Tooltip("Inspector에서 직접 연결하는 UI 패널 GameObject를 참조하세요")]
    [SerializeField] private GameObject missionUIPanel;

    private IMissionUI missionUI;
    public MissionType missionType;
    public PlayerManager playerManager;
    public PlayerController playerController = null;

    private bool isOpen = false;

    private void Awake()
    {
        if (missionUIPanel == null)
        {
            Debug.LogError($"{name}: missionUIPanel에 UI 패널 GameObject가 할당되어야 합니다.");
            return;
        }

        // GameObject에서 IMissionUI 인터페이스를 구현한 컴포넌트를 찾음
        missionUI = missionUIPanel.GetComponent<IMissionUI>();
        if (missionUI == null)
            Debug.LogError($"{name}: missionUIPanel에 IMissionUI를 구현한 컴포넌트(LaundryUI 등)가 필요합니다.");
    }
    

    public void HandleInteract(string playerId)
    {
        if (playerController == null)
        {
            playerController = playerManager.FindPlayerController(PhotonNetwork.LocalPlayer.ActorNumber);   
        }
        
        if (isOpen) return;
        
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(PlayerPropKey.Role, out object roleObj))
        {
            Role myRole = (Role)Convert.ToInt32(roleObj);
            if (myRole == Role.Impostor)
            {
                Debug.Log("[MissionCollider] 임포스터는 미션을 실행할 수 없습니다.");
                return;
            }
        }
        if (missionUI == null) return;

        var mission = MissionManager.Instance
            .PlayerMissions[playerId]
            .FirstOrDefault(m => m.MissionID == missionType.ToString());
        if (mission == null || mission.IsCompleted) return;
        isOpen = true;
        missionUI.Show(mission, playerId);
        playerController.SetInteraction(true);
    }

    public void CloseUI()
    {
        if (playerController == null)
        {
            playerController = playerManager.FindPlayerController(PhotonNetwork.LocalPlayer.ActorNumber);   
        }
        
        if (!isOpen) return;

        isOpen = false;
        missionUIPanel.SetActive(false);

        // 상호작용 상태 해제
        playerController.SetInteraction(false);
    }
}