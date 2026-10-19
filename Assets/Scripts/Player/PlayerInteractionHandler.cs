using System;
using UnityEngine;

public class PlayerInteractionHandler : MonoBehaviour
{
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float interactionRange = 2.5f;
    [SerializeField] private LayerMask interactionLayer;

    private PlayerInputHandler inputHandler;
    private InventoryHandler inventoryHandler;

    private GameObject currentInteractionTarget;

    private bool isInteractingWithTarget = false;

    // Events
    public static Action<bool> OnInteractableTargeted;

    private void Awake()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
        inventoryHandler = GetComponent<InventoryHandler>();
    }

    private void OnEnable()
    {
        inputHandler.OnInteract += HandleInteract;
    }

    private void OnDisable()
    {
        inputHandler.OnInteract -= HandleInteract;
    }

    private void Update()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        bool foundInteractable = false;

        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange, interactionLayer))
        {
            currentInteractionTarget = hit.collider.gameObject;
            foundInteractable = true;

            Debug.DrawLine(ray.origin, hit.point, Color.green);
        }
        else
        {
            currentInteractionTarget = null;

            Debug.DrawRay(ray.origin, ray.direction * interactionRange, Color.red);
        }

        if(foundInteractable != isInteractingWithTarget)
        {
            isInteractingWithTarget = foundInteractable;
            OnInteractableTargeted?.Invoke(isInteractingWithTarget);
        }
    }

    private void HandleInteract()
    {
        if(currentInteractionTarget == null)
        {
            return;
        }

        if(currentInteractionTarget.TryGetComponent(out IInteractable interactable))
        {
            // Interactions (Doors, Puzzles, etc.)
            interactable.Interact();
        }
        else if (currentInteractionTarget.TryGetComponent(out ICollectable collectable))
        {
            // Collectables (Notes, Documents, Items, etc.)
            ItemData itemData = collectable.PickupItem();
            inventoryHandler.AddItem(itemData);
        }

        currentInteractionTarget = null;
        isInteractingWithTarget = false;
        OnInteractableTargeted?.Invoke(isInteractingWithTarget);
    }
}
