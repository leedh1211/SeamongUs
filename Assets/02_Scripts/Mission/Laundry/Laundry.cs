using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laundry : Mission
{
    //Resources/ClothPrefabs ������ ���� �����յ� ����.
    private List<GameObject> orderPrefabs;
    private int currentIndex;

    public Laundry() : base ("Laundry", "������ ���õ� ������� �ξ��ּ���.")
    {
        var prefabs = Resources.LoadAll<GameObject>("ClothPrefabs");
        orderPrefabs = new List<GameObject>(prefabs);
    }

  

    public override Mission Clone()
    {
        var clone = new Laundry();
        clone.orderPrefabs = new List<GameObject>(orderPrefabs);
        clone.currentIndex = 0;
        return clone;
    }

    private void Shuffle<T>(List<T> list)
    {
        var rnd = new System.Random();
        int n = list.Count;
        while(n>1)
        {
            int k = rnd.Next(n--);
            T temp = list[n];
            list[n] = list[k];
            list[k] = temp;
        }
    }
}
