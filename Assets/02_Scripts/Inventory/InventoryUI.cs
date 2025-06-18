using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;  // 인벤토리 전체 패널
    [SerializeField] private GameObject exitButton;      // Exit 버튼 (선택)

    private bool isOpen = false;

    public void ToggleInventory()
    {
        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);
    }

    public void CloseInventory()
    {
        isOpen = false;
        inventoryPanel.SetActive(false);
    }

    public void OpenInventory()
    {
        isOpen = true;
        inventoryPanel.SetActive(true);
    }

    private void Start()
    {
        CloseInventory(); // 처음엔 꺼진 상태
    }
}
