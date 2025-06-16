using UnityEngine;
using UnityEngine.UI;

public class ResultSlot : MonoBehaviour
{
    [SerializeField] private Text label; // 슬롯 상 몇번 플레이어 인지 확인
    [SerializeField] private Transform prefabParent;
    [SerializeField] private GameObject votedMarkPrefab; // 개표 1표 이미지 프리팹

    public void SetLabel(string text)
    {
        label.text = text;
    }
    public void AddBubble()
    {
        Instantiate(votedMarkPrefab, prefabParent);
    }
}
