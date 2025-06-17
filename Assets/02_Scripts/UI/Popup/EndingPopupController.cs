using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Linq;

public class EndingPopupController : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private RectTransform contentParent;
    [SerializeField] private GameObject playerSlotPrefab;

    // 팝업을 띄울 때 이 메서드를 꼭 호출하세요.
    public void Init(EndGameCategory winnerCategory)
    {
        // 1) 제목 세팅
        titleText.text =
            winnerCategory == EndGameCategory.CitizensWin
            ? "생존자 승리!"
            : "임포스터 승리!";

        // 2) 승리자 목록 조회
        List<Player> winners = new List<Player>();

        if (winnerCategory == EndGameCategory.CitizensWin)
        {
            foreach (var p in PhotonNetwork.PlayerList)
            {
                // Role 뽑아오기
                p.CustomProperties.TryGetValue(PlayerPropKey.Role, out object roleObj);
                Role role = (Role)Convert.ToInt32(roleObj);
                if (role != Role.Crewmate)
                {
                    continue;   // 임포스터는 건너뛴다
                }
                // PlayerMissions 에서 이 플레이어 키(ActorNumber.ToString())로 할당된 미션 가져오기
                winners.Add(p);
            }
        }
        else
        {
            // “임포스터 승리” → 살아 있는 임포스터 전원
            foreach (var p in PhotonNetwork.PlayerList)
            {
                // dead 여부
                p.CustomProperties.TryGetValue(PlayerPropKey.IsDead, out object deadObj);
                bool isDead = deadObj is bool db && db;

                // role 여부
                p.CustomProperties.TryGetValue(PlayerPropKey.Role, out object roleObj);
                Role role = (Role)Convert.ToInt32(roleObj);

                if (role == Role.Impostor && !isDead)
                    winners.Add(p);
            }
        }

        // 3) 슬롯 생성
        foreach (var p in winners)
        {
            var slot = Instantiate(playerSlotPrefab, contentParent, false);
            var img = slot.GetComponentInChildren<Image>();
            var txt = slot.GetComponentInChildren<TMP_Text>();

            // 아바타 가져오기 (AvatarManager에 연결되어 있다면)
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
