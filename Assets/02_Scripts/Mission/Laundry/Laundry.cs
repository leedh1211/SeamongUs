using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laundry : Mission
{
    //Resources/ClothPrefabs 폴더에 빨래 프리팹들 있음.
    private List<GameObject> orderPrefabs;
    private int currentIndex;

    public Laundry() : base ("Laundry", "빨래를 제시된 순서대로 널어주세요.")
    {
        var prefabs = Resources.LoadAll<GameObject>("ClothPrefabs");
        orderPrefabs = new List<GameObject>(prefabs);
        Shuffle(orderPrefabs);
        orderPrefabs = orderPrefabs.GetRange(0, Mathf.Min(5, orderPrefabs.Count));
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

    public void TryHang(GameObject clothPrefab)
    {
        if (IsCompleted) return;
        if (orderPrefabs[currentIndex] == clothPrefab)
        {
            currentIndex++;
            if( currentIndex >= orderPrefabs.Count)
            {
                Complete();
            }
        }
        else
        {
            currentIndex = 0; // 잘못널면 다시해야함.
        }
    }

    public List<GameObject> GetorderPrefabs()
    {
        return new List<GameObject>(orderPrefabs);
    }
    public int GetCurrentIndex()
    {
        return currentIndex;
    }

    public void ResetProgress()
    {
        currentIndex = 0;
    }
}
