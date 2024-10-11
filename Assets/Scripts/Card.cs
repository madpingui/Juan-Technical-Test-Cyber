using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] private Sprite frontSprite;
    [SerializeField] private Sprite backSprite;
    [SerializeField] private Image cardFigureImage;

    private Button cardBackground;
    private bool isFlipped = false;
    private bool isMatched = false;
    private bool isFlipping = false;

    public int Id { get; private set; }
    public event Action<Card> CardFlipped;

    private void Start()
    {
        cardBackground = GetComponent<Button>();
        ((Image)cardBackground.targetGraphic).sprite = backSprite;
    }

    public void Initialize(int id, Sprite figure)
    {
        Id = id;
        this.cardFigureImage.sprite = figure;
    }

    //Called when card is clicked via Button Event
    public void Flip()
    {
        if (isMatched || isFlipped) return;
        StartCoroutine(FlipAnimation(isFlipped));
        isFlipped = !isFlipped;
    }

    public void ResetCard()
    {
        if (isMatched) return;
        StopAllCoroutines();
        isFlipping = false;
        StartCoroutine(FlipAnimation(isFlipped));
        isFlipped = !isFlipped;
    }

    private IEnumerator FlipAnimation(bool isFlipped)
    {
        if (isFlipping) yield break; // Prevent multiple flips at once

        isFlipping = true;
        float duration = 0.15f; // Time to reach 90 degrees (halfway point)
        float elapsed = 0f;

        // Phase 1: Rotate from 0 to 90 degrees
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float angle = Mathf.Lerp(0f, 90f, t); // Interpolate from 0 to 90 degrees

            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            yield return null; // Wait for the next frame
        }

        // Change the sprite after reaching 90 degrees
        ((Image)cardBackground.targetGraphic).sprite = isFlipped ? backSprite : frontSprite;
        cardFigureImage.gameObject.SetActive(!isFlipped);

        elapsed = 0f; // Reset the timer for the second phase

        // Phase 2: Rotate back from 90 to 0 degrees
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float angle = Mathf.Lerp(90f, 0f, t); // Interpolate from 90 to 0 degrees

            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            yield return null; // Wait for the next frame
        }

        // Mark the animation as completed
        if(!isFlipped)
            CardFlipped?.Invoke(this);
        isFlipping = false;
    }

    public void SetMatched()
    {
        isMatched = true;
        cardBackground.interactable = false;
        StartCoroutine(PlayMatchAnimation());
        // Play match animation
    }

    [SerializeField] private AnimationCurve popCurve;
    private IEnumerator PlayMatchAnimation()
    {
        Vector3 initialScale = transform.localScale;
        float elapsedTime = 0f;
        float animationDuration = popCurve[popCurve.length - 1].time;

        // Use the animation curve to control the scaling
        while (elapsedTime < animationDuration)
        {
            float progress = elapsedTime / animationDuration;
            transform.localScale = initialScale * popCurve.Evaluate(progress);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = initialScale; // Ensure the card returns to its original scale
    }

    public bool IsFlipped() => isFlipped;
    public bool IsMatched() => isMatched;
}

