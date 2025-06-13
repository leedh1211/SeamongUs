using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Collections.Generic;
using UnityEngine;

public class Laundry : Mission
{
    // Resources/ClothPrefabs ������ ���� �����յ� ����.
    private List<GameObject> orderPrefabs;
    // �� ����(0���� orderPrefabs.Count-1)���� ä�������� ���
    private bool[] filledSlots;

    public Laundry()
        : base("Laundry", "������ ���õ� ������� �ξ��ּ���.")
    {
        // 1) ���� ������ �ҷ��� ���� ���� �� 5�� �̱�
        var prefabs = Resources.LoadAll<GameObject>("ClothPrefabs");
        orderPrefabs = new List<GameObject>(prefabs);
        Shuffle(orderPrefabs);
        orderPrefabs = orderPrefabs.GetRange(0, Mathf.Min(5, orderPrefabs.Count));

        // 2) ���� ä�� ���� �ʱ�ȭ
        filledSlots = new bool[orderPrefabs.Count];
    }

    public override Mission Clone()
    {
        var clone = new Laundry();
        // Clone �� ���� ����, �� ä�� ���·� ����
        clone.orderPrefabs = new List<GameObject>(orderPrefabs);
        clone.filledSlots = new bool[orderPrefabs.Count];
        return clone;
    }

    /// <summary>
    /// ����(slotIndex)�� �ùٸ� ���� ������ �� ȣ�����ּ���.
    /// </summary>
    public void MarkSlotFilled(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= filledSlots.Length)
            return;

        if (filledSlots[slotIndex])
            return; // �̹� ä���� �����̸� ����

        filledSlots[slotIndex] = true;

        // ��� ������ ä�������� �˻�
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
            Complete();
        }
    }

    /// <summary>
    /// ������� ä���� ���� ������ ��ȯ�մϴ�.
    /// </summary>
    public int GetFilledCount()
    {
        int count = 0;
        foreach (var filled in filledSlots)
            if (filled) count++;
        return count;
    }

    /// <summary>
    /// �� �̼��� �䱸�ϴ� ���� ������ ���� ����Ʈ�� ��ȯ�մϴ�.
    /// </summary>
    public List<GameObject> GetorderPrefabs()
    {
        return new List<GameObject>(orderPrefabs);
    }

    // Fisher?Yates ���� ����
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
