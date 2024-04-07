using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mineral : MonoBehaviour
{
    public MineralType mineral;

    public int xIndex;
    public int yIndex;

    public bool IsMatched;
    Vector2 currentPos;
    Vector2 targetPos;

    public bool IsMoving;

    public Mineral(int x, int y)
    {
        xIndex = x;
        yIndex = y;
    }

    public void SetIndicies(int x, int y)
    {
        xIndex = x;
        yIndex = y;
    }
}
public enum MineralType
{
    Purple,
    Green,
    Orange,
    Blue
}

