using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Collections.Generic;
using UnityEngine;

public class Laundry : Mission
{
    // Resources/ClothPrefabs 폴더에 빨래 프리팹들 있음.
    private List<GameObject> orderPrefabs;
    // 각 슬롯(0부터 orderPrefabs.Count-1)별로 채워졌는지 기록
    private bool[] filledSlots;

    public Laundry()
        : base("Laundry", "빨래를 제시된 순서대로 널어주세요.")
    {
        // 1) 빨래 프리팹 불러와 랜덤 섞은 뒤 5개 뽑기
        var prefabs = Resources.LoadAll<GameObject>("ClothPrefabs");
        orderPrefabs = new List<GameObject>(prefabs);
        Shuffle(orderPrefabs);
        orderPrefabs = orderPrefabs.GetRange(0, Mathf.Min(5, orderPrefabs.Count));

        // 2) 슬롯 채움 상태 초기화
        filledSlots = new bool[orderPrefabs.Count];
    }

    public override Mission Clone()
    {
        var clone = new Laundry();
        // Clone 시 같은 순서, 빈 채움 상태로 복제
        clone.orderPrefabs = new List<GameObject>(orderPrefabs);
        clone.filledSlots = new bool[orderPrefabs.Count];
        return clone;
    }

    /// <summary>
    /// 슬롯(slotIndex)에 올바른 옷이 놓였을 때 호출해주세요.
    /// </summary>
    public bool MarkSlotFilled(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= filledSlots.Length)
            return false;

        if (filledSlots[slotIndex])
            return false; // 이미 채워진 슬롯이면 무시

        filledSlots[slotIndex] = true;

        // 모든 슬롯이 채워졌는지 검사
        bool allFilled = true;
        foreach (var filled in filledSlots)
        {
            if (!filled)
            {
                allFilled = false;
                break;
            }
        }

        if (allFilled)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 현재까지 채워진 슬롯 개수를 반환합니다.
    /// </summary>
    public int GetFilledCount()
    {
        int count = 0;
        foreach (var filled in filledSlots)
            if (filled) count++;
        return count;
    }

    /// <summary>
    /// 이 미션이 요구하는 빨래 프리팹 순서 리스트를 반환합니다.
    /// </summary>
    public List<GameObject> GetorderPrefabs()
    {
        return new List<GameObject>(orderPrefabs);
    }

    // Fisher?Yates 랜덤 셔플
    private void Shuffle<T>(List<T> list)
    {
        var rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            int k = rng.Next(n--);
            T temp = list[n];
            list[n] = list[k];
            list[k] = temp;
        }
    }
}
