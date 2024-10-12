using System;
using System.Collections.Generic;

// Represents the serializable game state
[Serializable]
public class GameData
{
    public int turns;
    public int score;
    public int combo;
    public int chosenDifficulty;
    public List<CardData> cardStates;
}

// Represents the state of an individual card
[Serializable]
public class CardData
{
    public int id;
    public bool isFlipped;
    public bool isMatched;
}

// Pairs a game difficulty with its corresponding configuration
[Serializable]
public class DifficultyConfigPair
{
    public GameDifficulty difficulty;
    public CardGameConfig config;
}

[Serializable]
public enum GameDifficulty
{
    Easy,
    Medium,
    Hard
}
