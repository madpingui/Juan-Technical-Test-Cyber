using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CardFlipAnimator : MonoBehaviour
{
    [SerializeField] private Sprite frontSprite;
    [SerializeField] private Sprite backSprite;
    [SerializeField] private Image cardFigureImage;

    private Button cardBackground;
    private bool isFlipping = false;

    private void Awake()
    {
        cardBackground = GetComponent<Button>();
        ((Image)cardBackground.targetGraphic).sprite = backSprite;
    }

    public void SetSprite(Sprite cardFigure)
    {
        cardFigureImage.sprite = cardFigure;
    }

    //If we load a card that is already matched we want to make sure we already have setted up the flipped graphics.
    public void EnsureFlippedUp()
    {
        ((Image)cardBackground.targetGraphic).sprite = frontSprite;
        cardFigureImage.gameObject.SetActive(true);
    }

    public void PlayFlip(bool isFlipped, Action onComplete = null) => StartCoroutine(PlayFlipAnimation(isFlipped, onComplete));
    private IEnumerator PlayFlipAnimation(bool isFlipped, Action onComplete = null)
    {
        if (isFlipping) yield break;
        isFlipping = true;
        float duration = 0.15f;
        float elapsed = 0f;

        // Phase 1: Rotate to 90 degrees
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float angle = Mathf.Lerp(0f, 90f, elapsed / duration);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            yield return null;
        }

        // Change sprite at 90 degrees
        ((Image)cardBackground.targetGraphic).sprite = isFlipped ? backSprite : frontSprite;
        cardFigureImage.gameObject.SetActive(!isFlipped);

        elapsed = 0f; // Reset for phase 2

        // Phase 2: Rotate back from 90 to 0 degrees
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float angle = Mathf.Lerp(90f, 0f, elapsed / duration);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            yield return null;
        }

        isFlipping = false;

        // Mark the animation as completed
        onComplete?.Invoke();
    }

    [SerializeField] private AnimationCurve scaleCurve;
    public void PlayMatch() => StartCoroutine(PlayMatchAnimation());
    private IEnumerator PlayMatchAnimation()
    {
        cardBackground.interactable = false;
        Vector3 initialScale = transform.localScale;
        float elapsedTime = 0f;
        float animationDuration = scaleCurve[scaleCurve.length - 1].time;

        // Use the curve to control the scaling
        while (elapsedTime < animationDuration)
        {
            float progress = elapsedTime / animationDuration;
            transform.localScale = initialScale * scaleCurve.Evaluate(progress);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = initialScale;
    }
}
