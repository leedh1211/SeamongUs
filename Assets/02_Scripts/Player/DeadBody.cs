using Photon.Realtime;
using UnityEngine;

public class DeadBody : MonoBehaviour
{
    public string ownerID;
    public int PlayerActorNumber;           // 죽은 플레이어 ID

    public void Initialize(int ActorNumber)
    {
        PlayerActorNumber = ActorNumber;
        // 색상, 닉네임 등 표현 가능
    }
}
