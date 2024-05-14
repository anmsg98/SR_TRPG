using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Vector2Int pos;
    private void Start()
    {
        int x = (int) transform.GetSiblingIndex() / 10;
        int y = transform.GetSiblingIndex() % 10;
        pos.x = x;
        pos.y = y;
    }
}
