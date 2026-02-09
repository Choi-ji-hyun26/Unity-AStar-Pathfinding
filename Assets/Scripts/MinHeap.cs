using System;
using System.Collections.Generic;

public class MinHeap<T> where T : IHeapItem<T>
{
    // List 대신 배열을 사용하여 메모리 할당 및 접근 성능을 최적화    
    private T[] items;
    private int currentItemCount;

    public MinHeap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }

    public void Push(T item)
    {
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;

        SortUp(item);
        currentItemCount++;
    }

    public T Pop()
    {
        T firstItem = items[0];

        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;

        SortDown(items[0]);
        return firstItem;
    }

    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    public int Count => currentItemCount;

    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item);
    }

    private void SortDown(T item)
    {
        while (true)
        {
            int leftChildIndex = item.HeapIndex * 2 + 1;
            int rightChildIndex = item.HeapIndex * 2 + 2;
            int swapIndex = 0;

            // 왼쪽 자식이 존재하지 않으면 더 이상 내려갈 수 없음
            if (leftChildIndex >= currentItemCount)
                return;

            swapIndex = leftChildIndex;

            // 오른쪽 자식이 존재하고 더 낮은 우선순위를 가진 경우 교체 대상 변경
            if (rightChildIndex < currentItemCount &&
                items[rightChildIndex].CompareTo(items[leftChildIndex]) > 0)
            {
                swapIndex = rightChildIndex;
            }

            // 현재 노드보다 자식 노드의 우선순위가 낮으면 교환
            if (item.CompareTo(items[swapIndex]) < 0)
            {
                Swap(item, items[swapIndex]);
            }
            else
            {
                return;
            }
        }
    }

    private void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;

        while (item.HeapIndex > 0)
        {
            T parentItem = items[parentIndex];

            // CompareTo 결과를 기준으로 우선순위 비교
            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
                parentIndex = (item.HeapIndex - 1) / 2;
            }
            else
            {
                break;
            }
        }
    }

    private void Swap(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;

        int tempIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = tempIndex;
    }
}