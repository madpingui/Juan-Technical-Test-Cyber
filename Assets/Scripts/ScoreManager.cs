using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboText;

    private int score;
    public int Score
    {
        get => score;
        set
        {
            score = value;
            SetScore(score);
        }
    }

    private int combo;
    public int Combo
    {
        get => combo;
        set
        {
            combo = value;
            SetCombo(combo);
        }
    }

    private void Start()
    {
        GameController.OnCardMatch += OnMatch;
        GameController.OnCardMiss += OnMiss;
        GameController.OnMatchStarted += ResetScores;
    }

    private void OnDestroy()
    {
        GameController.OnCardMatch -= OnMatch;
        GameController.OnCardMiss -= OnMiss;
        GameController.OnMatchStarted -= ResetScores;
    }

    private void ResetScores()
    {
        Score = 0;
        Combo = 0;
    }

    private void OnMatch()
    {
        Score++; // Increment score for every match
        Combo++; // Increase combo for successful matches
    }

    private void OnMiss()
    {
        Combo = 0; // Reset combo on a miss
    }

    private void SetScore(int score) => scoreText.text = $"Score: {score}";
    private void SetCombo(int combo) => comboText.text = $"Combo x{combo}";
}

