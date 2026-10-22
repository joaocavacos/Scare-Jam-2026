using System;
using TMPro;
using UnityEngine;

public class GameHUDManager : Singleton<GameHUDManager>
{
    [Header("HUD References")]
    [SerializeField] private GameObject playerHUD;
    [SerializeField] private GameObject interactionHUD;

    [Header("Interaction HUD Parameters")]
    [SerializeField] private TMP_Text noteTitle;
    [SerializeField] private TMP_Text noteDescription;

    private void OnEnable()
    {
        NoteItemObject.OnNoteCollected += ShowNote;

        PlayerInputHandler.OnCancel += CloseNote;
    }

    private void OnDisable()
    {
        NoteItemObject.OnNoteCollected -= ShowNote;

        PlayerInputHandler.OnCancel -= CloseNote;
    }

    private void Start()
    {
        // By default, HUD state should always be PlayerHUD
        ChangeHUDState(true);
    }

    private void ChangeHUDState(bool isPlayerHUD)
    {
        PlayerInputHandler.Instance.ChangeInputMapping(isPlayerHUD ? ActionMappingsEnum.Player : ActionMappingsEnum.UI);
        GameStateManager.Instance.SetGameState(!isPlayerHUD);

        playerHUD.SetActive(isPlayerHUD);
        interactionHUD.SetActive(!isPlayerHUD);
    }

    private void ShowNote(ItemData itemData)
    {
        if (itemData != null)
        {
            noteTitle.SetText(itemData.itemName);
            noteDescription.SetText(itemData.itemDescription);
        }

        ChangeHUDState(false);
    }

    private void CloseNote()
    {
        ChangeHUDState(true);
    }
}
