using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIInventory : MonoBehaviour
{
    [Header("UI References")]
    public ItemSlot[] itemSlots;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescText;
    public Button useButton;

    [Header("Player")]
    public PlayerStatus player;

    private int selectedIndex = -1;
    private List<ItemSO> items = new List<ItemSO>();
    private List<int> quantities = new List<int>();

    private ItemSlot selectedSlot;

    private void Start()
    {
        useButton.onClick.AddListener(OnClickUseItem);
        RefreshUI();
    }

    public void AddItem(ItemSO itemSO)
    {
        int index = items.IndexOf(itemSO);
        if (index >= 0)
        {
            quantities[index]++;
        }
        else
        {
            items.Add(itemSO);
            quantities.Add(1);
        }

        RefreshUI();
    }
    public void SelectItem(int index)
    {
        Debug.Log($"SelectItem called with index: {index}");
        selectedIndex = index;
        ItemSO item = items[index];

        itemNameText.text = item.itemName;
        itemDescText.text = GetItemEffectText(item);
        useButton.interactable = true;
        Debug.Log($"Selected item: {item.itemName}, Description: {itemDescText.text}");
    }

    public void OnClickUseItem()
    {
        if (selectedSlot == null || selectedSlot.item == null)
            return;

        player.ApplyItemEffect(selectedSlot.item);

        selectedSlot.quantity--;
        if (selectedSlot.quantity <= 0)
        {
            items.RemoveAt(selectedSlot.index);
            quantities.RemoveAt(selectedSlot.index);
            selectedSlot.Clear();
            selectedSlot = null;
        }

        RefreshUI();
        ClearItemInfo();
    }

    private void RefreshUI()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (i < items.Count)
            {
                itemSlots[i].item = items[i];
                itemSlots[i].quantity = quantities[i];
                itemSlots[i].index = i;
                itemSlots[i].Set();
            }
            else
            {
                itemSlots[i].Clear();
            }
        }
    }

    private void ClearItemInfo()
    {
        itemNameText.text = "";
        itemDescText.text = "";
        useButton.interactable = false;
    }

    private string GetItemEffectText(ItemSO item)
    {
        switch (item.effectType)
        {
            case ItemEffectType.HealHp:
                return $"체력을 {item.effectValue} 회복합니다.";
            case ItemEffectType.HealStamina:
                return $"스태미나를 {item.effectValue} 회복합니다.";
            case ItemEffectType.BuffSpeed:
                return $"이동속도가 {item.effectValue} 만큼 증가합니다. ({item.duration}초)";
            case ItemEffectType.RegenHp:
                return $"체력을 {item.duration}초 동안 총 {item.effectValue} 회복합니다.";
            case ItemEffectType.Invisibility:
                return $"은신 상태가 {item.duration}초간 유지됩니다.";
            default:
                return "설명 없음.";
        }
    }



    public void OnClickItemButton(ItemSlot slot)
    {
        selectedSlot = slot;

        itemNameText.text = slot.item.itemName;
        itemDescText.text = GetItemEffectText(slot.item);
        useButton.interactable = true;
    }
}
