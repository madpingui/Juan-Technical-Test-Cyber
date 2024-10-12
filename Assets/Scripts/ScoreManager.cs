using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI turnsText;
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

    private int turns;
    public int Turns
    {
        get => turns;
        set
        {
            turns = value;
            SetTurns(turns);
        }
    }

    private void Start()
    {
        MatchHandler.OnCardMatch += OnMatch;
        MatchHandler.OnCardMiss += OnMiss;
        GameController.OnMatchStarted += ResetScores;
    }

    private void OnDestroy()
    {
        MatchHandler.OnCardMatch -= OnMatch;
        MatchHandler.OnCardMiss -= OnMiss;
        GameController.OnMatchStarted -= ResetScores;
    }

    // Reset all scores when a new match starts
    private void ResetScores()
    {
        Score = 0;
        Combo = 0;
        Turns = 0;
    }

    // Handle successful card match
    private void OnMatch()
    {
        Score++; // Increment score for every match
        Combo++; // Increase combo for successful matches
        Turns++;
    }

    // Handle card mismatch
    private void OnMiss()
    {
        Combo = 0; // Reset combo on a miss
        Turns++;
    }

    // Update UI text elements
    private void SetScore(int score) => scoreText.text = $"Score: {score}";
    private void SetCombo(int combo) => comboText.text = $"Combo x{combo}";
    private void SetTurns(int turns) => turnsText.text = $"Turns: {turns}";
}

