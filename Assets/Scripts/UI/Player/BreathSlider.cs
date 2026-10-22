using UnityEngine;
using UnityEngine.UI;

public class BreathSlider : MonoBehaviour
{
    private Slider breathSlider;

    private void Awake()
    {
        breathSlider = GetComponent<Slider>();
        breathSlider.minValue = 0f;
        breathSlider.maxValue = 1f;
    }

    private void OnEnable()
    {
        PlayerBreathController.OnBreathUpdated += UpdateBreathSlider;
    }

    private void OnDisable()
    {
        PlayerBreathController.OnBreathUpdated -= UpdateBreathSlider;
    }

    private void UpdateBreathSlider(float normalizedBreath)
    {
        breathSlider.value = normalizedBreath;
    }
}
