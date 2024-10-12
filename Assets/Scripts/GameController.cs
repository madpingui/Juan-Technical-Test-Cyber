using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections;

public class GameController : MonoBehaviour
{
    [SerializeField] private List<DifficultyConfigPair> difficultyConfigPairs;
    [SerializeField] private MatchHandler matchHandler;
    [SerializeField] private CardGridCreator cardGridCreator;

    private Dictionary<GameDifficulty, CardGameConfig> difficultyConfigs;
    private List<Card> allCards = new List<Card>();
    private GameDifficulty chosenDifficulty;

    public static event Action OnMatchStarted;
    public static event Action OnMatchLoaded;
    public static event Action OnMatchFinished;

    private void Awake()
    {
        // Initialize the dictionary from the list of pairs
        difficultyConfigs = difficultyConfigPairs.ToDictionary(pair => pair.difficulty, pair => pair.config);       
        PlayButton.OnPlayButtonClicked += StartNewGame;
        MatchHandler.OnCardMatch += CheckForEndGame;
        Card.OnCardFlipped += matchHandler.OnCardFlipped;
    }

    private void OnDestroy()
    {
        PlayButton.OnPlayButtonClicked -= StartNewGame;
        MatchHandler.OnCardMatch -= CheckForEndGame;
        Card.OnCardFlipped -= matchHandler.OnCardFlipped;
    }

    private void StartNewGame(GameDifficulty difficulty)
    {
        ClearExistingCards();
        chosenDifficulty = difficulty;
        SetupNewGame(difficultyConfigs[chosenDifficulty]);
        OnMatchStarted?.Invoke();
    }

    private void ClearExistingCards()
    {
        allCards.Clear();
        matchHandler.Clear();
        cardGridCreator.Clear();
    }

    private void SetupNewGame(CardGameConfig config)
    {
        cardGridCreator.CreateCardGrid(config.rows, config.columns, config.cardFrontSprites, ref allCards);
        StartCoroutine(ShowCardsTemporarily());
    }

    private IEnumerator ShowCardsTemporarily()
    {
        allCards.ForEach(card => card.FlipUpCard(false));
        yield return new WaitForSeconds(2f);
        allCards.ForEach(card => card.FlipDownCard());
    }

    //Check if all cards are matched if so then finish the game.
    private void CheckForEndGame()
    {
        if (allCards.All(card => card.IsMatched()))
        {
            OnMatchFinished?.Invoke();
        }
    }

    public GameData GetGameData(int turns, int score, int combo)
    {
        return new GameData
        {
            turns = turns,
            score = score,
            combo = combo,
            chosenDifficulty = (int)chosenDifficulty,
            cardStates = allCards.Select(card => new CardData
            {
                id = card.Id,
                isFlipped = card.IsFlipped(),
                isMatched = card.IsMatched()
            }).ToList()
        };
    }

    public void LoadGameData(GameData data)
    {
        StopAllCoroutines();
        ClearExistingCards();
        var gameConfig = difficultyConfigs[(GameDifficulty)data.chosenDifficulty];
        cardGridCreator.CreateCardGrid(gameConfig.columns, gameConfig.cardFrontSprites, data.cardStates, ref allCards);
        OnMatchLoaded?.Invoke();
    }
}