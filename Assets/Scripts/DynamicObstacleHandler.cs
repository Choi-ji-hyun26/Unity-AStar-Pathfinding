using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DynamicObstacleHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap obstacleTilemap; // 장애물 전용 타일맵
    [SerializeField] private TileBase wallTile; // 설치할 벽 타일 에셋 

    private Camera mainCamera;

    private void Awake()
    {
        // Camera.main은 호출 시 내부적으로 검색하므로 캐싱해서 성능 최적화
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            ToggleWall();
        }
    }

    private void ToggleWall()
    {
        // 1. 마우스 위치를 월드 좌표로 변환
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        // 2. 타일맵 상의 좌표(Cell)로 변환
        Vector3Int cellPos = obstacleTilemap.WorldToCell(mouseWorldPos);
        Vector3 centerWorldPos = obstacleTilemap.GetCellCenterWorld(cellPos);
        // 3. 해당 칸에 이미 타일이 있는지 확인
        if (obstacleTilemap.HasTile(cellPos))
        {
            // 3-1. 벽이 있으면 제거
            obstacleTilemap.SetTile(cellPos, null);
            grid.UpdateNodeObstacle(centerWorldPos, true); // 데이터 갱신
            //Debug.Log("벽 제거");
        }
        else
        {
            // 3-2. 벽이 없으면 생성
            obstacleTilemap.SetTile(cellPos, wallTile);
            grid.UpdateNodeObstacle(centerWorldPos, false); // 데이터 갱신
            //Debug.Log("벽 생성");
        }

    }
}
