using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    public enum State // 아군, 적군
    {
       friend,
       enemy
    }

    public enum Type // 근접, 광역근접, 원거리
    {
        melee,
        w_melee,
        range
    }

    // 남은 체력 비례 이미지 조절
    public Image hp_Fill;
    // 팀, 아군 판별
    public State currentState;
    // 공격 타입 (근접, 원거리...)
    public Type attackType;
    // 보드 상의 현재 위치
    public Vector2Int pos;
    public int _hp;
    public int _damage;
    // 최대 체력
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
        SetHP();
    }

    public void Attack(Transform target)
    {
        // 타겟의 정보를 받아 해당 캐릭터(적)의 체력을 깎음
        target.GetComponent<Character>().hp -= damage;
        // 피격 이펙트 애니메이션 발동
        StartCoroutine(Gamemanager.instance.AttackEffect(target));
    }
    
    private void SetHP()
    {
        // 이미지 타입의 Fill Amout 변수 조절 (현재 체력 비례)
        hp_Fill.fillAmount = (float) hp / (float) maxHp;
        
        // 죽었을 때
        if (hp <= 0)
        {
            // 맵 상의 좌표를 삭제하고
            Gamemanager.instance.map[pos.x, pos.y] = 0;
            // 아군 및 적군 킬 카운트
            if (currentState == State.friend)
                Gamemanager.instance.deadCount[0]++;
            else
                Gamemanager.instance.deadCount[1]++;
            // 죽은 캐릭터는 오브젝트 비활성화
            gameObject.SetActive(false);
        }
    }
}
