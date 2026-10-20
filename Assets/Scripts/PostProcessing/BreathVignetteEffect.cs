using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BreathVignetteEffect : MonoBehaviour
{
    [Header("Vignette Settings")]
    [SerializeField, Tooltip("Intensity for the normal value of the vignette when resting")] 
    private float normalIntensity = 0.2f;
    [SerializeField, Tooltip("Intensity for the max value of the vignette when out of breath")] 
    private float chokedIntensity = 0.6f;
    
    [Header("Color Settings")]
    [SerializeField] private Color normalColor = Color.black;
    [SerializeField] private Color outOfBreathColor = new Color(0.15f, 0f, 0f);

    private Volume postProcessVolume;
    private Vignette vignette;

    private void Awake()
    {
        postProcessVolume = GetComponent<Volume>();

        // Search the Volume Profile for a Vignette override
        if (!postProcessVolume.profile.TryGet(out vignette))
        {
            Debug.LogError("No Vignette override found in the Volume profile!");
        }
    }

    private void OnEnable()
    {
        PlayerBreathController.OnBreathUpdated += UpdateVignette;
    }

    private void OnDisable()
    {
        PlayerBreathController.OnBreathUpdated -= UpdateVignette;
    }

    private void UpdateVignette(float normalizedBreath)
    {
        if (vignette == null)
        {
            return;
        }

        // normalizedBreath goes from 1.0 (full air) to 0.0 (empty).
        // We invert it so 0.0 air = 1.0 effect intensity.
        float breathLost = 1.0f - normalizedBreath;

        // Smoothly transition the intensity and color
        vignette.intensity.value = Mathf.Lerp(normalIntensity, chokedIntensity, breathLost);
        vignette.color.value = Color.Lerp(normalColor, outOfBreathColor, breathLost);
    }
}
