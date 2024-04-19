using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Element : MonoBehaviour
{
    public ElementType elementType;
    public int elementScore = 10;

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

    public void MoveToTarget(Vector2 target , Element _moveElement,float duration)
    {
        if (_moveElement == null)
        {
            Debug.LogWarning("MoveToTarget: _moveElement is null!");
            return;
        }
        StartCoroutine(MoveCoroutine(target, _moveElement,duration));
    }

    private IEnumerator MoveCoroutine(Vector2 target ,Element _moveElement,float duration)
    {
        isMoving = true;
        Vector2 startPos = transform.position;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (_moveElement == null)
            {
                isMoving = false;
                yield break;
            }

            float t = elapsedTime / duration;
            transform.position = Vector2.Lerp(startPos, target, t);
            elapsedTime += Time.deltaTime;

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
    Blue,
    Boom
}