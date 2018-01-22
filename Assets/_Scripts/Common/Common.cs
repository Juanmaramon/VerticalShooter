using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Common 
{
    public enum Side
    {
        LEFT,
        RIGHT
    }

    public enum Direction
    {
        UP,
        DOWN,
        CUSTOM
    }

    // Event names
    public const string ON_LIVES_CHANGED = "OnLivesChanged";
    public const string ON_GAME_OVER = "OnGameOver";
    public const string ON_RAISE_SCORE = "OnRaiseScore";
    public const string ON_WIN_GAME = "OnWinGame";
}
