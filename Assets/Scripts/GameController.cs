using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GridLayoutGroup cardGrid;

    private List<Card> flippedCards = new List<Card>();
    private List<Card> allCards = new List<Card>();

    public void PlayGame(CardGameConfig gameConfig) => CreateCardGrid(gameConfig.rows, gameConfig.columns, gameConfig.cardFrontSprites);

    private void CreateCardGrid(int rows, int columns, Sprite[] cardFrontSprites)
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

        List<int> cardIds = GenerateCardIds(rows, columns);
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
            // Update score, play match sound
            // Add score
        }
        else
        {
            flippedCards[0].ResetCard();
            flippedCards[1].ResetCard();
            // Play mismatch sound
        }

        flippedCards.Clear();
    }
}

