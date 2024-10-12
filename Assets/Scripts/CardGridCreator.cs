using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class CardGridCreator : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GridLayoutGroup cardGrid;

    public void CreateCardGrid(int rows, int columns, Sprite[] cardFrontSprites, ref List<Card> allCards)
    {
        int totalCards = rows * columns;
        if (totalCards % 2 != 0)
        {
            Debug.LogError("Total cards must be even for pairs.");
            return;
        }

        int uniqueCardCount = totalCards / 2;
        Assert.IsTrue(cardFrontSprites.Length >= uniqueCardCount,
            $"Insufficient card sprites. Needed: {uniqueCardCount}, Available: {cardFrontSprites.Length}");

        cardGrid.constraintCount = columns;
        List<int> cardIds = new List<int>();
        cardIds.GenerateAndShuffleIds(totalCards);
        InstantiateCards(cardIds, cardFrontSprites, ref allCards);
    }

    private void InstantiateCards(List<int> cardIds, Sprite[] cardFrontSprites, ref List<Card> allCards)
    {
        for (int i = 0; i < cardIds.Count; i++)
        {
            GameObject newCardObj = Instantiate(cardPrefab, cardGrid.transform);
            Card newCard = newCardObj.GetComponent<Card>();
            allCards.Add(newCard);

            int id = cardIds[i];
            newCard.Initialize(id, cardFrontSprites[id]);
        }
    }

    public void CreateCardGrid(int columns, Sprite[] cardFrontSprites, List<CardData> cardDataList, ref List<Card> allCards)
    {
        cardGrid.constraintCount = columns;

        for (int i = 0; i < cardDataList.Count; i++)
        {
            GameObject newCardObj = Instantiate(cardPrefab, cardGrid.transform);
            Card newCard = newCardObj.GetComponent<Card>();
            allCards.Add(newCard);

            var cardData = cardDataList[i];
            newCard.Initialize(cardData.id, cardFrontSprites[cardData.id]);

            if (cardData.isMatched)
            {
                newCard.SetMatched();
            }
            else if (cardData.isFlipped)
            {
                newCard.FlipUpCard();
            }
        }
    }

    public void Clear()
    {
        foreach (Transform item in cardGrid.transform)
        {
            Destroy(item.gameObject);
        }
    }
}
