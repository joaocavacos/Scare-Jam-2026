using ArtificeToolkit.Attributes;
using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour, IInteractable
{
    [SerializeField] private bool useUnityEvent;
    [SerializeField, EnableIf(nameof(useUnityEvent))] private UnityEvent interactUnityEvent;

    private bool canInteract = true;

    public void Interact()
    {
        if (!canInteract)
        {
            return;
        }

        if (useUnityEvent)
        {
            interactUnityEvent?.Invoke();
        }
    }
    
    public void ChangeInteractionState(bool interactable)
    {
        canInteract = interactable;
    }
}
