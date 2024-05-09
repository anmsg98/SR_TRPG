using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private int[,] map = new int[5, 10];
    private bool[,] visited = new bool[5, 10];
    private int[,] disMap = new int[5, 10];

    private int[] dx = new int[4] {-1, 1, 0, 0};
    private int[] dy = new int[4] {0, 0, -1, 1};
    
    private int BFS(int startx, int starty, int endx, int endy)
    {
        Reset();
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
                cnt = -1;
            }

            //상하좌우 체크
            for (int i = 0; i < 4; i++)
            {
                int dirx = pos.Item1 + dx[i];
                int diry = pos.Item2 + dy[i];
                if ((dirx >= 0 && dirx < 5) && (diry >= 0 && diry < 10)
                                            && visited[dirx, diry] == false && map[dirx, diry] == 0)
                {
                    visited[dirx, diry] = true;
                    disMap[dirx, diry] += disMap[posx, posy] + 1;
                    queue.Enqueue((dirx, diry));
                }

            }
        }

        return cnt;
    }

    void Start()
    {
        Debug.Log(BFS(0, 0, 1, 9));
    }

    void Reset()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                map[i, j] = 0;
                visited[i, j] = false;
                disMap[i, j] = 0;
            }
        }
    }
}
