using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TogglePanel : MonoBehaviour
{
    [Header("패널 연결")]
    public GameObject targetPanel;
    public GameObject optionalSecondPanel; // (예: 채팅 패널 등)

    private void Start()
    {
        if (TryGetComponent(out Button button))
        {
            button.onClick.AddListener(ToggleTarget);
        }
        if (targetPanel != null && !targetPanel.activeSelf)
        {
            targetPanel.SetActive(true);    // 잠깐 켰다가
            targetPanel.SetActive(false);   // 바로 끔
        }
    }

    private void ToggleTarget()
    {
        Debug.Log("ToggleTarget 실행됨!");
        // 첫 번째 패널 토글
        if (targetPanel != null)
        {
            bool isActive = targetPanel.activeSelf;
            targetPanel.SetActive(!isActive);
            if (!isActive)
            {
                // 프레임 끝에서 Layout 강제 재계산
                Canvas.ForceUpdateCanvases();
                LayoutRebuilder.ForceRebuildLayoutImmediate(targetPanel.GetComponent<RectTransform>());
            }
        }

        // 두 번째 패널이 설정돼 있으면 같이 토글 (선택 사항)
        if (optionalSecondPanel != null)
        {
            bool isActive = optionalSecondPanel.activeSelf;
            optionalSecondPanel.SetActive(!isActive);
        }

        // 버튼 포커스 해제 (Space 키 자동 입력 방지)
        EventSystem.current.SetSelectedGameObject(null);
    }
}