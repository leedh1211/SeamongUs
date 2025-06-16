using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using _02_Scripts.Ung_Managers;
using System;
using System.Linq;

public class EndingPopupController : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private RectTransform contentParent;
    [SerializeField] private GameObject playerSlotPrefab;

    // �˾��� ��� �� �� �޼��带 �� ȣ���ϼ���.
    public void Init(EndGameCategory winnerCategory)
    {
        // 1) ���� ����
        titleText.text =
            winnerCategory == EndGameCategory.CitizensWin
            ? "������ �¸�!"
            : "�������� �¸�!";

        // 2) �¸��� ��� ��ȸ
        List<Player> winners = new List<Player>();

        if (winnerCategory == EndGameCategory.CitizensWin)
        {
            foreach (var p in PhotonNetwork.PlayerList)
            {
                // Role �̾ƿ���
                p.CustomProperties.TryGetValue(PlayerPropKey.Role, out object roleObj);
                Role role = (Role)Convert.ToInt32(roleObj);
                if (role != Role.Crewmate)
                    continue;   // �������ʹ� �ǳʶڴ�

                // PlayerMissions ���� �� �÷��̾� Ű(ActorNumber.ToString())�� �Ҵ�� �̼� ��������
                string key = p.ActorNumber.ToString();
                if (MissionManager.Instance.PlayerMissions.TryGetValue(key, out var missions)
                    && missions.All(m => m.IsCompleted))
                {
                    winners.Add(p);
                }
            }
        }
        else
        {
            // ���������� �¸��� �� ��� �ִ� �������� ����
            foreach (var p in PhotonNetwork.PlayerList)
            {
                // dead ����
                p.CustomProperties.TryGetValue(PlayerPropKey.IsDead, out object deadObj);
                bool isDead = deadObj is bool db && db;

                // role ����
                p.CustomProperties.TryGetValue(PlayerPropKey.Role, out object roleObj);
                Role role = (Role)Convert.ToInt32(roleObj);

                if (role == Role.Impostor && !isDead)
                    winners.Add(p);
            }
        }

        // 3) ���� ����
        foreach (var p in winners)
        {
            var slot = Instantiate(playerSlotPrefab, contentParent, false);
            var img = slot.GetComponentInChildren<Image>();
            var txt = slot.GetComponentInChildren<TMP_Text>();

            // �ƹ�Ÿ �������� (AvatarManager�� ����Ǿ� �ִٸ�)
            if (AvatarManager.Instance != null &&
                p.CustomProperties.TryGetValue(PlayerPropKey.Spr, out object sprIdx))
            {
                img.sprite = AvatarManager.Instance.GetSprite((int)sprIdx);
            }

            txt.text = p.NickName;
        }
    }

    public void PlayEnter() => GetComponent<Animator>().SetTrigger("Enter");
    public void PlayExit() => GetComponent<Animator>().SetTrigger("Exit");
}
