using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public enum State
    {
       friend,
       enemy
    }
    public State currentState;
    public Vector2Int pos;
}
