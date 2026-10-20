using System;
using ArtificeToolkit.Attributes;
using UnityEngine;

public class PlayerHidingController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerCamera;
    
    [Header("Hiding Camera Limits")]
    [SerializeField] private float lookSensitivity = 0.15f;
    [SerializeField] private float maxLookAngle = 45f;
    
    private CharacterController controller;
    private PlayerMovement playerMovement;
    private PlayerCameraController cameraController;
    private PlayerInteractionHandler interactionHandler;
    private PlayerInputHandler inputHandler;

    [SerializeField, ReadOnly] private bool isHiding = false;
    private Transform currentExitPoint;

    // Track the restricted camera rotation
    private float currentYaw = 0f;
    private float currentPitch = 0f;

    // Events
    public static event Action<bool> OnHidingStateChanged;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerMovement = GetComponent<PlayerMovement>();
        cameraController = GetComponent<PlayerCameraController>();
        interactionHandler = GetComponent<PlayerInteractionHandler>();
        inputHandler = GetComponent<PlayerInputHandler>();
    }
    
    private void OnEnable()
    {
        HidingTriggerObject.OnHidingTriggered += EnterHiding;
    }

    private void OnDisable()
    {
        HidingTriggerObject.OnHidingTriggered -= EnterHiding;
        inputHandler.OnInteract -= HandleExitHiding;
    }

    private void EnterHiding(Transform hidePoint, Transform exitPoint)
    {
        if (isHiding)
        {
            return;
        }

        inputHandler.OnInteract += HandleExitHiding;
        HidingTriggerObject.OnHidingTriggered -= EnterHiding;

        isHiding = true;
        currentExitPoint = exitPoint;

        // Disable standard components
        controller.enabled = false;
        playerMovement.enabled = false;
        cameraController.enabled = false;
        interactionHandler.enabled = false;

        // Snap player body to the hiding spot
        transform.position = hidePoint.position;
        transform.rotation = hidePoint.rotation;

        // Reset the camera's local rotation to player's restricted view
        playerCamera.localRotation = Quaternion.identity;
        currentYaw = 0f;
        currentPitch = 0f;

        OnHidingStateChanged?.Invoke(true);
    }

    private void Update()
    {
        if (!isHiding)
        {
            return;
        }

        // Handle restricted looking manually while hiding
        Vector2 lookInput = inputHandler.LookInput * lookSensitivity;

        currentYaw += lookInput.x;
        currentPitch -= lookInput.y;

        // Clamp the angles so they can't do a 360 spin inside a locker
        currentYaw = Mathf.Clamp(currentYaw, -maxLookAngle, maxLookAngle);
        currentPitch = Mathf.Clamp(currentPitch, -maxLookAngle, maxLookAngle);

        // Apply rotation ONLY to the camera, so the physical body stays hidden
        playerCamera.localRotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
    }

    private void HandleExitHiding()
    {
        if (!isHiding)
        {
            return;
        }

        inputHandler.OnInteract -= HandleExitHiding;
        HidingTriggerObject.OnHidingTriggered += EnterHiding;

        isHiding = false;

        // Snap to exit point
        transform.position = currentExitPoint.position;
        transform.rotation = currentExitPoint.rotation;

        // Reset camera straight
        playerCamera.localRotation = Quaternion.identity;

        // Re-enable standard controllers
        controller.enabled = true;
        playerMovement.enabled = true;
        cameraController.enabled = true;
        interactionHandler.enabled = true;

        OnHidingStateChanged?.Invoke(false);
    }
}
