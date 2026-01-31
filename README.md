## **🚩 Real-time Dynamic A* Pathfinding Demo**

---
![dynamic_pathfinding](https://github.com/user-attachments/assets/7187ceb0-5f84-4c83-b161-af5e557c946d)

**실시간 지형 변화에 즉각적으로 대응하는 최적화된 A* Pathfinding 기술 데모**

본 프로젝트는 Untiy Tilemap과 Grid 데이터를 실시간으로 동기화하여 런타임 중 장애물의 생성 및 제거에 따른 유닛이 경로를 즉각 재탐색하도록 설계된 동적 Pathfinding 시스템입니다.

본 시스템은 RTS, 타워 디펜스, 로그라이크 등 실시간 맵 변경이 빈번한 장르에 바로 적용 가능한 구조로 설계되었습니다.

---

### 💻 Tech Stack

---

**Engine / Language**: Unity 2021.3 LTS / C#

**Key Concepts** : A* Algorithm, Min-Heap Optimization, Tilemap-Grid Sychronization, Real-time Path Validation, Line of Sight Smoothing

---

### **⚙️ 핵심 시스템**

---

1. **Optimized A* Search with Min-Heap**
- **설계 의도** : Unity C# 환경에서 기본 우선순위 큐 라이브러리를 사용하지 않고 GC 발생과 내부 동작을 직접 제어하기 위해 Min-Heap을 직접 구현
- **성능 최적화** : Open List의 노드 탐색 비용을 O(n)에서 O(log n)으로 단축하기 위해 직접 **Min-Heap**을 구현
- **효율성** : 대규모 그리드 맵에서도 프레임 드랍 없이 최단 경로를 산출

---

1. **Path Smoothing (Line-of-Sight)**
- **자연스러운 이동** : 격자 기반 경로의 단점인 지그재그 움직임을 해결하기 위해 **CircleCast**를 활용한 시야 검사 로직 적용
- **결과** : 불필요한 경유지를 생략하고 실제 유닛 이동에 적합한 직선 위주의 부드러운 경로를 생성

---

1. **Dynamic Obstacle Handling**
- **실시간 대응** : 유닛이 이동 중일 때 경로 상에 새로운 벽이 생기면 이를 즉시 감지(`IsPathBlocked`)
- **자동 재탐색** : 장애물을 인지한 즉시 현재 위치에서 목적지까지의 새로운 최적 경로를 재계산

---

1. **Pixel-Perfect Synchronization**
- **데이터 일치** : Tilemap.GetCellCenterWorld를 활용하여 유니티 타일맵과 내부 그리드 노드 좌표를 1:1로 정밀 동기화
- **정확도** : 사용자가 클릭한 타일의 시각적 위치와 논리적 데이터가 어긋나지 않도록 설계

---

### **🔧** Troubleshooting : 좌표 불일치 및 데이터 잔상 문제

---

**문제점**

- 마우스 클릭으로 벽을 생성/제거 시 특정 위치에서 노드가 빨갛게 변하지 않거나 지워지지 않는 데이터 불일치 발생
- 부동소수점 오차로 인해 클릭한 타일 옆 칸의 노드가 업데이트되는 현상 발견

**해결책**

- 중심점 기반 좌표계 : 마우스 클릭 좌표를 그대로 쓰지 않고 타일맵의 셀 중앙 좌표(`GetCellCenterWorld`)를 추출하여 그리드 시스템에 전달
- **데이터 강제 동기화** : `ToggleWall` 로직 내에서 타일의 존재 여부를 먼저 검사한 후 그리드 노드의 `isWalkable` 상태를 타일맵 상태에 맞춰 강제로 덮어쓰도록 구현하여 예외 케이스 제거

---

### 📺 Preview & Visuals

- **Red/White Nodes** : 장애물(Red)과 이동 가능 구역(White) 시각화
- **Blue Line** : 스무딩이 적용된 최종 이동 경로 (`LineRenderer`)
- **Grey Line** : 알고리즘이 계산한 원본 격자 경로 (`Gizmos`)
  
---
<img width="720" height="336" alt="GizmosOn" src="https://github.com/user-attachments/assets/06ff9b0b-5fcb-4548-bb39-6bf0a1297868" />
<img width="703" height="347" alt="GizmosOff" src="https://github.com/user-attachments/assets/45f64249-eb18-4964-94d8-b2ed702f961a" />

