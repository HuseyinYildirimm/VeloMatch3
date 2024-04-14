using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Element : MonoBehaviour
{
    public ElementType elementType;

    public int xIndex;
    public int yIndex;

    public bool isMatched;
    private Vector2 currentPos;
    private Vector2 targetPos;

    public bool isMoving;
    

    public Element(int _x, int _y)
    {
        xIndex = _x;
        yIndex = _y;
    }

    public void SetIndex(int _x, int _y)
    {
        xIndex = _x;
        yIndex = _y;
    }

    public void MoveToTarget(Vector2 target)
    {
        StartCoroutine(MoveCoroutine(target));
    }

    private IEnumerator MoveCoroutine(Vector2 target)
    {
        isMoving = true;
        Vector2 startPos = transform.position;

        float duration = 0.2f;
        float elaspedTime = 0f;

        while (elaspedTime < duration)
        {
            float t = elaspedTime / duration;
            transform.position = Vector2.Lerp(startPos, target, t);
            elaspedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = target;
        isMoving = false;
    }
}

public enum ElementType
{
    Red,
    White,
    Black,
    Green,
    Blue
}