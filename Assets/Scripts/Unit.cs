using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public float speed = 5f;
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

    IEnumerator FollowPath()
    {
        if(path == null || path.Count == 0) yield break;

        Vector3 currentWayPoint = path[0].worldPosition;
        while (true)
        {
            // 목표 지점에 거의 도달했는지 확인
            if(transform.position == currentWayPoint)
            {
                targetIndex++;
                if(targetIndex >= path.Count){
                    PathfindingManager.Instance.lineRenderer.positionCount = 0;
                    yield break; // 목표 지점에 도착시 루프 종료
                }
                currentWayPoint = path[targetIndex].worldPosition;
            }
            // 부드러운 이동
            transform.position = Vector3.MoveTowards(transform.position, currentWayPoint, speed * Time.deltaTime);
            yield return null;
        }
    }
}
