using System.Collections.Generic;
using UnityEngine;
using System;

public class MatchHandler : MonoBehaviour
{
    private List<Card> currentMatchCheck = new List<Card>();

    public static event Action OnCardMatch;
    public static event Action OnCardMiss;

    public void OnCardFlipped(Card card)
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
        else HandleMismatch();

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

    public void Clear() => currentMatchCheck.Clear();
}
