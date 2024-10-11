using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GridLayoutGroup cardLayout;
    [SerializeField] private int rows = 2, columns = 2; // default 2x2 layout
    [SerializeField] private Sprite[] cardFrontSprites;

    private List<Card> flippedCards = new List<Card>();
    private List<Card> allCards = new List<Card>();
    private bool canFlip = true;

    private void Start()
    {
        CreateCardGrid(rows, columns);
    }

    private void CreateCardGrid(int rows, int columns)
    {
        List<int> cardIds = GenerateCardIds(rows, columns);
        cardLayout.constraintCount = columns;

        for (int i = 0; i < rows * columns; i++)
        {
            GameObject newCardObj = Instantiate(cardPrefab, cardLayout.transform);
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

        //ids.Shuffle(); // Utility method to randomize order
        return ids;
    }

    private void OnCardFlipped(Card card)
    {
        if (!canFlip || flippedCards.Contains(card)) return;

        flippedCards.Add(card);

        if (flippedCards.Count == 2)
        {
            StartCoroutine(CheckMatch());
        }
    }

    private IEnumerator CheckMatch()
    {
        canFlip = false;
        yield return new WaitForSeconds(1f); // delay for card comparison

        if (flippedCards[0].Id == flippedCards[1].Id)
        {
            flippedCards[0].SetMatched();
            flippedCards[1].SetMatched();
            // Update score, play match sound
            //ScoreManager.Instance.AddScore();
        }
        else
        {
            flippedCards[0].Flip();
            flippedCards[1].Flip();
            // Play mismatch sound
        }

        flippedCards.Clear();
        canFlip = true;
    }
}

