using _02_Scripts.Login;
using Photon.Pun;
using UnityEngine;

[System.Serializable]
public class PlayerInfo : MonoBehaviourPunCallbacks
{
    public int PlayerSeq;               // 플레이어 순번 (0 ~ N)
    public string PlayerID;            // 고유 식별자 (ex. PhotonView.ViewID or 커스텀 UUID)
    public string Nickname;            // 닉네임
    // public Color Color;                // 플레이어 컬러
    public int PlayerLevel;            // (선택 사항) 레벨
    public int PlayerExp;            // (선택 사항) 경험치
    public int PlayerGold;             // (선택 사항) 골드
    public bool isReady = false; // 준비 상태 (게임 시작 전)

    public string SpriteData;          // 선택한 캐릭터 스프라이트 이름/ID

    public bool IsDead = false;        // 사망 여부
    public Role Role;                  // 현재 역할 (Crewmate / Impostor)

    public Transform Transform;        // 해당 플레이어의 Transform
    public GameObject GameObject;      // 해당 플레이어의 GameObject (선택)

    public bool IsLocalPlayer = false; // 로컬 플레이어 여부 (클라이언트에서 직접 지정)

    public SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Init();
    }
    void Start()
    {
        
        // PlayerID, Nickname 등 초기화 먼저
       // PlayerID = System.Guid.NewGuid().ToString(); // or 할당된 ID
       // Nickname = "Player_" + Random.Range(0, 1000);

        // PlayerManager.Instance.RegisterPlayer(this);
        // GameManager.Instance.ChangeState(GameState.RoleAssignment);
    }

    private void Init()
    {
        PlayerSeq = LoginSession.loginPlayerInfo.seq;               // 플레이어 순번 (0 ~ N)
        PlayerID = LoginSession.loginPlayerInfo.id;            // 고유 식별자 (ex. PhotonView.ViewID or 커스텀 UUID)
        Nickname = LoginSession.loginPlayerInfo.name;            // 닉네임
        PlayerLevel = LoginSession.loginPlayerInfo.level;            // (선택 사항) 레벨
        PlayerGold = LoginSession.loginPlayerInfo.gold;             // (선택 사항) 골드
        
        EventBus.Raise(new OnPlayerInfoChanged());
    }
    public string GetDisplayInfo()
    {
        return $"[{PlayerSeq}] {Nickname} - Lv.{PlayerLevel} - Gold: {PlayerGold}";
    }

    [PunRPC]
    public void SetSprite(string spriteName)
    {
        Sprite newSprite = Resources.Load<Sprite>(spriteName);
        if (newSprite != null)
        {
            spriteRenderer.sprite = newSprite;
        }
    }
}
