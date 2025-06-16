using UnityEngine;
using UnityEngine.UI;

public class TogglePanel : MonoBehaviour
{
    public GameObject targetPanel;

    private void Start()
    {
        if (TryGetComponent(out Button button))
        {
            button.onClick.AddListener(ToggleTarget);
        }
    }

    private void ToggleTarget()
    {
        if (targetPanel != null)
        {
            bool isActive = targetPanel.activeSelf;
            targetPanel.SetActive(!isActive);
        }
    }
}