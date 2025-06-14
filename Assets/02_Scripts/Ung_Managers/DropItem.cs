using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    [SerializeField] private ItemSO itemData;
    [SerializeField] private UIInventory inventory;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"[DropItem] {itemData.itemName} 획득 시도");
            if (inventory != null)
            {
                inventory.AddItem(itemData);
                Debug.Log($"[DropItem] {itemData.itemName} 획득");
                Destroy(gameObject);
            }
        }
    }
}
