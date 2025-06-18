using System.Collections.Generic;
using UnityEngine;

public class PlayerItemUIManager : MonoBehaviour
{
    public static PlayerItemUIManager Instance;

    private Dictionary<int, UIInventory> playerInventories = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterInventory(int actorNumber, UIInventory inventory)
    {
        playerInventories[actorNumber] = inventory;
    }

    public UIInventory GetInventoryByActorNumber(int actorNumber)
    {
        playerInventories.TryGetValue(actorNumber, out var inventory);
        return inventory;
    }
}
