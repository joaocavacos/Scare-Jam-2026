using ArtificeToolkit.Attributes;
using UnityEngine;
using UnityEngine.Events;

public class CollectableObject : MonoBehaviour, ICollectable
{[SerializeField] private ItemData itemData;
     [SerializeField] private bool useUnityEvent;
     [SerializeField, EnableIf(nameof(useUnityEvent))] private UnityEvent collectUnityEvent;
 
     public ItemData PickupItem()
     {
         if (useUnityEvent)
         {
             collectUnityEvent?.Invoke();
         }
 
         Destroy(gameObject);
 
         return itemData;
     }
    
}
