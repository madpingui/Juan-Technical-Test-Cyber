using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System;
using System.Linq;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GridLayoutGroup cardGrid;
    [SerializeField] private List<DifficultyConfigPair> difficultyConfigPairs;

    private Dictionary<GameDifficulty, CardGameConfig> difficultyConfigs;
    private List<Card> currentMatchCheck = new List<Card>();
    private List<Card> allCards = new List<Card>();
    private GameDifficulty chosenDifficulty;

    public static event Action OnMatchStarted;
    public static event Action OnMatchFinished;
    public static event Action OnCardMatch;
    public static event Action OnCardMiss;

    private void Awake()
    {
        PlayButton.OnPlayButtonClicked += StartNewGame;

        // Initialize the dictionary from the list of pairs
        difficultyConfigs = new Dictionary<GameDifficulty, CardGameConfig>();

        foreach (var pair in difficultyConfigPairs)
        {
            if (!difficultyConfigs.ContainsKey(pair.difficulty))
            {
                difficultyConfigs.Add(pair.difficulty, pair.config);
            }
        }
    }

    private void StartNewGame(GameDifficulty difficulty)
    {
        ClearExistingCards();
        SetupNewGame(difficulty);
        OnMatchStarted?.Invoke();
    }

    private void ClearExistingCards()
    {
        foreach (Transform item in cardGrid.transform)
        {
            Destroy(item.gameObject);
        }
        allCards.Clear();
        currentMatchCheck.Clear();
    }

    private void SetupNewGame(GameDifficulty difficulty)
    {
        chosenDifficulty = difficulty;
        var gameConfig = difficultyConfigs[chosenDifficulty];
        CreateCardGrid(gameConfig.rows, gameConfig.columns, gameConfig.cardFrontSprites);
    }

    private void CreateCardGrid(int rows, int columns, Sprite[] cardFrontSprites, List<int> cardIds = null)
    {
        #region Pre-Conditions
        // Check if rows * columns is even
        int totalCards = rows * columns;
        if (totalCards % 2 != 0)
        {
            Debug.LogError("The total number of cards must be even to ensure all cards have a pair.");
            return; // Exit early if the number of cards is odd
        }

        // Assert that there are enough sprites for the number of unique card IDs
        int uniqueCardCount = (rows * columns) / 2; // Each card ID will have a pair
        Assert.IsTrue(cardFrontSprites.Length >= uniqueCardCount,
            $"Not enough cardFrontSprites! Required: {uniqueCardCount}, Available: {cardFrontSprites.Length}");
        #endregion

        cardIds ??= GenerateCardIds(rows, columns);
        cardGrid.constraintCount = columns;

        for (int i = 0; i < rows * columns; i++)
        {
            GameObject newCardObj = Instantiate(cardPrefab, cardGrid.transform);
            Card newCard = newCardObj.GetComponent<Card>();
            allCards.Add(newCard);

            int id = cardIds[i];
            newCard.Initialize(id, cardFrontSprites[id]);
            newCard.CardFlipped += OnCardFlipped;
        }
    }

    private List<int> GenerateCardIds(int rows, int columns)
    {
        List<int> ids = new List<int>();
        int pairs = (rows * columns) / 2;

        for (int i = 0; i < pairs; i++)
        {
            ids.Add(i); // pair 1
            ids.Add(i); // pair 2
        }

        ids.Shuffle(); // Utility method to randomize order
        return ids;
    }

    private void OnCardFlipped(Card card)
    {
        if (currentMatchCheck.Contains(card)) return;

        currentMatchCheck.Add(card);

        if (currentMatchCheck.Count == 2)
        {
            CheckMatch();
        }
    }

    private void CheckMatch()
    {
        if (currentMatchCheck[0].Id == currentMatchCheck[1].Id)
        {
            currentMatchCheck[0].SetMatched();
            currentMatchCheck[1].SetMatched();

            OnCardMatch?.Invoke();

            //Check if all cards are matched
            if(allCards.All(card => card.IsMatched()))
                OnMatchFinished?.Invoke();

            //play match sound
        }
        else
        {
            currentMatchCheck[0].FlipDownCard();
            currentMatchCheck[1].FlipDownCard();

            OnCardMiss?.Invoke();

            // Play mismatch sound
        }

        currentMatchCheck.Clear();
    }

    public GameData GetGameData(int score, int combo)
    {
        GameData gameData = new GameData
        {
            score = score,
            combo = combo,
            chosenDifficulty = (int)this.chosenDifficulty,
            cardStates = new List<CardData>()
        };

        foreach (Card card in allCards)
        {
            CardData cardData = new CardData
            {
                id = card.Id,
                isFlipped = card.IsFlipped(),
                isMatched = card.IsMatched()
            };
            gameData.cardStates.Add(cardData);
        }

        return gameData;
    }

    public void LoadGameData(GameData data)
    {
        ClearExistingCards();

        var gameConfig = difficultyConfigs[(GameDifficulty)data.chosenDifficulty];
        cardGrid.constraintCount = gameConfig.columns;

        var cardSprites = gameConfig.cardFrontSprites;

        foreach (var cardData in data.cardStates)
        {
            GameObject newCardObj = Instantiate(cardPrefab, cardGrid.transform);
            Card newCard = newCardObj.GetComponent<Card>();
            allCards.Add(newCard);

            newCard.Initialize(cardData.id, cardSprites[cardData.id]);

            if (cardData.isMatched)
            {
                newCard.SetMatched(); // Mark it as matched
            }
            else if (cardData.isFlipped)
            {
                newCard.FlipUpCard(); // Pass false to avoid triggering the event
            }

            newCard.CardFlipped += OnCardFlipped;
        }
    }
}

