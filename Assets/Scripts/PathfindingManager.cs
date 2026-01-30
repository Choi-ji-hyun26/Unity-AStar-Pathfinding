using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PathfindingManager : MonoBehaviour
{
    public static PathfindingManager Instance { get; private set; }
    
    [Header("Settings")]
    [SerializeField] private bool useSmoothing = true;
    public bool useSmoothingProperty => useSmoothing; // Unit 등에서 참조할 경우를 대비한 읽기 전용 프로퍼티
    [SerializeField] private bool showDebugVisuals = true;

    [Header("References")]
    [SerializeField] private Unit unit; // 캐릭터의 Unit 스크립트
    [SerializeField] private Transform target; // 목적지
    [SerializeField] private LineRenderer lineRenderer;
    public LineRenderer lineRendererProperty => lineRenderer; // Unit 등에서 참조할 경우를 대비한 읽기 전용 프로퍼티

    private AStarPathfinding pathFinding;
    private List<Node> finalPath;
    private Camera mainCamera;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        pathFinding = GetComponent<AStarPathfinding>();
        mainCamera = Camera.main;
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //  마우스 클릭 지점을 월드 좌표로 변환 (2D 기준)
            Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            //  타겟 위치를 마우스 클릭 지점으로 옮김
            target.position = mousePos;

            List<Node> path = pathFinding.FindPath(unit.transform.position, target.position);

            if(path != null && path.Count > 0)
            {
                finalPath = path;
                unit.OnPathFound(path);
                DrawPathLine();
            }
        }
    }

    public void RequestPath(Vector3 pathStart, Vector3 pathEnd, System.Action<List<Node>> callback)
    {
        List<Node> newPath = pathFinding.FindPath(pathStart, pathEnd);

        if(newPath != null)
            callback?.Invoke(newPath);
    }

    //  찾은 경로를 시각적으로 확인하기 위한 기즈모
    public void OnDrawGizmos()
    {
        // 디버그 시각화가 꺼져있거나 알고리즘 객체가 없으면 그리지 않음
        if (!showDebugVisuals || pathFinding == null) return;

        // 1. 원본 격자 경로, 회색 실선으로 표시 (지그재그 확인용)
        if (pathFinding.lastFullGridPath != null && pathFinding.lastFullGridPath.Count > 0)
        {
            Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.8f); // 약간 투명한 회색
            for (int i = 0; i < pathFinding.lastFullGridPath.Count - 1; i++)
            {
                // 노드 사이를 선으로 연결
                Gizmos.DrawLine(pathFinding.lastFullGridPath[i].worldPosition, pathFinding.lastFullGridPath[i + 1].worldPosition);
                // 각 노드 위치에 작은 구체 표시
                Gizmos.DrawWireSphere(pathFinding.lastFullGridPath[i].worldPosition, 0.15f);
            }
        }

        // 2. 최종 결과 경로, 하늘색/파란색 굵은 표시
        if (finalPath != null && finalPath.Count > 0)
        {
            // 스무딩 여부에 따라 색상 변경 (스무딩 중이면 파란색, 아니면 하늘색)
            Gizmos.color = useSmoothing ? Color.blue : Color.cyan;

            for (int i = 0; i < finalPath.Count - 1; i++)
            {
                Vector3 startPos = new Vector3(finalPath[i].worldPosition.x, finalPath[i].worldPosition.y, -0.5f);
                Vector3 endPos = new Vector3(finalPath[i+1].worldPosition.x, finalPath[i+1].worldPosition.y, -0.5f);
                
                // 경로 선 그리기
                Gizmos.DrawLine(startPos, endPos);
                // 경유지(Waypoint) 강조
                Gizmos.DrawSphere(startPos, 0.25f);
            }
            
            // 마지막 노드에도 구체 그리기
            Gizmos.DrawSphere(new Vector3(finalPath[finalPath.Count-1].worldPosition.x, finalPath[finalPath.Count-1].worldPosition.y, -0.5f), 0.25f);
        }
    }

    private void DrawPathLine()
    {
        if (finalPath == null || finalPath.Count == 0) return;

        lineRenderer.positionCount = finalPath.Count;
        for (int i = 0; i < finalPath.Count; i++)
        {
            // 캐릭터 발밑에 선이 그려지도록 Z축을 살짝 앞(-0.1f)으로 설정
            Vector3 pos = new Vector3(finalPath[i].worldPosition.x, finalPath[i].worldPosition.y, -0.1f);
            lineRenderer.SetPosition(i, pos);
        }
    }
}
