## **🚩 Real-time Dynamic A* Pathfinding Demo**

---
![dynamic_pathfinding2](https://github.com/user-attachments/assets/58a81099-7f89-4ec5-a758-9070acc3a1da)

**실시간 지형 변화에 대응하는 A* Pathfinding 기술 데모**

유닛이 이동 중 장애물을 즉시 인식하고 최적 경로를 재계산하는 동적 Pathfinding 시스템입니다.

---

### 💻 Tech Stack

---

**Engine / Language**: Unity 2021.3 LTS / C#

**Key Concepts** : A* Algorithm, Min-Heap Optimization, Tilemap-Grid Sychronization, Real-time Path Validation, Line of Sight Smoothing

---

### **⚙️ 핵심 시스템**

---

1. **Optimized A\* Search with Min-Heap**
- **설계 의도** : Unity C# 환경에서 기본 우선순위 큐 대신 GC 발생과 내부 동작을 직접 제어하기 위해 Generic Min-Heap 구현
- **성능 최적화** : Open List의 노드 탐색 비용을 O(n) → O(log n)으로 단축하여 안정적인 프레임 유지
- **효율성** : 배열 기반 구조와 Partial Reset 시스템을 활용해 방문한 노드만 초기화, 불필요한 연산 최소화

2. **Path Smoothing (Line-of-Sight)**
- **자연스러운 이동** : 격자 기반 경로의 지그재그 움직임을 **CircleCast 기반 직선 시야 검사**로 보정
- **결과** : 불필요한 경유지를 생략하고 유닛 부피를 고려한 부드러운 직선 경로 생성, 코너끼임 방지

3. **Dynamic Obstacle Handling**
- **실시간 대응** : 유닛 이동 중 경로 상 새로운 벽 발생 시 즉시 감지(`IsPathBlocked`)
- **자동 재탐색** : 장애물 발견 즉시 현재 위치에서 목적지까지 최적 경로를 재계산, 이동 중 경로 무효화 및 재요청 처리

4. **Pixel-Perfect Synchronization**
- **데이터 일치** : Tilemap.GetCellCenterWorld를 기준으로 월드 좌표와 내부 그리드 노드 좌표 1:1 동기화
- **정확도** : 유저 클릭 좌표와 논리적 그리드가 어긋나지 않도록 설계, 경계 조건에서도 안정적인 노드 선택

---

### 📺 Preview & Visuals

---

- **Red/White Nodes** : 장애물(Red)과 이동 가능 구역(White) 시각화
- **Blue Line** : Smoothing이 적용된 최종 이동 경로 (`LineRenderer`)
- **Grey Line** : 알고리즘이 계산한 원본 격자 경로 (`Gizmos`)
  
---
![dynamic_pathfinding](https://github.com/user-attachments/assets/bd863393-ee55-45eb-b77f-7bc1868a2eea)

<img width="720" height="336" alt="GizmosOn" src="https://github.com/user-attachments/assets/06ff9b0b-5fcb-4548-bb39-6bf0a1297868" />
<img width="703" height="347" alt="GizmosOff" src="https://github.com/user-attachments/assets/45f64249-eb18-4964-94d8-b2ed702f961a" />

