using UnityEngine;
using TMPro;

public class CountSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label; // 슬롯 상 몇번 플레이어 인지 확인
    [SerializeField] private Transform markParent;
    [SerializeField] private GameObject markPrefab; // 개표 1표 이미지 프리팹

    public void SetLabel(string text)
    {
        label.text = text;
    }
    public void AddMark()
    {
        Instantiate(markPrefab, markParent);
    }
}
