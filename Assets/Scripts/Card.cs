using System;
using UnityEngine;

[RequireComponent(typeof(CardFlipAnimator))]
public class Card : MonoBehaviour
{
    [SerializeField] private CardFlipAnimator flipAnimator;

    private bool isFlipped = false;
    private bool isMatched = false;

    public int Id { get; private set; }
    public event Action<Card> OnCardFlipped;

    public void Initialize(int id, Sprite figure)
    {
        Id = id;
        flipAnimator.SetSprite(figure);
    }

    public void FlipUpCard(bool triggerEvent = true)
    {
        if (isMatched || isFlipped) return;
        flipAnimator.PlayFlip(false, triggerEvent ? () => OnCardFlipped?.Invoke(this) : null);
        isFlipped = true;
        SoundManager.Instance.PlayCardFlip();
    }

    public void FlipDownCard()
    {
        if (isMatched) return;
        flipAnimator.PlayFlip(true);
        isFlipped = false;
    }

    public void SetMatched()
    {
        isMatched = true;
        isFlipped = true;

        flipAnimator.EnsureFlippedUp();
        flipAnimator.PlayMatch();
    }

    public bool IsFlipped() => isFlipped;
    public bool IsMatched() => isMatched;
}

