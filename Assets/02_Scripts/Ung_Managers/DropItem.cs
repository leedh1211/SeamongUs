using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class DropItem : MonoBehaviourPun
{
    [SerializeField] private int itemId;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!photonView.IsMine) return;

        if (other.CompareTag("Player"))
        {
            int actorNumber = other.GetComponent<PhotonView>().Owner.ActorNumber;

            Debug.Log($"플레이어 {actorNumber}가 아이템 {itemId} 획득 요청");

            // MasterClient에 아이템 추가 요청 RPC 호출
            photonView.RPC("RPC_RequestAddItem", RpcTarget.MasterClient, actorNumber, itemId);

            PhotonNetwork.Destroy(gameObject);
        }
    }

    // MasterClient에서 실행됨: 아이템 지급 승인 및 대상 클라이언트 호출
    [PunRPC]
    private void RPC_RequestAddItem(int actorNumber, int itemId)
    {
        Debug.Log($"[MasterClient] 아이템 {itemId} 지급 허용 → Actor {actorNumber}");

        Player targetPlayer = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);
        if (targetPlayer == null)
        {
            Debug.LogWarning($"[MasterClient] Actor {actorNumber}에 해당하는 플레이어를 찾을 수 없습니다.");
            return;
        }

        // 대상 클라이언트에게 아이템 지급 RPC 호출
        photonView.RPC(nameof(RPC_ReceiveItem), targetPlayer, itemId);
    }

    // 아이템 실제 추가: 각 클라이언트에서 실행
    [PunRPC]
    private void RPC_ReceiveItem(int itemId)
    {
        ItemSO item = ItemDatabase.Instance.GetItemById(itemId);
        if (item == null)
        {
            Debug.LogError($"[Local] 아이템 ID {itemId}에 해당하는 아이템을 찾을 수 없습니다!");
            return;
        }

        UIInventory inventory = UIInventory.Instance;  // 싱글톤으로 UIInventory 가져오기
        if (inventory == null)
        {
            Debug.LogWarning("[Local] UIInventory 싱글톤 인스턴스를 찾을 수 없습니다.");
            return;
        }

        inventory.AddItem(item);
        Debug.Log($"[Local] 아이템 '{item.itemName}' 인벤토리에 추가 완료");
    }
}
