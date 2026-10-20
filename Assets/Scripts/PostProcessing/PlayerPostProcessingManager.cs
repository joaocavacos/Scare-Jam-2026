using UnityEditor.Searcher;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerPostProcessingManager : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] private float baseVignette = 0.2f;

    [Header("Breath Effect Limits")]
    [SerializeField] private float maxBreathVignette = 0.6f;
    [SerializeField] private Color normalColor = Color.black;
    [SerializeField] private Color chokedColor = new Color(0.15f, 0f, 0f);

    [Header("Hiding Effect Limits")]
    [SerializeField] private float hidingVignette = 0.5f;
    [SerializeField] private float normalFocusDistance = 10f;
    [SerializeField] private float hidingFocusDistance = 0.5f;
    [SerializeField] private float normalFocalLength = 50f;
    [SerializeField] private float hidingFocalLength = 150f;

    private Volume postProcessVolume;
    private Vignette vignette;
    private DepthOfField depthOfField;

    // Tracked States
    private float currentBreathLost = 0f;
    private bool isHiding = false;
    private float hidingWeight = 0f; // Used to smoothly lerp the hiding transition

    private void Awake()
    {
        postProcessVolume = GetComponent<Volume>();
        postProcessVolume.profile.TryGet(out vignette);
        postProcessVolume.profile.TryGet(out depthOfField);
    }

    private void OnEnable()
    {
        PlayerBreathController.OnBreathUpdated += HandleBreathUpdate;
        PlayerHidingController.OnHidingStateChanged += HandleHidingUpdate;
    }

    private void OnDisable()
    {
        PlayerBreathController.OnBreathUpdated -= HandleBreathUpdate;
        PlayerHidingController.OnHidingStateChanged -= HandleHidingUpdate;
    }

    private void HandleBreathUpdate(float normalizedBreath)
    {
        // Convert 1.0 (full) to 0.0 (empty) into a "lost" percentage
        currentBreathLost = 1.0f - normalizedBreath;
    }

    private void HandleHidingUpdate(bool hiding)
    {
        isHiding = hiding;
    }

    private void Update()
    {
        ApplyPostProcessing();
    }

    private void ApplyPostProcessing()
    {
        hidingWeight = isHiding ? 1f : 0f;

        if (vignette != null)
        {
            vignette.intensity.overrideState = true;
            vignette.color.overrideState = true;

            // Breath smoothly fades based on stamina
            float breathVig = Mathf.Lerp(baseVignette, maxBreathVignette, currentBreathLost);
            
            // Hiding instantly snaps between base and hiding values
            float hidingVig = Mathf.Lerp(baseVignette, hidingVignette, hidingWeight);
            
            vignette.intensity.value = Mathf.Max(breathVig, hidingVig);
            vignette.color.value = Color.Lerp(normalColor, chokedColor, currentBreathLost);
        }

        if (depthOfField != null)
        {
            depthOfField.focusDistance.overrideState = true;
            depthOfField.focalLength.overrideState = true;

            // Instantly snaps between normal and hiding blur
            depthOfField.focusDistance.value = Mathf.Lerp(normalFocusDistance, hidingFocusDistance, hidingWeight);

            depthOfField.focalLength.value = Mathf.Lerp(normalFocalLength, hidingFocalLength, hidingWeight);
        }
    }
}
