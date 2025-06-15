using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }
    public List<ItemSO> items;

    private Dictionary<int, ItemSO> itemDict;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        itemDict = new Dictionary<int, ItemSO>();
        foreach (var item in items)
        {
            itemDict[item.Id] = item;
        }
    }

    public ItemSO GetItemById(int id)
    {
        if (itemDict.TryGetValue(id, out var item))
            return item;
        return null;
    }
}
