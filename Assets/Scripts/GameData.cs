using System;
using System.Collections.Generic;

[Serializable]
public class GameData
{
    public int score;
    public int combo;
    public int chosenDifficulty;
    public List<CardData> cardStates;
}

[Serializable]
public class CardData
{
    public int id;
    public bool isFlipped;
    public bool isMatched;
}