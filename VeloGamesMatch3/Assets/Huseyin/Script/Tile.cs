using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isUsable;

    public GameObject element;

    public Tile(bool _isUsable , GameObject _element)
    {
        element = _element;
        isUsable = _isUsable;
    }
}
