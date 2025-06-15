using UnityEngine;
using Photon.Pun;

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

            photonView.RPC("RPC_RequestAddItem", RpcTarget.MasterClient, actorNumber, itemId);

            PhotonNetwork.Destroy(gameObject);
        }
    }

    [PunRPC]
    public void RPC_RequestAddItem(int actorNumber, int itemId)
    {
        Debug.Log($"MasterClient: 플레이어 {actorNumber}에게 아이템 {itemId} 추가 처리");

        ItemSO item = ItemDatabase.Instance.GetItemById(itemId);
        if (item == null)
        {
            Debug.LogError($"아이템 ID {itemId}에 해당하는 아이템을 찾을 수 없습니다!");
            return;
        }

        // PlayerUIManager에서 UIInventory 가져오기
        var uiInventory = PlayerUIManager.Instance.GetInventoryByActorNumber(actorNumber);
        if (uiInventory == null)
        {
            Debug.LogWarning($"플레이어 {actorNumber}의 UIInventory를 찾을 수 없습니다.");
            return;
        }

        // 인벤토리에 아이템 추가
        uiInventory.AddItem(item);

        Debug.Log($"아이템 '{item.itemName}'이 플레이어 {actorNumber}의 인벤토리에 추가되었습니다.");
    }

    // 플레이어 오브젝트를 ActorNumber로 찾는 함수
    private GameObject FindPlayerByActorNumber(int actorNumber)
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            PhotonView pv = player.GetComponent<PhotonView>();
            if (pv != null && pv.Owner.ActorNumber == actorNumber)
            {
                return player;
            }
        }
        return null;
    }
}