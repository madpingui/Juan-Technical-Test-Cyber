using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PlayButton : MonoBehaviour
{
    [SerializeField] private GameDifficulty gameDifficulty;
    private Button button;

    public static Action<GameDifficulty> OnPlayButtonClicked;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(Play);

        // Subscribe to game state events to manage button interactability
        GameController.OnMatchStarted += DisableButton;
        GameController.OnMatchLoaded += DisableButton;
        GameController.OnMatchFinished += EnableButton;
    }

    private void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        GameController.OnMatchStarted -= DisableButton;
        GameController.OnMatchLoaded -= DisableButton;
        GameController.OnMatchFinished -= EnableButton;
    }

    private void DisableButton() => button.interactable = false;
    private void EnableButton()  => button.interactable = true;
    private void Play()          => OnPlayButtonClicked?.Invoke(gameDifficulty);
}
