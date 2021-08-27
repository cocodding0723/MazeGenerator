using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public struct DirectionInfo
    {
        public Vector2Int point;
        public Direction currentDirection;
        public DirectionInfo(Vector2Int point, Direction current)
        {
            this.point = point;
            this.currentDirection = current;
        }

        public Vector2Int GetNextPoint()
        {
            Vector2Int plusValue = Vector2Int.zero;

            switch (currentDirection)
            {
                case Direction.Up:
                    plusValue = new Vector2Int(0, 1);
                    break;
                case Direction.Down:
                    plusValue = new Vector2Int(0, -1);
                    break;
                case Direction.Left:
                    plusValue = new Vector2Int(-1, 0);
                    break;
                case Direction.Right:
                    plusValue = new Vector2Int(1, 0);
                    break;
            }

            return point + plusValue;
        }
    }

    public enum Direction
    {
        Up = 0, Down = 1, Right = 2, Left = 3, Count
    }

    private int[,] maze;
    private bool[,] visit;
    private List<DirectionInfo> directionInfos = new List<DirectionInfo>();

    private void Start()
    {
        int[,] maze = GenerateMaze(new Vector2Int(1, 0), new Vector2Int(24, 24), Vector2Int.one * 25);

        for (int i = 0; i < 25; i++)
        {
            for (int j = 0; j < 25; j++)
            {
                if (maze[i, j] > 0)
                {
                    GameObject gobj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    gobj.transform.position = new Vector2(i, j);
                }
            }
        }
    }

    private int[,] GenerateMaze(Vector2Int start, Vector2Int end, Vector2Int size)
    {
        maze = new int[size.y, size.x];
        visit = new bool[size.y, size.x];

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                if (x == 0 || y == 0 || x == size.x - 1 || y == size.y - 1)
                {
                    maze[y, x] = 1;
                    visit[y, x] = true;

                    AddInfos(directionInfos, new Vector2Int(x, y));
                }
            }
        }

        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                Vector2Int plusValue = new Vector2Int(i, j);
                Vector2Int startArround = start + plusValue;
                Vector2Int endArround = end + plusValue;

                if (CheckSizeOver(startArround, size))
                {
                    Debug.Log(startArround);
                    if (maze[startArround.y, startArround.x] == 1)
                    {
                        maze[startArround.y, startArround.x] = 2;
                        visit[startArround.y, startArround.x] = true;
                    }
                }
                if (CheckSizeOver(endArround, size))
                {
                    if (maze[endArround.y, endArround.x] == 1)
                    {
                        maze[endArround.y, endArround.x] = 2;
                        visit[endArround.y, endArround.x] = true;
                    }
                }
            }
        }

        while (directionInfos.Count != 0)
        {
            int randomIndex = Random.Range(0, directionInfos.Count);
            DirectionInfo randomInfo = directionInfos[randomIndex];
            Vector2Int nextPoint = randomInfo.GetNextPoint();

            if (CheckSizeOver(nextPoint, size))
            {
                Debug.Log(nextPoint);
                if (!visit[nextPoint.y, nextPoint.x])
                {
                    if (CheckPossible(randomInfo, maze))
                    {
                        maze[nextPoint.y, nextPoint.x] = 1;
                        AddInfos(directionInfos, nextPoint);
                    }
                    visit[nextPoint.y, nextPoint.x] = true;
                }
            }

            directionInfos.Remove(randomInfo);
        }

        return maze;
    }

    private bool CheckPossible(DirectionInfo info, int[,] maze)
    {
        bool result = true;

        for (int y = -1; y < 2; y++)
        {
            for (int x = -1; x < 2; x++)
            {
                Vector2Int checkPosition = info.GetNextPoint() + new Vector2Int(x, y);
                
                if (!visit[checkPosition.y, checkPosition.x]){
                    if (CheckSizeOver(checkPosition, Vector2Int.one * maze.GetLength(0)))
                    {
                        if (checkPosition == info.point)
                        {
                            continue;
                        }
                        else
                        {
                            if (maze[checkPosition.y, checkPosition.x] > 0)
                            {
                                result = false;
                            }
                        }
                    }
                }
            }
        }


        return result;
    }

    private Vector2Int DirectionToVector2(Direction direction)
    {
        Vector2Int result = Vector2Int.zero;
        switch (direction)
        {
            case Direction.Down:
                result = new Vector2Int(0, -1);
                break;
            case Direction.Up:
                result = new Vector2Int(0, 1);
                break;
            case Direction.Right:
                result = new Vector2Int(1, 0);
                break;
            case Direction.Left:
                result = new Vector2Int(-1, 0);
                break;
        }

        return result;
    }

    private bool CheckSizeOver(Vector2Int point, Vector2Int size)
    {
        if (point.x >= size.x || point.x < 0) return false;
        if (point.y >= size.y || point.y < 0) return false;

        return true;
    }

    private void AddInfos(List<DirectionInfo> list, Vector2Int point)
    {
        for (int i = 0; i < (int)Direction.Count; i++)
        {
            list.Add(new DirectionInfo(point, (Direction)i));
        }
    }
}
