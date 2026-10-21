using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Menu Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject optionsPanel;

    [Header("Settings UI")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TMP_Text sensitivityText;
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private TMP_Dropdown screenModeDropdown;

    [Header("Default Values")]
    [SerializeField] private float defaultVolume = 1.0f;
    [SerializeField] private float defaultSensitivity = 2.0f;

    [Header("Level Loading")] 
    [SerializeField] private string sceneToLoad;

    private const string PREF_VOLUME = "MasterVolume";
    private const string PREF_SENSITIVITY = "MouseSensitivity";
    private const string PREF_SCREENMODE = "ScreenMode";

    private void Start()
    {
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);

        // Load saved settings (or use defaults if none exist)
        LoadSettings();
    }

    public void PlayGame()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    public void OpenOptions()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("Game has quit!");
        Application.Quit();
    }

    private void LoadSettings()
    {
        // Load values
        float savedVolume = PlayerPrefs.GetFloat(PREF_VOLUME, defaultVolume);
        float savedSensitivity = PlayerPrefs.GetFloat(PREF_SENSITIVITY, defaultSensitivity);
        int savedScreenMode = PlayerPrefs.GetInt(PREF_SCREENMODE, 0); // Default 0 is Fullscreen

        volumeSlider.value = savedVolume;
        sensitivitySlider.value = savedSensitivity;
        screenModeDropdown.value = savedScreenMode;

        ApplyVolume(savedVolume);
        ApplySensitivity(savedSensitivity);
        ApplyScreenMode(savedScreenMode);
    }

    public void ApplyVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat(PREF_VOLUME, value);
    }

    public void ApplySensitivity(float value)
    {
        PlayerPrefs.SetFloat(PREF_SENSITIVITY, value);

        if (sensitivityText != null)
        {
            sensitivityText.text = value.ToString("F2");
        }
    }

    public void ApplyScreenMode(int dropdownIndex)
    {
        switch (dropdownIndex)
        {
            case 0: // Fullscreen
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 1: // Borderless
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case 2: // Windowed
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
        }

        PlayerPrefs.SetInt(PREF_SCREENMODE, dropdownIndex);
    }
}
