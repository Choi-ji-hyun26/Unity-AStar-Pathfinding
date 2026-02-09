using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [Header("Grid Configuration")]
    public LayerMask wallMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;

    [HideInInspector] public Node[,] grid;

    private float nodeDiameter;
    private int gridSizeX, gridSizeY;

    public int MaxSize => gridSizeX * gridSizeY; // 그리드 전체 노드 수

    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.FloorToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.FloorToInt(gridWorldSize.y / nodeDiameter);

        CreateGrid();
    }

    public void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];

        // transform.position을 기준으로 그리드의 왼쪽 하단 좌표 계산
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPosition = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                // 장애물(Wall) 레이어와 충돌하지 않는 경우 이동 가능
                bool walkable = !(Physics2D.OverlapCircle(worldPosition, (float)(nodeRadius * 0.5), wallMask));

                grid[x, y] = new Node(walkable, worldPosition, x, y);

                //grid[x,y].gCost = int.MaxValue;
            }
        }
    }
    // 이웃 노드를 가져오는 로직
    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue; // 자기 자신은 스킵

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                // 기본 맵 범위 체크 
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    bool side1Walkable = grid[node.gridX + x, node.gridY].isWalkable;
                    bool side2Walkable = grid[node.gridX, node.gridY + y].isWalkable;

                    // 대각선 이동일 때만 인접 벽 체크
                    if (x != 0 && y != 0)
                    {
                        // 대각선 경로를 가로 막는 두 직교 노드가 갈 수 있는지 확인
                        if (!side1Walkable || !side2Walkable)
                            continue;
                    }

                    // 일반적인 갈 수 있는 노드인지 체크 후 추가
                    if (grid[checkX, checkY].isWalkable)
                        neighbors.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbors;
    }
    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        // grid 배열이 아직 생성되지 않았다면 null 반환
        if (grid == null)
        {
            //Debug.LogWarning("Grid가 아직 생성되지 않았습니다!");
            return null;
        }

        // 0~1 사이의 비율로 변환
        float percentX = (worldPosition.x - transform.position.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.y - transform.position.y + gridWorldSize.y / 2) / gridWorldSize.y;

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        x = Mathf.Clamp(x, 0, gridSizeX - 1);
        y = Mathf.Clamp(y, 0, gridSizeY - 1);

        return grid[x, y];
    }

    // 그리드 상태가 변경되었을 때 알림을 전달하기 위한 이벤트
    public event System.Action OnGridChanged;

    public void UpdateNodeObstacle(Vector3 worldPos, bool isWalkable)
    {
        Node node = NodeFromWorldPoint(worldPos);
        if (node == null) return;

        node.isWalkable = isWalkable;
        OnGridChanged?.Invoke();
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 0.1f));

        // 실행 중이 아닐 때도 그리드를 강제로 생성해서 보여줌
        /*
        if (grid == null) 
        {
            // gridSizeX, Y 계산이 미리 되어야 하므로 호출
            nodeDiameter = nodeRadius * 2;
            gridSizeX = Mathf.FloorToInt(gridWorldSize.x / nodeDiameter);
            gridSizeY = Mathf.FloorToInt(gridWorldSize.y / nodeDiameter);
            CreateGrid(); 
        }
        */
        if (grid != null)
        {
            foreach (Node node in grid)
            {
                Gizmos.color = (node.isWalkable) ? Color.white : Color.red;
                Gizmos.DrawCube(node.worldPosition, Vector3.one * (nodeDiameter - .1f));
            }
        }
    }
}
