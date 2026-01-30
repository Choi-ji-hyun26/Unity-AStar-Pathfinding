using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;
    private List<Node> path;
    private int targetIndex;

    public void OnPathFound(List<Node> newPath) // PathfindingManager에서 길을 찾았을 때 이 함수를 호출
    {
        if(newPath == null || newPath.Count == 0)
            return;

        path = newPath;
        targetIndex = 0;
        StopAllCoroutines();
        StartCoroutine(FollowPath());
    }

    private bool IsPathBlocked()
    {
        if(path == null || path.Count == 0) return false;

        // 현재 타겟 인덱스부터 끝까지의 노드 중 하나라도 벽이 되었는지 확인
        for(int i = targetIndex; i < path.Count; i++)
        {
            if(!path[i].isWalkable){
                return true; // 앞길이 막힘
            }
        }
        return false;
    }
    private IEnumerator FollowPath()
    {
        
        if(path == null || path.Count == 0) yield break;

        Vector3 currentWayPoint = path[targetIndex].worldPosition;
        while (true)
        {
            // 1. 경로 차단 여부 체크, 실시간 벽 대응
            if (IsPathBlocked())
            {
                // 현재 내 위치에서 원래 목적지까지 다시 길찾기 요청
                // 마지막 노드가 목적지이므로 path[path.Count - 1] 사용
                PathfindingManager.Instance.RequestPath(transform.position, path[path.Count -1].worldPosition, OnPathFound);
                yield break;
            }

            // 2. 이동 처리
            transform.position = Vector3.MoveTowards(transform.position, currentWayPoint, speed * Time.deltaTime);
            
            if(transform.position == currentWayPoint)
            {
                targetIndex++;

                if(targetIndex >= path.Count){
                    PathfindingManager.Instance.lineRendererProperty.positionCount = 0;
                    yield break; // 목표 지점에 도착시 루프 종료
                }
                currentWayPoint = path[targetIndex].worldPosition;
                continue;
            }

            // 3. 마지막에 배치하여 캐릭터 이동이 완전 끝난 상태를 시네머신이 읽게 함
            yield return null;
        }
    }
}
