using Photon.Pun;
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
    [SerializeField] private List<InventoryItem> items = new();
    [SerializeField] private Transform slotParent;
    [SerializeField] private GameObject itemSlotPrefab;

    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescText;
    [SerializeField] private Button useButton;

    [SerializeField] private ItemEffectHandler effectHandler;

    [SerializeField] private List<ItemSlot> itemSlots = new();

    private InventoryItem selectedInventoryItem = null;

    public static UIInventory Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        useButton.onClick.AddListener(OnClickUseButton);
        useButton.interactable = false;
        RefreshUI();

        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        PlayerItemUIManager.Instance.RegisterInventory(actorNumber, this);

    }

    public void AddItem(ItemSO item)
    {
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

    public InventoryItem FindInventoryItem(ItemSO item)
    {
        return items.Find(x => x.item == item);
    }

    private void UpdateSelectedItemUI(InventoryItem inventoryItem)
    {
        // 예: 이름과 설명 텍스트 UI 갱신
        itemNameText.text = inventoryItem.item.itemName;
        itemDescText.text = GetItemEffectText(inventoryItem.item);
        useButton.interactable = true;
    }

    private string GetItemEffectText(ItemSO item)
    {
        // effectType에 따른 설명 리턴 (예시)
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
    public void SelectItem(InventoryItem inventoryItem)
    {
        selectedInventoryItem = inventoryItem;
        UpdateSelectedItemUI(inventoryItem);
        Debug.Log($"[UIInventory] 아이템 선택됨: {inventoryItem.item.itemName}");
    }


    public void RefreshUI()
    {
        while (itemSlots.Count < items.Count)
        {
            GameObject slotObj = Instantiate(itemSlotPrefab, slotParent);
            ItemSlot newSlot = slotObj.GetComponent<ItemSlot>();
            newSlot.inventory = this;
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

    private void OnClickUseButton()
    {
        if (selectedInventoryItem == null || selectedInventoryItem.quantity <= 0)
            return;

        effectHandler.UseItem(selectedInventoryItem.item);

        selectedInventoryItem.quantity--;
        if (selectedInventoryItem.quantity <= 0)
        {
            items.Remove(selectedInventoryItem);
            DeselectItem();
        }
        RefreshUI();
    }

    public void DeselectItem()
    {
        selectedInventoryItem = null;
        itemNameText.text = "";
        itemDescText.text = "";
        useButton.interactable = false;
    }
}
