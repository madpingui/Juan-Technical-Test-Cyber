using UnityEngine;

[CreateAssetMenu(fileName = "NewCardGameConfig", menuName = "Card Game/Config")]
public class CardGameConfig : ScriptableObject
{
    [Header("Grid Settings")]
    [Range(2, 5)] public int rows = 2;
    [Range(2, 6)] public int columns = 2;

    [Header("Card Sprites")]
    public Sprite[] cardFrontSprites;
}
