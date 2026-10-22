using System;
using UnityEngine;

public class GlowstickController : MonoBehaviour
{
    private static readonly int EmissiveMultiplier = Shader.PropertyToID("_EmissiveMultiplier");

    [Header("References")]
    [SerializeField] private Light glowstickLight;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip snapClip;

    [Header("Player References")]
    [SerializeField] private InventoryHandler inventoryHandler;

    [Header("Item Data")]
    [SerializeField] private ItemData glowstickData;

    [Header("After Use Settings")]
    [SerializeField] private GameObject deadGlowstickPrefab;
    [SerializeField] private Transform dropPoint;

    [Header("Light Settings")]
    [SerializeField] private float maxIntensity = 5f;
    [SerializeField] private float duration = 15f;
    [SerializeField, Tooltip("X axis is Time (0 to 1). Y axis is Light Intensity (0 to 1).")] 
    private AnimationCurve lightCurve; 
    
    [Header("Material Glow (Shader Graph)")]
    [SerializeField] private MeshRenderer glowstickRenderer;
    [SerializeField] private float maxEmissiveMultiplier = 2.0f;

    [Header("AI Alert Settings")]
    [SerializeField] private float noiseRadius = 20f;
    
    // The event the Enemy AI will listen to
    public static event Action<Vector3, float> OnNoiseGenerated;

    private float currentTimer = 0f;
    private bool isActive = false;

    private void Start()
    {
        if (glowstickLight != null)
        {
            glowstickLight.intensity = 0f;
        }

        if (glowstickRenderer != null)
        {
            InventoryItem glowstickItem = inventoryHandler.GetItemByData(glowstickData);
            bool hasGlowstick = glowstickItem != null && glowstickItem.quantity > 0;

            if (hasGlowstick) OnGlowstickAddedToInventory(glowstickData, true);
            else OnGlowstickRemovedFromInventory(glowstickData, true);

            glowstickRenderer.material.SetFloat(EmissiveMultiplier, 0.1f);
        }
    }

    private void OnEnable()
    {
        PlayerInputHandler.OnSnapGlowstick += HandleSnap;
        InventoryHandler.OnInventoryItemAdded += OnGlowstickAddedToInventory;
        InventoryHandler.OnInventoryItemRemoved += OnGlowstickRemovedFromInventory;
    }

    private void OnDisable()
    {
        PlayerInputHandler.OnSnapGlowstick -= HandleSnap;
        InventoryHandler.OnInventoryItemAdded -= OnGlowstickAddedToInventory;
        InventoryHandler.OnInventoryItemRemoved -= OnGlowstickRemovedFromInventory;
    }

    private void OnGlowstickAddedToInventory(ItemData itemData, bool hasBeenAdded)
    {
        if (hasBeenAdded && itemData == glowstickData && glowstickRenderer != null)
        {
            glowstickRenderer.enabled = true;
        }
    }

    private void OnGlowstickRemovedFromInventory(ItemData itemData, bool hasBeenRemoved)
    {
        if (hasBeenRemoved && itemData == glowstickData && glowstickRenderer != null)
        {
            glowstickRenderer.enabled = false;
        }
    }

    private void HandleSnap()
    {
        // Prevent snapping a new one if one is already active
        if (isActive)
        {
            return;
        }

        InventoryItem glowstickItem = inventoryHandler.GetItemByData(glowstickData);
        if (glowstickItem == null || glowstickItem.quantity <= 0)
        {
            return;
        }

        isActive = true;
        currentTimer = 0f;

        // Play the sharp snap sound
        if (audioSource != null && snapClip != null)
        {
            audioSource.PlayOneShot(snapClip);
        }

        // Broadcast the noise to the AI
        // We pass the exact position the noise happened, and how far it travels
        OnNoiseGenerated?.Invoke(transform.position, noiseRadius);
    }

    private void HandleGlowstickDeath()
    {
        // Remove from inventory
        inventoryHandler.RemoveItem(glowstickData);

        // Drop the dead physical object
        if (deadGlowstickPrefab != null && dropPoint != null)
        {
            GameObject deadStick = Instantiate(deadGlowstickPrefab, dropPoint.position, dropPoint.rotation);

            // Give it a tiny forward nudge so it doesn't clip directly into the player's feet
            if (deadStick.TryGetComponent(out Rigidbody rb))
            {
                rb.AddForce(dropPoint.forward * 1.5f, ForceMode.VelocityChange);
            }
        }
    }

    private void Update()
    {
        if (!isActive || glowstickLight == null)
        {
            return;
        }

        currentTimer += Time.deltaTime;

        // Normalize time to a 0.0 - 1.0 scale for the Animation Curve
        float normalizedTime = currentTimer / duration;

        if (normalizedTime >= 1f)
        {
            // Glowstick dies
            isActive = false;
            glowstickLight.intensity = 0f;

            if (glowstickRenderer != null)
            {
                glowstickRenderer.material.SetFloat(EmissiveMultiplier, 0.1f);
            }

            HandleGlowstickDeath();
            return;
        }

        // Evaluate the curve and apply the light intensity
        float currentCurveValue = lightCurve.Evaluate(normalizedTime);
        glowstickLight.intensity = maxIntensity * currentCurveValue;

        if (glowstickRenderer != null)
        {
            float currentMaterialEmission = maxEmissiveMultiplier * currentCurveValue;
            glowstickRenderer.material.SetFloat(EmissiveMultiplier, currentMaterialEmission);
        }
    }
}
