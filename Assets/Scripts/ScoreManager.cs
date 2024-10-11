using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboText;

    private int score = 0;
    private int combo = 0;

    private void OnEnable()
    {
        gameController.OnMatchEvent += OnMatch;
        gameController.OnMissEvent += OnMiss;
    }

    private void OnDisable()
    {
        gameController.OnMatchEvent -= OnMatch;
        gameController.OnMissEvent -= OnMiss;
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

    private void SetScore(int score) => scoreText.text = $"Score: {score}";
    private void SetCombo(int combo) => comboText.text = $"Combo x{combo}";
}

