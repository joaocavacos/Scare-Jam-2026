using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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

    public InventoryItem GetItemByData(ItemData itemData)
    {
        return inventoryList.FirstOrDefault(item => item.itemData == itemData);
    }

    public InventoryItem GetItemByString(string itemName)
    {
        return inventoryList.FirstOrDefault(item => item.itemData.itemName.Equals(itemName));
    }
}
