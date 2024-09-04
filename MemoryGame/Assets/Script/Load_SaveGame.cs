using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameState
{
    public int gameSize;
    public float time;
    public int score;
    public List<CardData> cards;
}

[System.Serializable]
public class CardData
{
    public int spriteID;
    public bool flipped;
    public bool inactive;
}