using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Gamemanager : MonoBehaviour
{
    public static Gamemanager instance { get; set; }
    
    public int[,] map = new int[5, 10];

    public List<GameObject> entityOrder = new List<GameObject>();
    public Transform[] camp;
    public Transform Field;
    
    private bool start = false;
    private List<Vector2Int> enemyPos = new List<Vector2Int>();
    
    
    private bool[,] visited = new bool[5, 10];
    private int[,] disMap = new int[5, 10];
    
    private int[] dx = new int[4]{ -1, 1, 0, 0 };
    private int[] dy = new int[4]{ 0, 0, -1, 1 };

    private Vector2Int currentPos;
    private Vector2Int targetPos;
    private bool canMove = false;
    
    private void Awake()
    {
        Application.targetFrameRate = 60;
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FindRoute(GameObject Entity)
    {
        float distance = 99f;
        
        currentPos = Entity.GetComponent<Character>().pos;
        
        // 타겟을 구함
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if (Entity.transform.GetComponent<Character>().currentState == Character.State.friend)
                {
                    if (map[i, j] == 2)
                    {
                        if (distance > Vector2Int.Distance(currentPos, new Vector2Int(i, j)))
                        {
                            distance = Vector2Int.Distance(currentPos, new Vector2Int(i, j));
                            targetPos = new Vector2Int(i, j);
                        }
                    }
                }
                else
                {
                    if (map[i, j] == 1)
                    {
                        if (distance > Vector2Int.Distance(currentPos, new Vector2Int(i, j)))
                        {
                            distance = Vector2Int.Distance(currentPos, new Vector2Int(i, j));
                            targetPos = new Vector2Int(i, j);
                        }
                    }
                }
            }
        }

        Debug.Log(targetPos);
        // 타겟이 공격거리에 도달했을때 (지금은 근거리 공격수 기준밖에 없음)
        int totalDistance = Math.Abs(currentPos.x - targetPos.x) + Math.Abs(currentPos.y - targetPos.y);
        if (Entity.transform.GetComponent<Character>().attackType == Character.Type.range)
        {
            if (totalDistance > 3)
                canMove = true;
            else
                Attack(Entity);
        }
        else
        {
            if (totalDistance > 1)
                canMove = true;
            else
                Attack(Entity);
        }
        Debug.Log(canMove);
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
                    Reset();
                    int cnt;
                    if (Entity.transform.GetComponent<Character>().currentState == Character.State.friend)
                    {
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

            Debug.Log(minDis);

            Entity.transform.GetComponent<Animator>().SetTrigger("Run");
            // 맵 현황을 바꾸고 해당 캐릭터를 이동
            map[currentPos.x, currentPos.y] = 0;
            currentPos = dirPos;
            if (Entity.transform.GetComponent<Character>().currentState == Character.State.friend)
                map[currentPos.x, currentPos.y] = 1;
            else
                map[currentPos.x, currentPos.y] = 2;

            int idx = currentPos.x * 10 + currentPos.y;
            Vector3 newPos = Field.GetChild(idx).transform.position;
            newPos.z = 1f;
            Entity.transform.GetComponent<Character>().pos = currentPos;
            Entity.transform.position = newPos;
            
            canMove = false;
        }
    }

    public void Attack(GameObject Entity)
    {
        Entity.transform.GetComponent<Animator>().SetTrigger("Attack");
    }
    
    public void InsertMapInfo()
    {
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < camp[i].childCount; j++)
            {
                entityOrder.Add(camp[i].GetChild(j).gameObject);
            }
        }

        // for (int i = 0; i < 5; i++)
        // {
        //     for (int j = 0; j < 10; j++)
        //     {
        //         Debug.Log(map[i, j]);
        //     }
        // }
        StartCoroutine(GameStart());
    }

    public IEnumerator GameStart()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                FindRoute(entityOrder[j]);
                yield return new WaitForSeconds(1f);
            }
        }
    }

    private int BFS(int startx, int starty, int endx, int endy, bool isBlue)
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
                    if (isBlue) // 블루가 레드 때렸을때
                    {
                        if (map[dirx, diry] != 1)
                        {
                            visited[dirx, diry] = true;
                            disMap[dirx, diry] += disMap[posx, posy] + 1;
                            queue.Enqueue((dirx, diry));
                        }
                    }
                    else
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

    private void Reset()
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
}
