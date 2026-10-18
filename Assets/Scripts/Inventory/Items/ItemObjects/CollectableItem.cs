using UnityEngine;

public class CollectableItem : MonoBehaviour, ICollectable
{
    [SerializeField] private ItemData itemData;

    public ItemData PickupItem()
    {
        Destroy(gameObject);

        return itemData;
    }
}
