using _02_Scripts.Login;
using Photon.Pun;
using UnityEngine;

[System.Serializable]
public class PlayerInfo : MonoBehaviourPunCallbacks
{
    public int PlayerSeq;               // 플레이어 고유 번호
    public string PlayerID;            // 플레이어 ID
    public string Nickname;            // 닉네임
    // public Color Color;                // 플레이어 컬러
    public int PlayerLevel;            // (선택 사항) 레벨
    public int PlayerExp;            // (선택 사항) 경험치
    public int PlayerGold;             // (선택 사항) 골드

    public string SpriteData;          // 선택한 캐릭터 스프라이트 이름/ID

    public bool IsDead = false;        // 사망 여부
    public Role Role;                  // 현재 역할 (Crewmate / Impostor)

    public Transform Transform;        // 해당 플레이어의 Transform
    public GameObject GameObject;      // 해당 플레이어의 GameObject (선택)

    public bool IsLocalPlayer = false; // 로컬 플레이어 여부 (클라이언트에서 직접 지정)
    public Photon.Realtime.Player currentPlayer;
    
    private void Start()
    {
        currentPlayer = photonView.Owner;
        Init(currentPlayer);
    }

    private void Init(Photon.Realtime.Player owner)
    {
        if (owner.CustomProperties.TryGetValue("PlayerSeq", out object playerSeq))
        {
            PlayerSeq = (int)playerSeq;    
        }

        if (owner.CustomProperties.TryGetValue("PlayerID", out object playerID))
        {
            PlayerID = (string)playerID;
        };
        if (owner.CustomProperties.TryGetValue("Nickname", out object nickname))
        {
            Nickname = (string)nickname;
        }

        if (owner.CustomProperties.TryGetValue("PlayerLevel", out object playerLevel))
        {
            PlayerLevel = (int)playerLevel;
        }

        if (owner.CustomProperties.TryGetValue("PlayerGold", out object playerGold))
        {
            PlayerGold = (int)playerGold;
        }
        EventBus.Raise(new OnPlayerInfoChanged());
    }

}
