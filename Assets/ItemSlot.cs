using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public ItemSO item;
    public Image icon;
    public TextMeshProUGUI quantityText;

    public UIInventory inventory;
    public Button button;

    private int quantity;

    private void Awake()
    {
        button.onClick.AddListener(() =>
        {
            if (item != null)
            {
                // 인벤토리 리스트의 InventoryItem 찾아서 전달
                var inventoryItem = inventory.FindInventoryItem(item);
                inventory.SelectItem(inventoryItem);
            }
            else
            {
                Debug.LogWarning("[ItemSlot] 빈 슬롯 클릭됨");
            }
        });
    }

    public void Set(ItemSO newItem, int newQuantity)
    {
        item = newItem;
        quantity = newQuantity;

        if (item != null && quantity > 0)
        {
            icon.sprite = item.icon;
            icon.gameObject.SetActive(true);
            quantityText.text = quantity > 1 ? quantity.ToString() : "";
            gameObject.SetActive(true);
        }
        else
        {
            Clear();
        }
    }

    public void Clear()
    {
        item = null;
        quantity = 0;
        icon.gameObject.SetActive(false);
        quantityText.text = "";
        gameObject.SetActive(false);
    }
}
