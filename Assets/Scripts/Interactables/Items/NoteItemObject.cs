using System;
using UnityEngine;

public class NoteItemObject : MonoBehaviour, ICollectable
{
    [SerializeField] private ItemData noteItemData;

    public static event Action<ItemData> OnNoteCollected;

    public ItemData PickupItem()
    {
        OnNoteCollected?.Invoke(noteItemData);
        Destroy(gameObject);

        return noteItemData;
    }
}
