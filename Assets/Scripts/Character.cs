using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public Image hp_Fill;
    public State currentState;
    public Type attackType;
    public Vector2Int pos;
    public int _hp;
    public int _damage;
    private int maxHp;
    public int hp
    {
        set
        {
            _hp = value;
        }

        get
        {
            return _hp;
        }
    }

    public int damage
    {
        set
        {
            if ((value < 0) || (value > int.MaxValue))
            {
                return;
            }

            _damage = value;
        }

        get
        {
            return _damage;
        }
    }
    
    private void Start()
    {
        maxHp = hp;
    }

    private void Update()
    {
        SetHpBar();
    }

    public void Attack(Transform Target)
    {
        Target.GetComponent<Character>().hp -= damage;
    }
    private void SetHpBar()
    {
        hp_Fill.fillAmount = (float) hp / (float) maxHp;
        if (hp <= 0)
        {
            Gamemanager.instance.map[pos.x, pos.y] = 0;
            if (currentState == State.friend)
                Gamemanager.instance.deadCount[0]++;
            else
                Gamemanager.instance.deadCount[1]++;
            gameObject.SetActive(false);
            
        }
    }
}
