using UnityEngine;
using System.Collections.Generic;

public class InventoryHandler : MonoBehaviour
{
    [SerializeField] private List<InventoryItem> inventoryList = new List<InventoryItem>();

    private Dictionary<ItemData, InventoryItem> inventoryDictionary = new Dictionary<ItemData, InventoryItem>();

    public void AddItem(ItemData itemData)
    {
        if (inventoryDictionary.TryGetValue(itemData, out InventoryItem item))
        {
            item.AddQuantity();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(itemData);
            inventoryList.Add(newItem);
            inventoryDictionary.Add(itemData, newItem);
        }
    }

    public void RemoveItem(ItemData itemData)
    {
        if (inventoryDictionary.TryGetValue(itemData, out InventoryItem item))
        {
            item.RemoveQuantity();

            if (item.quantity == 0)
            {
                inventoryList.Remove(item);
                inventoryDictionary.Remove(itemData);
            }
        }
    }

    public InventoryItem GetItemByString(string itemName)
    {
        foreach (InventoryItem item in inventoryList)
        {
            if (item.itemData.itemName.Equals(itemName))
            {
                return item;
            }
        }

        return null;
    }
}
