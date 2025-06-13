using Photon.Pun;
using UnityEngine;

[System.Serializable]
public class PlayerInfo : MonoBehaviourPunCallbacks
{
    public int PlayerSeq;               // 플레이어 순번 (0 ~ N)
    public string PlayerID;            // 고유 식별자 (ex. PhotonView.ViewID or 커스텀 UUID)
    public string Nickname;            // 닉네임
    public Color Color;                // 플레이어 컬러
    public int PlayerLevel;            // (선택 사항) 레벨
    public int PlayerGold;             // (선택 사항) 골드

    public string SpriteData;          // 선택한 캐릭터 스프라이트 이름/ID

    public bool IsDead = false;        // 사망 여부
    public Role Role;                  // 현재 역할 (Crewmate / Impostor)

    public Transform Transform;        // 해당 플레이어의 Transform
    public GameObject GameObject;      // 해당 플레이어의 GameObject (선택)

    public bool IsLocalPlayer = false; // 로컬 플레이어 여부 (클라이언트에서 직접 지정)

    void Awake()
    {
        GameObject = this.gameObject;
        Transform = this.transform;
    }

    void Start()
    {
        /*
        if (photonView.IsMine)
        {
            PlayerID = PhotonNetwork.LocalPlayer.UserId;
            Nickname = PhotonNetwork.LocalPlayer.NickName;
            IsLocalPlayer = true;

            // 내 플레이어일 경우만 등록
            PlayerManager.Instance.RegisterPlayer(this);
         GameManager.Instance.ChangeState(GameState.RoleAssignment);
        }
        */
        // PlayerID, Nickname 등 초기화 먼저
        if (string.IsNullOrEmpty(PlayerID))
        {
            PlayerID = System.Guid.NewGuid().ToString();
        }
        Nickname = "Player_" + Random.Range(0, 1000);

        IsLocalPlayer = true;
        PlayerManager.Instance.RegisterPlayer(this);
        GameManager.Instance.ChangeState(GameState.RoleAssignment);
    }

    public void Die()
    {
        if (IsDead) return;
        IsDead = true;

        // 유령 처리
        //GetComponent<PlayerController>().enabled = false;

        // 시체 생성
        DeadBodyManager.Instance.SpawnDeadBody(transform.position, PlayerID);

        // 시각 효과 (투명도 등)
        // TODO: 유령 상태로 시각적 변경
        // GhostManager에 요청해서 투명하게 처리
        GhostManager.Instance.SetGhostAppearance(gameObject);
    }

}
