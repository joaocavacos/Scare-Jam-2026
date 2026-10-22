using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class InventoryHandler : MonoBehaviour
{
    [SerializeField] private List<InventoryItem> inventoryList = new List<InventoryItem>();

    private Dictionary<ItemData, InventoryItem> inventoryDictionary = new Dictionary<ItemData, InventoryItem>();

    // Item Data/Whether it's a new item or a +quantity
    public static event Action<ItemData, bool> OnInventoryItemAdded;
    public static event Action<ItemData, bool> OnInventoryItemRemoved;

    public void AddItem(ItemData itemData)
    {
        if (inventoryDictionary.TryGetValue(itemData, out InventoryItem item))
        {
            item.AddQuantity();
            OnInventoryItemAdded?.Invoke(itemData, false);
        }
        else
        {
            InventoryItem newItem = new InventoryItem(itemData);
            inventoryList.Add(newItem);
            inventoryDictionary.Add(itemData, newItem);
            OnInventoryItemAdded?.Invoke(itemData, true);
        }
    }

    public void RemoveItem(ItemData itemData)
    {
        if (inventoryDictionary.TryGetValue(itemData, out InventoryItem item))
        {
            bool wasRemoved = false;
            item.RemoveQuantity();

            if (item.quantity == 0)
            {
                inventoryList.Remove(item);
                inventoryDictionary.Remove(itemData);
                wasRemoved = true;
            }

            OnInventoryItemRemoved?.Invoke(itemData, wasRemoved);
        }
    }

    public InventoryItem GetItemByData(ItemData itemData)
    {
        return inventoryList.FirstOrDefault(item => item.itemData == itemData);
    }

    public InventoryItem GetItemByString(string itemName)
    {
        return inventoryList.FirstOrDefault(item => item.itemData.itemName.Equals(itemName));
    }
}
