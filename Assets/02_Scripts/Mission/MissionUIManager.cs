using System;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _02_Scripts.Mission
{
    public class MissionUIManager : MonoBehaviour, IOnEventCallback
    {
        [SerializeField] private GameObject missionUIListContent;
        [SerializeField] private Slider missionUISlider;
        [SerializeField] private GameObject missionTextPrefab;
        private string playerKey;
        private void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        private void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        public void OnEvent(EventData photonEvent)
        {
            switch (photonEvent.Code)
            {
                case EventCodes.MissionsAssignedCompleted:
                    playerKey = PhotonNetwork.LocalPlayer.ActorNumber.ToString();
                    UpdateMissionList();
                    GameManager.Instance.ChangeState(GameState.Playing);
                    break;

                case EventCodes.MissionCompletedUIRefresh: // UI 전용 트리거
                    UpdateMissionList();
                    UpdateMissionUISlider();
                    break;
            }
        }

        public void UpdateMissionList()
        {
            Role role = Role.UnManaged;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(PlayerPropKey.Role, out object roleObj))
            {
                role = (Role)Convert.ToInt32(roleObj);
            }
            if (role != Role.Crewmate) return;
            foreach (Transform child in missionUIListContent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            var sorted = MissionManager.Instance.PlayerMissions[playerKey]
                .OrderBy(m => m.IsCompleted) // false → true 순서로 정렬
                .ToList();
            foreach (var mission in sorted)
            {
                GameObject row = Instantiate(missionTextPrefab, missionUIListContent.transform);
                TMP_Text rowText = row.GetComponent<TMP_Text>();
                rowText.text = mission.Description;
                if (mission.IsCompleted)
                {
                    rowText.fontStyle = FontStyles.Strikethrough;
                }
            }
        }

        public void UpdateMissionUISlider()
        {
            missionUISlider.value = MissionManager.Instance.GetTotalProgress(); 
        }
    }
}