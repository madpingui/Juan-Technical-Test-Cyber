using System;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] private Sprite frontSprite;
    [SerializeField] private Sprite backSprite;

    private SpriteRenderer renderer;
    private bool isFlipped = false;
    private bool isMatched = false;
    public int Id { get; private set; }

    public event Action<Card> CardFlipped;

    private void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        renderer.sprite = backSprite;
    }

    public void Initialize(int id, Sprite frontSprite)
    {
        Id = id;
        this.frontSprite = frontSprite;
    }

    public void Flip()
    {
        if (isMatched) return;
        isFlipped = !isFlipped;

        // Smooth flip animation here
        if (isFlipped)
        {
            //frontSprite.gameObject.SetActive(true);
            //backSprite.gameObject.SetActive(false);
            CardFlipped?.Invoke(this);
        }
        else
        {
            //frontSprite.gameObject.SetActive(false);
            //backSprite.gameObject.SetActive(true);
        }
    }

    public void SetMatched()
    {
        isMatched = true;
        // Play match animation
    }

    public bool IsFlipped()
    {
        return isFlipped;
    }

    public bool IsMatched()
    {
        return isMatched;
    }
}

