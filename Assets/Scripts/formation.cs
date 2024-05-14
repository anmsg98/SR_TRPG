using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

public class Formation : MonoBehaviour
{
    // 캐릭터을 터치 & 드래그로 집고 있는 상태
    private bool picked = false;
    
    // 선택된 캐릭터
    private Transform pickedCharacter;

    // 캐릭터 프리펩들
    public GameObject[] character;
    
    void Start()
    {
        // 보드(맵)을 0(플레이어가 있지 않은 상태)으로 초기화
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                Gamemanager.instance.map[i, j] = 0;
            }
        }
    }
    void Update()
    {
        // 도움말이 켜져있지 않고 게임 시작전에만 조작 가능
        if (!Gamemanager.instance.onPlay && !Gamemanager.instance.guideOn)
        {
            SpawnCharacter();
            AssignCharacter();
        }
    }

    void SpawnCharacter() // 캐릭터 생성
    {
        Vector3 touchPos;
        
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                // 레이캐스팅을 이용해 UI 터치
                touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D[] hits = Physics2D.RaycastAll(touchPos, transform.forward, 15f);
                RaycastHit2D hit;
                if (hits.Length > 0)
                {
                    hit = hits[0];
                    // 아군 스폰 UI를 터치 했을때
                    if (hit.transform.tag == "Spawn_b")
                    {
                        if (Gamemanager.instance.camp[0].childCount < 3)
                        {
                            // 아군 캠프에 캐릭터를 자식으로 넣음(공격 하는 순서, 먼저 넣으면 먼저 행동함)
                            int idx = hit.transform.GetSiblingIndex();
                            GameObject obj = Instantiate(character[idx]);
                            obj.transform.SetParent(Gamemanager.instance.camp[0], false);
                            touchPos.z = 1f;
                            obj.transform.position = touchPos;
                            picked = true;
                            pickedCharacter = obj.transform;
                        }
                        
                        else
                        {
                            // 캠프안에 캐릭수가 3을 넘어 갔을 시
                            Gamemanager.instance.alertText.text = "각 진영당 캐릭터 배치 제한은 최대 3유닛 입니다.";
                            Gamemanager.instance.alertAnim.SetTrigger("Alert_On");
                        }
                    }

                    // 적군 스폰 UI 터치 했을때
                    else if (hit.transform.tag == "Spawn_r")
                    {
                        if (Gamemanager.instance.camp[1].childCount < 3)
                        {
                            
                            int idx = hit.transform.GetSiblingIndex();
                            GameObject obj = Instantiate(character[idx]);
                            obj.transform.SetParent(Gamemanager.instance.camp[1], false);
                            touchPos.z = 1f;
                            obj.transform.position = touchPos;
                            picked = true;
                            pickedCharacter = obj.transform;
                        }
                        
                        else
                        {
                            Gamemanager.instance.alertText.text = "각 진영당 캐릭터 배치 제한은 최대 3유닛 입니다.";
                            Gamemanager.instance.alertAnim.SetTrigger("Alert_On");
                        }
                    }
                }
            }
        }
    }

    void AssignCharacter() // 캐릭터 배치
    {
        Vector3 touchPos;
        
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D[] hits = Physics2D.RaycastAll(touchPos, transform.forward, 15f);
                RaycastHit2D hit;
                if (hits.Length > 0)
                {
                    hit = hits[hits.Length - 1];

                    if (hit.transform.tag == "Player")
                    {
                        picked = true;
                        pickedCharacter = hit.transform;
                    }
                }
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                if (picked)
                {
                    touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    touchPos.z = 1f;
                    pickedCharacter.position = touchPos;
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D[] hits = Physics2D.RaycastAll(touchPos, transform.forward, 15f);
                RaycastHit2D hit;
                if (hits.Length > 0) 
                {
                    hit = hits[0];

                    if (picked)
                    {
                        if (hit.transform.tag == "Board")
                        {
                            // 이동한 보드 위치에 캐릭터가 없을 때
                            Vector2Int pos = hit.transform.GetComponent<Board>().pos;
                            if (Gamemanager.instance.map[pos.x, pos.y] == 0)
                            {
                                // 해당 보드 위치로 옮김
                                Vector3 boardPosition;
                                boardPosition = hit.transform.position;
                                boardPosition.z = 1f;
                                pickedCharacter.position = boardPosition;

                                // 캐릭터의 해당 좌표도 보드 좌표로 변경
                                pos = pickedCharacter.GetComponent<Character>().pos;

                                // 캐릭터가 처음 생성된 경우가 아닐때
                                if (pos.x != -1 || pos.y != -1)
                                {
                                    Gamemanager.instance.map[pos.x, pos.y] = 0;
                                }

                                pos = hit.transform.GetComponent<Board>().pos;
                                if (pickedCharacter.GetComponent<Character>().currentState == Character.State.friend)
                                    Gamemanager.instance.map[pos.x, pos.y] = 1;
                                else Gamemanager.instance.map[pos.x, pos.y] = 2;
                                pickedCharacter.GetComponent<Character>().pos = pos;
                            }
                            // 이동한 보드 위치에 이미 캐릭터가 있을 때
                            else
                            {
                                Vector2Int characterPos = pickedCharacter.GetComponent<Character>().pos;
                                // 처음 생성된 캐릭터면
                                if (characterPos.x == -1 && characterPos.y == -1)
                                {
                                    Destroy(pickedCharacter.gameObject);
                                }
                                // 이미 배치된 캐릭터인 경우
                                else
                                {
                                    //원래 있던 보드로 옮김
                                    Vector2Int ps = pickedCharacter.GetComponent<Character>().pos;
                                    int idx = (ps.x * 10 + ps.y);
                                    Vector3 boardPosition;
                                    boardPosition = Gamemanager.instance.Field.GetChild(idx).transform.position;
                                    boardPosition.z = 1f;
                                    pickedCharacter.position = boardPosition;
                                }

                                Gamemanager.instance.alertText.text = "이동한 보드에 이미 캐릭터가 배치되어 있습니다.";
                                Gamemanager.instance.alertAnim.SetTrigger("Alert_On");
                            }
                        }
                        else
                        {
                            // 보드 외의 위치에 놓았을 때, 캐릭터 삭제
                            Vector2Int pos = pickedCharacter.GetComponent<Character>().pos;
                            if (pos.x != -1 || pos.y != -1)
                            {
                                Gamemanager.instance.map[pos.x, pos.y] = 0;
                            }

                            Destroy(pickedCharacter.gameObject);
                        }
                    }
                }

                picked = false;
            }
        }
    }
}
