using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool IsUsable;

    public GameObject mineral;

    public Tile(bool _IsUsable, GameObject _mineral)
    {
        IsUsable = _IsUsable;
        mineral = _mineral;
    }
}
