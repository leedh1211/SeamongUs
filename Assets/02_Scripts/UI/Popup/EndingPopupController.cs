using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using _02_Scripts.Ung_Managers;

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
            bool isDead = (bool)(p.CustomProperties[PlayerPropKey.IsDead] ?? false);
            Role role = (Role)(byte)(p.CustomProperties[PlayerPropKey.Role] ?? 0);

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
