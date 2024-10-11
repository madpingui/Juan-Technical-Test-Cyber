using UnityEngine;

public class PlayButton : MonoBehaviour
{
    [SerializeField] private GameDifficulty gameDifficulty;
    [SerializeField] private GameController gameController;

    public void Play() => gameController.PlayGame(gameDifficulty);
}
