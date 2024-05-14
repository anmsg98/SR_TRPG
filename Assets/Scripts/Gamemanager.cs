using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class Gamemanager : MonoBehaviour
{
    public static Gamemanager instance { get; set; }
    
    // 맵 정보(캐릭터가 있는지 없는지)
    public int[,] map = new int[5, 10];

    // 캐릭터 공격 순서
    public List<GameObject> entityOrder = new List<GameObject>();
    // 아군 적군 리스트
    public Transform[] camp;
    // 보드판(5x10)이 들어있는 부모 오브젝트
    public Transform Field;
    
    // BFS 알고리즘 변수
    private bool[,] visited = new bool[5, 10];
    private int[,] disMap = new int[5, 10];

    private int[] dx = new int[8] {-1, 1, 0, 0, -1, 1, -1, 1};
    private int[] dy = new int[8] {0, 0, -1, 1, 1, -1, -1, 1};

    // 현재 위치 (행동중인 캐릭터)
    private Vector2Int currentPos;
    
    // 가까운 타겟 (행동중인 캐릭터 기준)
    private Transform nearestTarget;
    // 타겟들 (광역 공격용)
    private List<Transform> targets = new List<Transform>();
    // 제일 가까운 타겟 좌표
    private Vector2Int targetPos;
    
    // 아군 적군 킬 카운트
    public int[] deadCount = new int [2];
    
    // 게임시작, 도움말, 턴오버, 게임 오버 등등 판별 변수
    public bool onPlay = false;
    public bool guideOn = false;
    private bool canMove = false;
    private bool turnEnd = false;
    private bool gameOver = false;
    
    // 경고 텍스트 (캐릭터 배치 할 때)
    public Animator alertAnim;
    public TMP_Text alertText;
    
    // 도움말 UI
    public GameObject guide;

    // 맵 애니메이션 및 시작시 비활성화 할 오브젝트들
    public GameObject entity;
    public GameObject[] disableAtStart;
    
    // 남은 시간 및 현재 진행 턴 수
    public GameObject timeText;
    public GameObject turnText;
    private float time = 61f;
    private int turnCnt = 1;
    
    // 피격 이펙트(애니메이션)
    public GameObject attackEffect;
    
    // 결과 창
    public GameObject result;
    public TMP_Text resultText;
    
    private void Awake()
    {
        Application.targetFrameRate = 60;
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }
    
    void Update()
    {
        if (!gameOver)
        {
            WinLoseDecision();

            if (turnEnd)
            {
                StartCoroutine(TurnStart());
                turnEnd = false;
            }

            if (onPlay)
            {
                CheckTime();
            }
        }
    }

    void SetOfAction(GameObject Entity) // 캐릭터 행동 결정 (이동 or 공격)
    {
        // 타겟 리스트를 비워줌
        targets.Clear();
        // 행동을 할 캐릭터의 현재 위치
        currentPos = Entity.GetComponent<Character>().pos;
        // 거리 임시 변수
        float distance = 99f;
        // 타겟을 구함
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if (Entity.transform.GetComponent<Character>().currentState == Character.State.friend)
                {
                    if (map[i, j] == 2) // 적군(2)일 때
                    {
                        // 식별된 타겟의 거리가 전 타겟 거리보다 가까우면 타겟 변경
                        if (distance >= Vector2Int.Distance(currentPos, new Vector2Int(i, j)))
                        {
                            for (int k = 0; k < 3; k++)
                            {
                                if (camp[1].GetChild(k).GetComponent<Character>().pos.x == i &&
                                    camp[1].GetChild(k).GetComponent<Character>().pos.y == j)
                                {
                                    nearestTarget = camp[1].GetChild(k);
                                }
                            }
                            distance = Vector2Int.Distance(currentPos, new Vector2Int(i, j));
                            targetPos = new Vector2Int(i, j);
                        }
                        // 근접 광역 캐릭터용 타겟 정보 수집
                        if (Vector2Int.Distance(currentPos, new Vector2Int(i, j)) < 2f) 
                        {
                            for (int k = 0; k < 3; k++)
                            {
                                if (camp[1].GetChild(k).GetComponent<Character>().pos.x == i &&
                                    camp[1].GetChild(k).GetComponent<Character>().pos.y == j)
                                {
                                    targets.Add(camp[1].GetChild(k));
                                }
                            }
                        }
                    }
                }
                
                else
                {
                    if (map[i, j] == 1) // 적군(1)일 때
                    {
                        if (distance >= Vector2Int.Distance(currentPos, new Vector2Int(i, j)))
                        {
                            for (int k = 0; k < 3; k++)
                            {
                                if (camp[0].GetChild(k).GetComponent<Character>().pos.x == i &&
                                    camp[0].GetChild(k).GetComponent<Character>().pos.y == j)
                                {
                                    nearestTarget = camp[0].GetChild(k);
                                }
                            }
                            
                            distance = Vector2Int.Distance(currentPos, new Vector2Int(i, j));
                            targetPos = new Vector2Int(i, j);
                        }
                        
                        if (Vector2Int.Distance(currentPos, new Vector2Int(i, j)) < 2f) 
                        {
                            for (int k = 0; k < 3; k++)
                            {
                                if (camp[0].GetChild(k).GetComponent<Character>().pos.x == i &&
                                    camp[0].GetChild(k).GetComponent<Character>().pos.y == j)
                                {
                                    targets.Add(camp[0].GetChild(k));
                                }
                            }
                        }
                    }
                }
            }
        }

        // 타겟이 공격거리에 도달했을때
        int attackDistance = Math.Abs(currentPos.x - targetPos.x) + Math.Abs(currentPos.y - targetPos.y);
        if (Entity.transform.GetComponent<Character>().attackType == Character.Type.range) // 원거리
        {
            // 공격거리 (원거리는 3) 이상일때 이동
            if (attackDistance > 3)
                canMove = true;
            // 이하이면 공격
            else
                Attack(Entity, nearestTarget);
        }
        
        else if(Entity.transform.GetComponent<Character>().attackType == Character.Type.melee) // 근거리
        {
            if (attackDistance > 2)
                canMove = true;
            else
                Attack(Entity, nearestTarget);
        }
        
        else // 근거리 광역
        {
            if (attackDistance > 2)
                canMove = true;
            else
                WideAttack(Entity, targets);
        }
        
        // BFS를 이용한 타겟으로 향하는 최단거리 임시 변수
        int minDis = 99;
        if (canMove) 
        {
            // 타겟에게 향하는 최단 거리(방향)을 구함
            Vector2Int dirPos = new Vector2Int(0, 0);
            for (int i = 0; i < 4; i++)
            {
                if ((currentPos.x + dx[i] >= 0 && currentPos.x + dx[i] < 5)
                    && (currentPos.y + dy[i] >= 0 && currentPos.y + dy[i] < 10))
                {
                    // BFS 변수들을 초기화 시켜주고
                    Reset();
                    // 최단거리 저장 변수
                    int cnt;
                    if (Entity.transform.GetComponent<Character>().currentState == Character.State.friend)
                    {
                        // BFS를 통해 최단 거리 계산
                        cnt = BFS(currentPos.x + dx[i], currentPos.y + dy[i], targetPos.x, targetPos.y, true);
                    }
                    else
                    {
                        cnt = BFS(currentPos.x + dx[i], currentPos.y + dy[i], targetPos.x, targetPos.y,false);
                    }
                    if (minDis >= cnt)
                    {
                        minDis = cnt;
                        // 가려던 방향에 캐릭터가 없을 때만
                        if (map[currentPos.x + dx[i], currentPos.y + dy[i]] == 0) 
                            dirPos = new Vector2Int(currentPos.x + dx[i], currentPos.y + dy[i]);
                    }
                }
            }
            // 이동시 달리는 애니메이션 ON
            Entity.transform.GetComponent<Animator>().SetTrigger("Run");
            // 맵 현황을 바꾸고 해당 캐릭터를 이동
            map[currentPos.x, currentPos.y] = 0;
            currentPos = dirPos;
            if (Entity.transform.GetComponent<Character>().currentState == Character.State.friend)
                map[currentPos.x, currentPos.y] = 1;
            else
                map[currentPos.x, currentPos.y] = 2;
            
            // 보드판 index
            int idx = currentPos.x * 10 + currentPos.y;
            Vector3 newPos = Field.GetChild(idx).transform.position;
            newPos.z = 1f;
            Entity.transform.GetComponent<Character>().pos = currentPos;
            Entity.transform.position = newPos;
            
            canMove = false;
        }
    }

    public void Attack(GameObject Entity, Transform target)
    {
        Entity.transform.GetComponent<Character>().Attack(target); 
        Entity.transform.GetComponent<Animator>().SetTrigger("Attack");
    }

    public void WideAttack(GameObject Entity, List<Transform> target)
    {
        for (int i = 0; i < target.Count; i++)
        {
            Entity.transform.GetComponent<Character>().Attack(target[i]); 
        }
        Entity.transform.GetComponent<Animator>().SetTrigger("Attack");
    }
    
    public IEnumerator TurnStart()
    {
        // 공격순서 리스트에 있는 캐릭터들을 순회해 한번씩 setofaction 함수 호출
        for (int j = 0; j < entityOrder.Count; j++)
        {
            if (entityOrder[j].activeInHierarchy)
            {
                SetOfAction(entityOrder[j]);
                yield return new WaitForSeconds(1f);
            }
        }
        // 마지막 캐릭터 까지 행동 했으면 턴 종료
        turnEnd = true;
        turnCnt += 1;
        turnText.GetComponent<TMP_Text>().text = "TURN  " + turnCnt.ToString();
    }

    public IEnumerator AttackEffect(Transform Target)
    {
        GameObject attackObj = Instantiate(Gamemanager.instance.attackEffect, Target.position, Quaternion.identity);
        attackObj.transform.GetComponent<Animator>().SetTrigger("Attack");
        attackObj.transform.SetParent(entity.transform);
        yield return new WaitForSeconds(0.4f);
        Destroy(attackObj);
    }

    private int BFS(int startx, int starty, int endx, int endy, bool isBlue) // BFS 알고리즘
    {
        Queue<(int, int)> queue = new Queue<(int, int)>();
        queue.Enqueue((startx, starty));
        visited[startx, starty] = true;
        int cnt = 0;
        while (queue.Count != 0)
        {
            var pos = queue.Dequeue();

            //pop됐을때의 x좌표와 y좌표
            int posx = pos.Item1;
            int posy = pos.Item2;

            visited[posx, posy] = true;

            // 목적지 찾으면 목적지까지 거리 저장하고 반복문 중단
            if (posx == endx && posy == endy)
            {
                cnt = disMap[posx, posy];
                break;
            }
            else
            {
                cnt = 100;
            }

            //상하좌우 체크
            for (int i = 0; i < 4; i++)
            {
                int dirx = pos.Item1 + dx[i];
                int diry = pos.Item2 + dy[i];
                if ((dirx >= 0 && dirx < 5) && (diry >= 0 && diry < 10) && visited[dirx, diry] == false)
                {
                    if (isBlue) // 아군이 적군 찾을 때
                    {
                        if (map[dirx, diry] != 1)
                        {
                            visited[dirx, diry] = true;
                            disMap[dirx, diry] += disMap[posx, posy] + 1;
                            queue.Enqueue((dirx, diry));
                        }
                    }
                    else // 적군이 아군 찾을 때
                    {
                        if (map[dirx, diry] != 2)
                        {
                            visited[dirx, diry] = true;
                            disMap[dirx, diry] += disMap[posx, posy] + 1;
                            queue.Enqueue((dirx, diry));
                        }
                    }
                }
            }
        }

        return cnt;
    }

    public void InsertOrderInfo()
    {
        if (camp[1].childCount + camp[0].childCount != 6)
        {
            alertText.text = "각 진영당 3명의 캐릭터를 배치해야 합니다.";
            alertAnim.SetTrigger("Alert_On");
        }
        
        // 캐릭터 행동 순서 정보 갱신
        else
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < camp[i].childCount; j++)
                {
                    entityOrder.Add(camp[i].GetChild(j).gameObject);
                }
            }
            StartCoroutine(DisableOnStart());
        }
    }
    
    private void Reset() // BFS 변수들 초기화
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                disMap[i, j] = 0;
                visited[i, j] = false;
            }
        }
    }
    
    private IEnumerator DisableOnStart() // 게임 시작시 비활성화 할 오브젝트들 관리
    {
        entity.transform.GetComponent<Animator>().SetTrigger("Start");
        for (int i = 0; i < disableAtStart.Length; i++)
        {
            disableAtStart[i].SetActive(false);
        }
        
        yield return new WaitForSeconds(1f);
        
        timeText.SetActive(true);
        turnText.SetActive(true);
        onPlay = true;
        turnEnd = true;
    }

    public void HelpGuide()
    {
        guide.SetActive(true);
        guideOn = true;
        for (int i = 0; i < disableAtStart.Length; i++)
        {
            disableAtStart[i].SetActive(false);
        }
    }
    
    public void QuidQuit()
    {
        guide.SetActive(false);
        guideOn = false;
        for (int i = 0; i < disableAtStart.Length; i++)
        {
            disableAtStart[i].SetActive(true);
        }
    }
    
    public void GameQuit()
    {
        Application.Quit();
    }
    
    private void WinLoseDecision()
    {
        if (deadCount[0] == 3)
        {
            gameOver = true;
            result.SetActive(true);
            resultText.text = "레드팀 승리!";
            resultText.color = Color.red;
        }

        if (deadCount[1] == 3)
        {
            gameOver = true;
            result.SetActive(true);
            resultText.text = "블루팀 승리!";
            resultText.color = Color.blue;
        }
    }

    private void CheckTime()
    {
        time -= Time.deltaTime;
        int timeToInt = (int) time;
        if (timeToInt <= 10)
        {
            timeText.GetComponent<TMP_Text>().color = Color.red;
            if (time < 0f && (deadCount[0] != 3 || deadCount[1] != 3)) 
            {
                gameOver = true;
                result.SetActive(true);
                resultText.text = "무 승 부";
                resultText.color = Color.white;
            }
        }
        timeText.GetComponent<TMP_Text>().text = timeToInt.ToString();
    }
}
