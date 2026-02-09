using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinding : MonoBehaviour
{
    private Grid grid;
    
    // [최적화] 탐색에 참여한 노드들만 기록하여 다음 탐색 시 초기화에 사용
    private List<Node> nodesToReset = new List<Node>();

    [Header("Debug Info")]
    public List<Node> lastFullGridPath = new List<Node>();
    public List<Node> lastSmoothedPath = new List<Node>();

    private void Awake()
    {
        grid = GetComponent<Grid>();
    }

    public List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        // 1. [최적화] 이전 탐색에서 변경된 노드들의 데이터만 정밀 초기화
        for (int i = 0; i < nodesToReset.Count; i++)
        {
            nodesToReset[i].gCost = int.MaxValue; // 무한대로 초기화
            nodesToReset[i].hCost = 0;
            nodesToReset[i].parent = null;
        }
        nodesToReset.Clear();

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (startNode == null || targetNode == null) return null;

        // 시작 노드 설정
        startNode.gCost = 0;
        nodesToReset.Add(startNode);

        // [리팩토링] Generic Heap 사용 (MaxSize는 Grid.cs에 public int MaxSize => gridSizeX * gridSizeY; 추가 필요)
        MinHeap<Node> openList = new MinHeap<Node>(grid.MaxSize);
        HashSet<Node> closedList = new HashSet<Node>();

        openList.Push(startNode);

        while (openList.Count > 0)
        {
            Node currentNode = openList.Pop();
            closedList.Add(currentNode);

            if (currentNode == targetNode)
            {
                // 목적지 도달 시 경로 역추적
                lastFullGridPath = RetracePath(startNode, targetNode);
                
                // Path Smoothing 적용 여부 확인
                if (PathfindingManager.Instance != null && PathfindingManager.Instance.useSmoothingProperty)
                {
                    lastSmoothedPath = GetSimplifiedPath(lastFullGridPath);
                    return lastSmoothedPath;
                }
                else
                {
                    lastSmoothedPath = new List<Node>();
                    return lastFullGridPath;
                }
            }

            foreach (Node neighbor in grid.GetNeighbors(currentNode))
            {
                if (!neighbor.isWalkable || closedList.Contains(neighbor))
                    continue;

                int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);

                // gCost 초기값이 int.MaxValue이므로 처음 방문 노드는 무조건 아래 조건문 통과
                if (newMovementCostToNeighbor < neighbor.gCost)
                {
                    bool isNewNode = neighbor.gCost == int.MaxValue;

                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (isNewNode)
                    {
                        openList.Push(neighbor);
                        nodesToReset.Add(neighbor); // 초기화 대상에 추가
                    }
                    else
                    {
                        openList.UpdateItem(neighbor); // 힙 위치 갱신 (O(log n))
                    }
                }
            }
        }
        return null;
    }

    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node curr = endNode;

        while (curr != startNode && curr != null)
        {
            path.Add(curr);
            curr = curr.parent;
        }

        path.Reverse();
        return path;
    }

    private bool CheckLineOfSight(Node a, Node b)
    {
        Vector3 dir = b.worldPosition - a.worldPosition;
        float dist = Vector3.Distance(a.worldPosition, b.worldPosition);

        // CircleCast를 통해 유닛의 반지름(nodeRadius * 0.5)을 고려한 직선 시야 검사
        RaycastHit2D hit = Physics2D.CircleCast(a.worldPosition, (float)(grid.nodeRadius * 0.4f), dir, dist, grid.wallMask);
        return hit.collider == null;
    }

    private List<Node> GetSimplifiedPath(List<Node> fullPath)
    {
        if (fullPath.Count < 3) return fullPath;

        List<Node> simplifiedPath = new List<Node>();
        simplifiedPath.Add(fullPath[0]);

        int current = 0;
        while (current < fullPath.Count - 1)
        {
            bool foundNext = false;
            // 가장 멀리 있는 노드부터 역순으로 탐색하여 직선으로 갈 수 있는 가장 먼 지점 탐색
            for (int i = fullPath.Count - 1; i > current; i--)
            {
                if (CheckLineOfSight(fullPath[current], fullPath[i]))
                {
                    simplifiedPath.Add(fullPath[i]);
                    current = i;
                    foundNext = true;
                    break;
                }
            }

            if (!foundNext)
            {
                current++;
                simplifiedPath.Add(fullPath[current]);
            }
        }
        return simplifiedPath;
    }

    private int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}