using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;
    private List<Node> path;
    private int targetIndex;

    [SerializeField] private Grid grid;

    public void OnPathFound(List<Node> newPath) // PathfindingManager에서 길을 찾았을 때 이 함수를 호출
    {
        if (newPath == null || newPath.Count == 0)
            return;

        path = newPath;
        targetIndex = 0;

        StopAllCoroutines();
        StartCoroutine(FollowPath());
    }

    private bool IsPathBlocked()
    {
        if (path == null || path.Count == 0)
            return false;

        // 현재 타겟 이후의 경로 중 장애물이 생겼는지 확인
        for (int i = targetIndex; i < path.Count; i++)
        {
            if (!path[i].isWalkable)
                return true; // 앞길이 막힘
        }
        return false;
    }

    private IEnumerator FollowPath()
    {

        if (path == null || path.Count == 0)
            yield break;

        Vector3 currentWayPoint = path[targetIndex].worldPosition;

        while (true)
        {
            Vector3 dir = (currentWayPoint - transform.position).normalized;
            float dist = Vector3.Distance(transform.position, currentWayPoint);

            // 물리 충돌 기반으로 경로 차단 여부 검사
            if (!path[targetIndex].isWalkable || Physics2D.CircleCast(transform.position, 0.2f, dir, dist, grid.wallMask))
            {
                PathfindingManager.Instance.RequestPath(
                    transform.position,
                    path[path.Count - 1].worldPosition,
                    OnPathFound
                );
                yield break;
            }


            // 현재 웨이포인트로 이동
            transform.position = Vector3.MoveTowards(transform.position, currentWayPoint, speed * Time.deltaTime);

            // 웨이포인트 도착 처리
            if (transform.position == currentWayPoint)
            {
                targetIndex++;
                // 목표 지점 도착
                if (targetIndex >= path.Count)
                {
                    PathfindingManager.Instance.lineRendererProperty.positionCount = 0;
                    yield break; // 목표 지점에 도착시 루프 종료
                }
                currentWayPoint = path[targetIndex].worldPosition;
                continue;
            }
            // 프레임 단위로 이동 처리
            yield return null;
        }
    }
}
