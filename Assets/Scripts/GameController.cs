using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System;
using System.Linq;
using System.Collections;

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
        // Initialize the dictionary from the list of pairs
        difficultyConfigs = difficultyConfigPairs.ToDictionary(pair => pair.difficulty, pair => pair.config);       
        PlayButton.OnPlayButtonClicked += StartNewGame;
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
        foreach (Transform item in cardGrid.transform)
        {
            Destroy(item.gameObject);
        }
        allCards.Clear();
        currentMatchCheck.Clear();
    }

    private void SetupNewGame(CardGameConfig config)
    {
        CreateCardGrid(config.rows, config.columns, config.cardFrontSprites);
    }

    private void CreateCardGrid(int rows, int columns, Sprite[] cardFrontSprites)
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
        InstantiateCards(cardIds, cardFrontSprites);
    }

    private void InstantiateCards(List<int> cardIds, Sprite[] cardFrontSprites)
    {
        for (int i = 0; i < cardIds.Count; i++)
        {
            GameObject newCardObj = Instantiate(cardPrefab, cardGrid.transform);
            Card newCard = newCardObj.GetComponent<Card>();
            allCards.Add(newCard);

            int id = cardIds[i];
            newCard.Initialize(id, cardFrontSprites[id]);
            newCard.OnCardFlipped += OnCardFlipped;
        }
        StartCoroutine(ShowCardsTemporarily());
    }

    private IEnumerator ShowCardsTemporarily()
    {
        allCards.ForEach(card => card.FlipUpCard(false));
        yield return new WaitForSeconds(2f);
        allCards.ForEach(card => card.FlipDownCard());
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
        bool isMatch = currentMatchCheck[0].Id == currentMatchCheck[1].Id;

        if (isMatch) HandleMatch();
        else         HandleMismatch();

        currentMatchCheck.Clear();
    }

    private void HandleMatch()
    {
        foreach (var card in currentMatchCheck)
        {
            card.SetMatched();
        }

        SoundManager.Instance.PlayCardMatch();
        OnCardMatch?.Invoke();

        if (allCards.All(card => card.IsMatched()))
        {
            OnMatchFinished?.Invoke();
        }
    }

    private void HandleMismatch()
    {
        foreach (var card in currentMatchCheck)
        {
            card.FlipDownCard();
        }

        SoundManager.Instance.PlayCardMiss();
        OnCardMiss?.Invoke();
    }

    public GameData GetGameData(int score, int combo)
    {
        return new GameData
        {
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
        ClearExistingCards();
        var gameConfig = difficultyConfigs[(GameDifficulty)data.chosenDifficulty];
        CreateCardGrid(gameConfig.rows, gameConfig.columns, gameConfig.cardFrontSprites, data.cardStates);
    }

    private void CreateCardGrid(int rows, int columns, Sprite[] cardFrontSprites, List<CardData> cardDataList)
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
            newCard.OnCardFlipped += OnCardFlipped;
        }
    }
}