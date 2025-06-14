using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class InventoryItem
{
    public ItemSO item;
    public int quantity;
}
public class UIInventory : MonoBehaviour
{
    [Header("인벤토리 아이템 목록")]
    [SerializeField] private List<InventoryItem> items = new();
    [SerializeField] private Transform slotParent; // 슬롯들을 담을 부모 오브젝트 (Grid Layout이 있는 GameObject)
    [SerializeField] private GameObject itemSlotPrefab; // 슬롯 프리팹

    [Header("UI 요소")]
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescText;
    [SerializeField] private Button useButton;

    [Header("필요 참조")]
    [SerializeField] private ItemEffectHandler effectHandler;

    [Header("슬롯 UI들")]
    [SerializeField] private List<ItemSlot> itemSlots = new();

    private InventoryItem selectedInventoryItem = null;

    private void Start()
    {
        useButton.onClick.AddListener(OnClickUseButton);
        useButton.interactable = false;
        RefreshUI();
    }

    public void AddItem(ItemSO item)
    {
        // 이미 존재하면 수량 증가
        InventoryItem existing = FindInventoryItem(item);
        if (existing != null)
        {
            existing.quantity++;
        }
        else
        {
            items.Add(new InventoryItem { item = item, quantity = 1 });
        }

        RefreshUI();
    }

    public void SelectItem(InventoryItem inventoryItem)
    {

        selectedInventoryItem = inventoryItem;
        UpdateSelectedItemUI(inventoryItem);

        Debug.Log($"[UIInventory] 아이템 선택됨: {inventoryItem.item.itemName}");
    }

    private void UpdateSelectedItemUI(InventoryItem inventoryItem)
    {
        itemNameText.text = inventoryItem.item.itemName;
        itemDescText.text = GetItemEffectText(inventoryItem.item);
        useButton.interactable = true;
    }

    private void OnClickUseButton()
    {
        if (selectedInventoryItem == null || selectedInventoryItem.quantity <= 0)
        {
            Debug.LogWarning("[UIInventory] 사용할 아이템이 없습니다.");
            return;
        }

        Debug.Log($"[UIInventory] 아이템 사용: {selectedInventoryItem.item.itemName}");

        // 아이템 효과 적용
        effectHandler.UseItem(selectedInventoryItem.item);

        // 수량 1 감소
        selectedInventoryItem.quantity--;

        // 수량이 0이면 인벤토리에서 제거
        if (selectedInventoryItem.quantity <= 0)
        {
            items.Remove(selectedInventoryItem);
            DeselectItem();
        }

        // UI 갱신
        RefreshUI();
    }

    public void DeselectItem()
    {
        selectedInventoryItem = null;
        itemNameText.text = "";
        itemDescText.text = "";
        useButton.interactable = false;

        Debug.Log("[UIInventory] 아이템 선택 해제됨");
    }

    private string GetItemEffectText(ItemSO item)
    {
        return item.effectType switch
        {
            ItemEffectType.HealHp => $"체력 {item.effectValue} 회복",
            ItemEffectType.HealStamina => $"스태미나 {item.effectValue} 회복",
            ItemEffectType.BuffSpeed => $"이동 속도 {item.effectValue} 증가 ({item.duration}초)",
            ItemEffectType.RegenHp => $"체력 초당 {item.effectValue / item.duration:F1}씩 회복 ({item.duration}초)",
            ItemEffectType.Invisibility => $"은신 ({item.duration}초)",
            _ => "알 수 없는 효과"
        };
    }

    public void RefreshUI()
    {
        // 슬롯 부족하면 프리팹으로 추가 생성
        while (itemSlots.Count < items.Count)
        {
            GameObject slotObj = Instantiate(itemSlotPrefab, slotParent);
            ItemSlot newSlot = slotObj.GetComponent<ItemSlot>();
            newSlot.inventory = this; // 필수: 자기 참조 전달
            itemSlots.Add(newSlot);
        }

        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (i < items.Count)
            {
                itemSlots[i].Set(items[i].item, items[i].quantity);
                itemSlots[i].gameObject.SetActive(true);
            }
            else
            {
                itemSlots[i].Clear();
                itemSlots[i].gameObject.SetActive(false);
            }
        }
    }

    public InventoryItem FindInventoryItem(ItemSO item)
    {
        return items.Find(x => x.item == item);
    }
}
