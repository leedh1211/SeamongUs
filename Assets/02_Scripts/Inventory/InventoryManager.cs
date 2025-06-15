using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class InventoryManager : MonoBehaviourPunCallbacks
{
    // 플레이어 ActorNumber 별 인벤토리 저장
    private Dictionary<int, List<int>> playerInventories = new();

    public void AddItemToPlayer(int actorNumber, int itemId)
    {
        if (!playerInventories.ContainsKey(actorNumber))
            playerInventories[actorNumber] = new List<int>();

        playerInventories[actorNumber].Add(itemId);

        // 해당 플레이어에게 인벤토리 갱신 RPC 호출
        Player player = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);
        if (player != null)
        {
            photonView.RPC(nameof(RPC_UpdateInventory), player, itemId);
        }
    }

    [PunRPC]
    private void RPC_UpdateInventory(int itemId)
    {
        var item = ItemDatabase.Instance.GetItemById(itemId);
        if (item == null)
        {
            Debug.LogWarning($"아이템 ID {itemId}를 ItemDatabase에서 찾을 수 없습니다.");
            return;
        }

        var uiInventory = FindObjectOfType<UIInventory>();
        if (uiInventory != null)
        {
            uiInventory.AddItem(item);
        }
        else
        {
            Debug.LogWarning("UIInventory를 찾을 수 없습니다.");
        }
    }

    // 플레이어가 아이템 획득 요청 RPC (DropItem에서 호출)
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

        // 플레이어 오브젝트 찾기
        GameObject playerGO = FindPlayerByActorNumber(actorNumber);
        if (playerGO == null)
        {
            Debug.LogWarning($"플레이어 {actorNumber}의 게임 오브젝트를 찾을 수 없습니다.");
            return;
        }

        UIInventory uiInventory = playerGO.GetComponent<UIInventory>();
        if (uiInventory == null)
        {
            Debug.LogWarning($"플레이어 {actorNumber}에 UIInventory 컴포넌트가 없습니다.");
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
