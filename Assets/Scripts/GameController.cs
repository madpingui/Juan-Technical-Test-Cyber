using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System;

[Serializable]
public enum GameDifficulty
{
    Easy,
    Medium,
    Hard
}

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GridLayoutGroup cardGrid;
    [SerializeField] private List<CardGameConfig> gameConfigs;

    private List<Card> flippedCards = new List<Card>();
    private List<Card> allCards = new List<Card>();
    private GameDifficulty chosenDifficulty;

    public event Action OnMatchStarted;
    public event Action OnCardMatch;
    public event Action OnCardMiss;

    public void PlayGame(GameDifficulty difficulty)
    {
        foreach (Transform item in cardGrid.transform)
        {
            Destroy(item.gameObject);
        }

        allCards.Clear();
        flippedCards.Clear();

        chosenDifficulty = difficulty;
        var gameConfig = gameConfigs[(int)chosenDifficulty];
        CreateCardGrid(gameConfig.rows, gameConfig.columns, gameConfig.cardFrontSprites);
        OnMatchStarted?.Invoke();
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
        if (flippedCards.Contains(card)) return;

        flippedCards.Add(card);

        if (flippedCards.Count == 2)
        {
            CheckMatch();
        }
    }

    private void CheckMatch()
    {
        if (flippedCards[0].Id == flippedCards[1].Id)
        {
            flippedCards[0].SetMatched();
            flippedCards[1].SetMatched();

            OnCardMatch?.Invoke();

            //play match sound
        }
        else
        {
            flippedCards[0].ResetCard();
            flippedCards[1].ResetCard();

            OnCardMiss?.Invoke();

            // Play mismatch sound
        }

        flippedCards.Clear();
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
        foreach (Transform item in cardGrid.transform)
        {
            Destroy(item.gameObject);
        }

        allCards.Clear();
        flippedCards.Clear();

        var gameConfig = gameConfigs[(int)data.chosenDifficulty];
        cardGrid.constraintCount = gameConfig.columns;

        var cardSprites = gameConfig.cardFrontSprites;

        foreach (var cardData in data.cardStates)
        {
            GameObject newCardObj = Instantiate(cardPrefab, cardGrid.transform);
            Card newCard = newCardObj.GetComponent<Card>();
            allCards.Add(newCard);

            newCard.Initialize(cardData.id, cardSprites[cardData.id]);

            if (cardData.isFlipped)
            {
                newCard.Flip(); // Flip the card to its front
            }

            if (cardData.isMatched)
            {
                newCard.SetMatched(); // Mark it as matched
            }

            newCard.CardFlipped += OnCardFlipped;
        }
    }


}

