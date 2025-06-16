using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using _02_Scripts.Ung_Managers;
using System;

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
        foreach (var p in PhotonNetwork.PlayerList)
        {
            // 1) IsDead
            object deadObj;
            p.CustomProperties.TryGetValue(PlayerPropKey.IsDead, out deadObj);
            bool isDead = deadObj is bool db && db;

            // 2) Role
            object roleObj;
            p.CustomProperties.TryGetValue(PlayerPropKey.Role, out roleObj);
            int roleInt = Convert.ToInt32(roleObj);
            Role role = (Role)roleInt;

            if (winnerCategory == EndGameCategory.CitizensWin && role == Role.Crewmate && !isDead)
                winners.Add(p);
            if (winnerCategory == EndGameCategory.ImpostorsWin && role == Role.Impostor && !isDead)
                winners.Add(p);
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
