using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboText;

    private int score = 0;
    private int combo = 0;

    private void Start()
    {
        gameController.OnCardMatch += OnMatch;
        gameController.OnCardMiss += OnMiss;
        gameController.OnMatchStarted += ResetScores;
    }

    private void OnDisable()
    {
        gameController.OnCardMatch -= OnMatch;
        gameController.OnCardMiss -= OnMiss;
        gameController.OnMatchStarted -= ResetScores;
    }

    private void ResetScores()
    {
        score = 0;
        combo = 0;
        SetScore(score);
        SetCombo(combo);
    }

    private void OnMatch()
    {
        score++; // Increment score for every match
        combo++; // Increase combo for successful matches
        SetScore(score);
        SetCombo(combo);
    }

    private void OnMiss()
    {
        combo = 0; // Reset combo on a miss
        SetCombo(combo);
    }

    public void SetScore(int score) => scoreText.text = $"Score: {score}";
    public void SetCombo(int combo) => comboText.text = $"Combo x{combo}";
    public int GetScore() => score;
    public int GetCombo() => combo;
}

