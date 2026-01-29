using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool isWalkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;

    public int gCost; // 시작점에서부터의 거리
    public int hCost; // 목적지까지의 예상 거리
    public Node parent; // 경로 역추적용

    public int fCost => gCost + hCost; // 총 비용

    public int heapIndex; // 힙 내부 배열의 위치를 저장

    public Node(bool _isWalkable, Vector3 worldPos, int _gridX, int _gridY)
    {
        isWalkable = _isWalkable;
        worldPosition = worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }
}
