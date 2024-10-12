using System;
using UnityEngine;

[RequireComponent(typeof(CardFlipAnimator))]
public class Card : MonoBehaviour
{
    [SerializeField] private CardFlipAnimator flipAnimator;

    private bool isFlipped = false;
    private bool isMatched = false;

    public int Id { get; private set; }
    public static event Action<Card> OnCardFlipped;

    public void Initialize(int id, Sprite figure)
    {
        Id = id;
        flipAnimator.SetSprite(figure);
    }

    public void FlipUpCard(bool triggerEvent = true)
    {
        if (isMatched || isFlipped) return;
        flipAnimator.PlayFlip(false, triggerEvent ? () => OnCardFlipped?.Invoke(this) : null); // Flip the card and optionally trigger the OnCardFlipped event
        isFlipped = true;
        SoundManager.Instance.PlayCardFlip();
    }

    public void FlipDownCard()
    {
        if (isMatched) return;
        flipAnimator.PlayFlip(true, () => isFlipped = false);
    }

    public void SetMatched()
    {
        isMatched = true;
        isFlipped = true;

        // Ensure the card is visually flipped up and play the match animation
        flipAnimator.EnsureFlippedUp();
        flipAnimator.PlayMatch();
    }

    public bool IsFlipped() => isFlipped;
    public bool IsMatched() => isMatched;
}

