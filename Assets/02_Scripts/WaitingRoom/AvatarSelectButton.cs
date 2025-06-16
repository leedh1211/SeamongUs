using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using ExitGames.Client.Photon;

public class AvatarSelectButton : MonoBehaviour
{
    public int avatarIndex; // 아바타 인덱스 0~8

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            // 커스텀 프로퍼티 변경: SPR 키
            Hashtable prop = new Hashtable { { PlayerPropKey.Spr, avatarIndex } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(prop);
        });
    }
}