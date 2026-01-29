using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AStarPathfinding : MonoBehaviour
{
    Grid grid;
    public void Awake()
    {
        grid = GetComponent<Grid>();
    }

    // 디버깅을 위해 클래스 상단에 추가
    public List<Node> lastFullGridPath = new List<Node>();
    public List<Node> lastSmoothedPath = new List<Node>();

    public List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        // 모든 노드의 비용과 부모 정보를 초기화
        foreach (Node n in grid.grid)
        {
            n.gCost = 0;
            n.hCost = 0;
            n.parent = null;
        }

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        // 노드를 제대로 가져오지 못했다면 바로 종료
        if (startNode == null || targetNode == null) return null;

        // 시작 노드 설정
        startNode.gCost = 0;
        //nodesToReset.Add(startNode);

        MinHeap openList = new MinHeap();
        HashSet<Node> closedList = new HashSet<Node>();

        openList.Push(startNode);

        while (openList.Count > 0)
        {
            Node currentNode = openList.Pop();
            closedList.Add(currentNode);

            // 목적지 도착 시 경로 역추적 시작
            if (currentNode == targetNode)
            {
                // 1. 원본 격자 경로 저장
                lastFullGridPath = RetracePath(startNode, targetNode);
                
                // 2. 스위치에 따라 스무딩 적용
                if (PathfindingManager.Instance.useSmoothing)
                {
                    lastSmoothedPath = GetSimplifiedPath(lastFullGridPath);
                    return lastSmoothedPath;
                }
                else
                {
                    lastSmoothedPath = new List<Node>(); // 스무딩 미사용 시 비워둠
                    return lastFullGridPath;
                }
            }

            foreach (Node neighbor in grid.GetNeighbors(currentNode))
            {
                if (!neighbor.isWalkable || closedList.Contains(neighbor))
                    continue;
                // 새로운 경로를 통한 이웃 노드까지의 이동 비용 계산
                int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);

                // 더 짧은 경로를 발견한 경우 (최초 방문 시에 gCost가 무한대 -> 무조건 통과)
                bool isNewNode = !openList.Contains(neighbor);
                if (isNewNode || newMovementCostToNeighbor < neighbor.gCost)
                {
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode; // 경로 역추적을 위해 부모 기록

                    if (isNewNode)
                        openList.Push(neighbor);
                    else
                        openList.UpdateItem(neighbor); // 힙 내에서 위치 갱신
                }
            }
        }
        return null; // 경로를 찾지 못한 경우
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
        // 두 노드 사이의 방향과 거리 계산
        Vector3 dir = b.worldPosition - a.worldPosition;
        float dist = Vector3.Distance(a.worldPosition, b.worldPosition);

        // Raycast를 쏘아 wallMask(장애물)에 걸리는지 확인
        // 원형 캐릭터 부피를 고려해 CircleCast 쓰면 더 정확
        RaycastHit2D hit = Physics2D.CircleCast(a.worldPosition, (float)(grid.nodeRadius * 0.5), dir, dist, grid.wallMask);
        return hit.collider == null;
    }
    private List<Node> GetSimplifiedPath(List<Node> fullPath)
    {
        if(fullPath.Count < 3) return fullPath; // 노드가 2개 이하인 경우는 줄일 게 없음

        List<Node> simplifiedPath = new List<Node>();
        simplifiedPath.Add(fullPath[0]); // 시작점 추가

        int current = 0;
        while(current < fullPath.Count - 1)
        {
            bool foundNext = false; // 안전장치 변수
            // 현재 위치에서 가장 멀리 있는 노드부터 역순으로 시야 검사
            for(int i = fullPath.Count - 1; i > current; i--)
            {
                if(CheckLineOfSight(fullPath[current], fullPath[i]))
                {
                    simplifiedPath.Add(fullPath[i]);
                    current = i; // 시야가 확보된 가장 먼 곳으로 점프
                    foundNext = true;
                    break;
                }
            }

            // 만약 어떤 노드와도 시야가 확보되지 않는다면 (이론상 불가능하지만 에러 방지용)
            if (!foundNext)
            {
                current++;
                simplifiedPath.Add(fullPath[current]);
            }
        }
        return simplifiedPath;
    }

    // 두 노드 간의 거리 계산 (상하좌우 10, 대각선 14)
    private int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}
