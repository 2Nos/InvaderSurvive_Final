# InvaderSurvive_Final
바이브 코딩을 활용한 개발 도전

시작 : 2025.04 <br/>
상태 관리 : 각 메인상태들과 Flags를 활용한 복수개 상태 관리 <br/>

![image](https://github.com/user-attachments/assets/6fca33f9-d112-422c-8f5d-912bda2770c9)


![image](https://github.com/user-attachments/assets/6fa25375-89e7-4fba-b386-521f75e6e891) 
<br>

[카메라 하이어라키 구조]
Player
├── target_FollowCam (View 높이)

CameraRig (CameraRigManager, target_FollowCam와 글로벌 위치 동일하게)
├── Pivot(Empty, 어깨와의 거리)
│   └── MainCamera(실제 카메라, Pivot과의 거리 유지)

[카메라 회전]
- 평소엔 정면 유지
- 에임 시 살짝 Lerp 회전해서 에임 방향 응시
- 에임을 해제하면 천천히 원래 정면 방향으로 복귀
