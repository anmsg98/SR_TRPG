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

    public enum Type
    {
        melee,
        w_melee,
        range
    }
    
    public State currentState;
    public Type attackType;
    public Vector2Int pos;
    public int hp;
    public int damage;
}
