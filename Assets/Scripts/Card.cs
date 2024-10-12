using System;
using UnityEngine;

[RequireComponent(typeof(FlipAnimator))]
public class Card : MonoBehaviour
{
    [SerializeField] private FlipAnimator flipAnimator;

    private bool isFlipped = false;
    private bool isMatched = false;

    public int Id { get; private set; }
    public event Action<Card> CardFlipped;

    public void Initialize(int id, Sprite figure)
    {
        Id = id;
        flipAnimator.SetSprite(figure);
    }

    public void FlipUpCard()
    {
        if (isMatched || isFlipped) return;
        flipAnimator.PlayFlip(false, () => CardFlipped?.Invoke(this));
        isFlipped = true;
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

