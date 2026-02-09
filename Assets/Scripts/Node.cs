using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool isWalkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;

    public int gCost; // 시작점에서부터의 거리
    public int hCost; // 목적지까지의 예상 거리
    public Node parent; // 경로 역추적용

    public int fCost
    {
        get
        {
            if (gCost == int.MaxValue) return int.MaxValue;
            return gCost + hCost;
        }
    }

    private int heapIndex; // 인터페이스 구현을 위한 내부 변수

    public Node(bool isWalkable, Vector3 worldPos, int gridX, int gridY)
    {
        this.isWalkable = isWalkable;
        worldPosition = worldPos;
        this.gridX = gridX;
        this.gridY = gridY;

        // 아직 방문되지 않은 노드는 무한대 비용으로 초기화
        gCost = int.MaxValue;
    }

    // 힙 자료구조에서 사용되는 현재 노드의 인덱스
    public int HeapIndex
    {
        get => heapIndex;
        set => heapIndex = value;
    }

    // 힙 정렬을 위한 비교 로직
    public int CompareTo(Node nodeToCompare)
    {
        // 1. fCost 비교
        int compare = fCost.CompareTo(nodeToCompare.fCost);

        // 2. fCost가 같다면 hCost 비교
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }

        // MinHeap 구조에 맞게 결과를 반전
        return -compare;
    }
}
