using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinHeap
{
    List<Node> items = new List<Node>(); // 데이터를 저장할 리스트

    public void Push(Node item)
    {
        item.heapIndex = items.Count;
        items.Add(item);
        SiftUp(item); // 위로 올리며 정렬
    }

    public Node Pop()
    {
        if (Count == 0)
            return null;

        Node firstItem = items[0]; // 최솟값(우선순위 가장 높음) 저장
        int lastIndex = items.Count - 1;


        items[0] = items[lastIndex]; // 마지막 아이템을 루트로 이동
        items[0].heapIndex = 0;
        items.RemoveAt(lastIndex); // 리스트 마지막 요소 제거

        if (items.Count > 0)
            SiftDown(items[0]); // 아래로 내리며 정렬  

        return firstItem;
    }

    public void UpdateItem(Node item)
    {
        SiftUp(item); // A*에서는 보통 비용이 줄어들기만 하므로 위로만 올리면 됨
    }

    public bool Contains(Node item)
    {
        if (item.heapIndex < items.Count)
            return items[item.heapIndex] == item;
        return false;
    }

    public void SiftUp(Node item)
    {
        int parentIndex = (item.heapIndex - 1) / 2;

        while (item.heapIndex > 0)
        {
            Node parentItem = items[parentIndex];
            if (item.fCost < parentItem.fCost || (item.fCost == parentItem.fCost && item.hCost < parentItem.hCost))
            {
                Swap(item, parentItem);
            }
            else
            {
                break;
            }

            parentIndex = (item.heapIndex - 1) / 2;
        }
    }
    public void SiftDown(Node item)
    {
        while (true)
        {
            int childLeftIndex = item.heapIndex * 2 + 1;
            int childRightIndex = item.heapIndex * 2 + 2;
            int swapIndex = 0;

            if (childLeftIndex < items.Count)
            {
                swapIndex = childLeftIndex;

                if (childRightIndex < items.Count)
                {
                    if (items[childRightIndex].fCost < items[childLeftIndex].fCost ||
                        (items[childRightIndex].fCost == items[childLeftIndex].fCost && items[childRightIndex].hCost < items[childLeftIndex].hCost))
                    {
                        swapIndex = childRightIndex;
                    }
                }

                if (item.fCost > items[swapIndex].fCost ||
                    (item.fCost == items[swapIndex].fCost && item.hCost > items[swapIndex].hCost))
                {
                    Swap(item, items[swapIndex]);
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }

    private void Swap(Node itemA, Node itemB)
    {
        items[itemA.heapIndex] = itemB;
        items[itemB.heapIndex] = itemA;

        int tempIndex = itemA.heapIndex;
        itemA.heapIndex = itemB.heapIndex;
        itemB.heapIndex = tempIndex;
    }

    public int Count => items.Count;
}