using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

public class NewBehaviourScript : MonoBehaviour
{
    private bool picked = false;
    
    private Transform pickedPlayer;

    public GameObject[] character;
    public Transform[] camp;
    public Transform Field;
    
    public GameObject alertMessage;
    private Animator alertAnim;
    private TMP_Text alertText;

    private int[,] board = new int[5, 10];
    void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                board[i, j] = 0;
            }
        }
        alertAnim = alertMessage.GetComponent<Animator>();
        alertText = alertMessage.GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        SpawnCharacter();
        AssignCharacter();
    }

    void SpawnCharacter()
    {
        Vector3 mousePos;
        Vector3 touchPos;

        // pc 테스트
        // if (Input.GetMouseButtonDown(0))
        // {
        //     mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //     RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, transform.forward, 15f);
        //     RaycastHit2D hit;
        //     hit = hits[0];
        //
        //     if (hit.transform.tag == "Spawn_b")
        //     {
        //         if (camp[0].childCount < 3)
        //         {
        //             int idx = hit.transform.GetSiblingIndex();
        //             GameObject obj = Instantiate(character[idx]);
        //             obj.transform.SetParent(camp[0], false);
        //             mousePos.z = 1f;
        //             obj.transform.position = mousePos;
        //             picked = true;
        //             pickedPlayer = obj.transform;
        //         }
        //         else
        //         {
        //             alertText.text = "각 진영당 캐릭터 배치 제한은 최대 3유닛 입니다.";
        //             alertAnim.SetTrigger("Alert_On");
        //         }
        //     }
        //
        //     else if (hit.transform.tag == "Spawn_r")
        //     {
        //         if (camp[1].childCount < 3)
        //         {
        //             int idx = hit.transform.GetSiblingIndex();
        //             GameObject obj = Instantiate(character[idx]);
        //             obj.transform.SetParent(camp[1], false);
        //             mousePos.z = 1f;
        //             obj.transform.position = mousePos;
        //             picked = true;
        //             pickedPlayer = obj.transform;
        //         }
        //         else
        //         {
        //             alertText.text = "각 진영당 캐릭터 배치 제한은 최대 3유닛 입니다.";
        //             alertAnim.SetTrigger("Alert_On");
        //         }
        //     }
        // }
        
        // android 터치
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D[] hits = Physics2D.RaycastAll(touchPos, transform.forward, 15f);
                RaycastHit2D hit;
                hit = hits[0];

                if (hit.transform.tag == "Spawn_b")
                {
                    if (camp[0].childCount < 3)
                    {
                        int idx = hit.transform.GetSiblingIndex();
                        GameObject obj = Instantiate(character[idx]);
                        obj.transform.SetParent(camp[0], false);
                        touchPos.z = 1f;
                        obj.transform.position = touchPos;
                        picked = true;
                        pickedPlayer = obj.transform;
                    }
                    else
                    {
                        alertText.text = "각 진영당 캐릭터 배치 제한은 최대 3유닛 입니다.";
                        alertAnim.SetTrigger("Alert_On");
                    }
                }

                else if (hit.transform.tag == "Spawn_r")
                {
                    if (camp[1].childCount < 3)
                    {
                        int idx = hit.transform.GetSiblingIndex();
                        GameObject obj = Instantiate(character[idx]);
                        obj.transform.SetParent(camp[1], false);
                        touchPos.z = 1f;
                        obj.transform.position = touchPos;
                        picked = true;
                        pickedPlayer = obj.transform;
                    }
                    else
                    {
                        alertText.text = "각 진영당 캐릭터 배치 제한은 최대 3유닛 입니다.";
                        alertAnim.SetTrigger("Alert_On");
                    }
                }
            }
        }
    }

    void AssignCharacter()
    {
        Vector3 mousePos;
        Vector3 touchPos;
        // pc 테스트
        // if (Input.GetMouseButtonDown(0))
        // {
        //     mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //     RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, transform.forward, 15f);
        //     RaycastHit2D hit;
        //     hit = hits[hits.Length - 1];
        //
        //     if (hit.transform.tag == "Player")
        //     {
        //         picked = true;
        //         pickedPlayer = hit.transform;
        //     }
        // }
        // else if (Input.GetMouseButton(0))
        // {
        //     if (picked)
        //     {
        //         mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //         mousePos.z = 1f;
        //         pickedPlayer.position = mousePos;
        //     }
        // }
        // else if (Input.GetMouseButtonUp(0))
        // {
        //     mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //     RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, transform.forward, 15f);
        //     RaycastHit2D hit;
        //     hit = hits[0];
        //     if (picked)
        //     {
        //         if (hit.transform.tag == "Board")
        //         {
        //             // 이동한 보드 위치에 캐릭터가 없을 때
        //             Vector2Int pos = hit.transform.GetComponent<Board>().pos;
        //             if (board[pos.x, pos.y] == 0)
        //             {
        //                 // 해당 보드 위치로 옮김
        //                 Vector3 boardPosition;
        //                 boardPosition = hit.transform.position;
        //                 boardPosition.z = 1f;
        //                 pickedPlayer.position = boardPosition;
        //
        //                 // 캐릭터의 해당 좌표도 보드 좌표로 변경
        //                 pos = pickedPlayer.GetComponent<Character>().pos;
        //                 
        //                 // 캐릭터가 처음 생성된 경우가 아닐때
        //                 if (pos.x != -1 || pos.y != -1)
        //                 {
        //                     board[pos.x, pos.y] = 0;
        //                 }
        //
        //                 pos = hit.transform.GetComponent<Board>().pos;
        //                 if (pickedPlayer.GetComponent<Character>().currentState == Character.State.friend)
        //                     board[pos.x, pos.y] = 1;
        //                 else board[pos.x, pos.y] = 2;
        //                 pickedPlayer.GetComponent<Character>().pos = pos;
        //             }
        //             // 이동한 보드 위치에 이미 캐릭터가 있을 때
        //             else
        //             {
        //                 Vector2Int characterPos = pickedPlayer.GetComponent<Character>().pos;
        //                 // 처음 생성된 캐릭터면
        //                 if (characterPos.x == -1 && characterPos.y == -1)
        //                 {
        //                     Destroy(pickedPlayer.gameObject);
        //                 }
        //                 // 이미 배치된 캐릭터인 경우
        //                 else
        //                 {
        //                     //원래 있던 보드로 옮김
        //                     Vector2Int ps = pickedPlayer.GetComponent<Character>().pos;
        //                     int idx = (ps.x * 10 + ps.y);
        //                     Vector3 boardPosition;
        //                     boardPosition = Field.GetChild(idx).transform.position;
        //                     boardPosition.z = 1f;
        //                     pickedPlayer.position = boardPosition;
        //                 }
        //
        //                 alertText.text = "이동한 보드에 이미 캐릭터가 배치되어 있습니다.";
        //                 alertAnim.SetTrigger("Alert_On");
        //             }
        //         }
        //         else
        //         {
        //             // 보드 외의 위치에 놓았을 때, 캐릭터 삭제
        //             Vector2Int pos = pickedPlayer.GetComponent<Character>().pos;
        //             if (pos.x != -1 || pos.y != -1)
        //             {
        //                 board[pos.x, pos.y] = 0;
        //             }
        //
        //             Destroy(pickedPlayer.gameObject);
        //         }
        //     }
        //
        //     picked = false;
        // }

        // 안드로이드 터치
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D[] hits = Physics2D.RaycastAll(touchPos, transform.forward, 15f);
                RaycastHit2D hit;
                hit = hits[hits.Length - 1];
        
                if (hit.transform.tag == "Player")
                {
                    picked = true;
                    pickedPlayer = hit.transform;
                }
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                if (picked)
                {
                    touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    touchPos.z = 1f;
                    pickedPlayer.position = touchPos;
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D[] hits = Physics2D.RaycastAll(touchPos, transform.forward, 15f);
                RaycastHit2D hit;
                hit = hits[0];
                if (picked)
                {
                    if (hit.transform.tag == "Board")
                    {
                        // 이동한 보드 위치에 캐릭터가 없을 때
                        Vector2Int pos = hit.transform.GetComponent<Board>().pos;
                        if (board[pos.x, pos.y] == 0)
                        {
                            // 해당 보드 위치로 옮김
                            Vector3 boardPosition;
                            boardPosition = hit.transform.position;
                            boardPosition.z = 1f;
                            pickedPlayer.position = boardPosition;

                            // 캐릭터의 해당 좌표도 보드 좌표로 변경
                            pos = pickedPlayer.GetComponent<Character>().pos;

                            // 캐릭터가 처음 생성된 경우가 아닐때
                            if (pos.x != -1 || pos.y != -1)
                            {
                                board[pos.x, pos.y] = 0;
                            }

                            pos = hit.transform.GetComponent<Board>().pos;
                            if (pickedPlayer.GetComponent<Character>().currentState == Character.State.friend)
                                board[pos.x, pos.y] = 1;
                            else board[pos.x, pos.y] = 2;
                            pickedPlayer.GetComponent<Character>().pos = pos;
                        }
                        // 이동한 보드 위치에 이미 캐릭터가 있을 때
                        else
                        {
                            Vector2Int characterPos = pickedPlayer.GetComponent<Character>().pos;
                            // 처음 생성된 캐릭터면
                            if (characterPos.x == -1 && characterPos.y == -1)
                            {
                                Destroy(pickedPlayer.gameObject);
                            }
                            // 이미 배치된 캐릭터인 경우
                            else
                            {
                                //원래 있던 보드로 옮김
                                Vector2Int ps = pickedPlayer.GetComponent<Character>().pos;
                                int idx = (ps.x * 10 + ps.y);
                                Vector3 boardPosition;
                                boardPosition = Field.GetChild(idx).transform.position;
                                boardPosition.z = 1f;
                                pickedPlayer.position = boardPosition;
                            }

                            alertText.text = "이동한 보드에 이미 캐릭터가 배치되어 있습니다.";
                            alertAnim.SetTrigger("Alert_On");
                        }
                    }
                    else
                    {
                        // 보드 외의 위치에 놓았을 때, 캐릭터 삭제
                        Vector2Int pos = pickedPlayer.GetComponent<Character>().pos;
                        if (pos.x != -1 || pos.y != -1)
                        {
                            board[pos.x, pos.y] = 0;
                        }

                        Destroy(pickedPlayer.gameObject);
                    }
                }
                picked = false;
            }
        }
    }

    void RemoveCharacter()
    {
        
    }
}
