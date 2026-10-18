using UnityEngine;
using UnityEngine.InputSystem;

public class GameStateManager : MonoBehaviour
{
    private PlayerInputActions inputActions;

    private bool isGamePaused = false;

    private void Awake()
    {
        inputActions = new PlayerInputActions();

        inputActions.Global.TogglePause.performed += _ => ToggleGameState();
    }

    private void OnEnable()
    {
        inputActions.Global.Enable();
        inputActions.Player.Enable();

        inputActions.UI.Disable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Start()
    {
        // TODO: Activate when UI is implemented
        //SetGameState(isGamePaused);
    }

    private void ToggleGameState()
    {
        // TODO: Activate when UI is implemented

        //isGamePaused = !isGamePaused;

        //SetGameState(isGamePaused);
    }

    private void SetGameState(bool isPaused)
    {
        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    private void PauseGame()
    {
        inputActions.Player.Disable();
        inputActions.UI.Enable();

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ResumeGame()
    {
        inputActions.Player.Enable();
        inputActions.UI.Disable();

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
