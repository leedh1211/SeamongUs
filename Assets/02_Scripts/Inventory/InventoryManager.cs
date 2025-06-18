using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class InventoryManager : MonoBehaviourPunCallbacks
{
    // 상태 저장: 플레이어 ActorNumber별로 아이템 ID 목록 보관
    private Dictionary<int, List<int>> playerInventories = new();

    // MasterClient가 호출: 아이템 추가 요청 처리
    [PunRPC]
    public void RPC_RequestAddItem(int actorNumber, int itemId)
    {
        Debug.Log($"[Master] 플레이어 {actorNumber}에게 아이템 {itemId} 추가 요청 받음");

        // 상태 업데이트
        AddItemToPlayer(actorNumber, itemId);
    }

    // MasterClient 내부 함수: 상태만 관리, UI는 아님
    private void AddItemToPlayer(int actorNumber, int itemId)
    {
        if (!playerInventories.ContainsKey(actorNumber))
            playerInventories[actorNumber] = new List<int>();

        playerInventories[actorNumber].Add(itemId);

        // 대상 플레이어 객체 찾기
        Player player = PhotonNetwork.CurrentRoom?.GetPlayer(actorNumber);
        if (player != null)
        {
            // 그 플레이어에게만 해당 아이템 갱신 요청
            photonView.RPC(nameof(RPC_UpdateInventory), player, itemId);
        }
        else
        {
            Debug.LogWarning($"[Master] ActorNumber {actorNumber}에 해당하는 플레이어를 찾을 수 없습니다.");
        }
    }

    // 클라이언트에서만 실행되는 함수: UI에 직접 반영
    [PunRPC]
    private void RPC_UpdateInventory(int itemId)
    {
        SoundManager.Instance.PlaySFX(SFXType.Item);

        var item = ItemDatabase.Instance.GetItemById(itemId);
        if (item == null)
        {
            Debug.LogWarning($"[Local] 아이템 ID {itemId}를 ItemDatabase에서 찾을 수 없습니다.");
            return;
        }

        var uiInventory = FindObjectOfType<UIInventory>();
        if (uiInventory != null)
        {
            uiInventory.AddItem(item);
            Debug.Log($"[Local] 아이템 '{item.itemName}' 인벤토리에 추가 완료");
        }
        else
        {
            Debug.LogWarning("[Local] UIInventory 컴포넌트를 찾을 수 없습니다.");
        }
    }
    public List<int> GetInventoryByActorNumber(int actorNumber)
    {
        return playerInventories.TryGetValue(actorNumber, out var inventory) ? inventory : new List<int>();
    }
}
