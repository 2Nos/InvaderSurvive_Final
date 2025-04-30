//루터슈터 장르의 경우 물리 이동이 유리
//Movment 관련

using UnityEngine;

namespace DUS.Player.Locomotion {
    public class PlayerLocomotion
    {
        private PlayerCore m_playerCore;
        public PlayerLocomotion(PlayerCore playerCore)
        {
            m_playerCore = playerCore;
        }

        public LocomotionStateUtility m_StateUtility;

        #region ======================================== State 관리
        private LocomotionStrategyState m_currentStrategyState;
        public LocomotionStrategyState m_nextStrategyState { get; set; }
        public LocomotionStrategyState m_prevStrategyState { get; private set; }
        #endregion ======================================== /State 관리

        // Move 설정
        private Vector2 m_currentAniInput = Vector2.zero;
        public Vector3 m_CurrentVelocity;
        public float m_CurrentVelocityY;
        private float m_prevSpeed = 0;

        public bool m_IsGrounded { get; private set; }

        // PlayerCore Start에서 호출
        public void InitializeLocomotionAtStart()
        {
            m_StateUtility = new LocomotionStateUtility();
            m_StateUtility.InitializeCreateMainStateMap(m_playerCore);
            m_currentStrategyState = m_StateUtility.m_MainStrategyMap[LocomotionMainState.Idle];
            Debug.Log(m_currentStrategyState);
            m_currentStrategyState.Enter();
            //m_StateUtility.SetMainStateAnimation();
        }

        public void FixedUpdate()
        {
            m_currentStrategyState?.FixedUpdate();
            UpdateCheckGround();
            HandleGravityMovement();
        }

        public void Update()
        {
            m_currentStrategyState?.Update();
            if (m_currentStrategyState != m_nextStrategyState)
                UpdateSwitchState(m_nextStrategyState);
        }

        public void LateUpdate()
        {
            
        }
        #region ======================================== State 관리
        public void SetNextState(LocomotionMainState locomotionMainState)
        {
            m_nextStrategyState = m_StateUtility.m_MainStrategyMap[locomotionMainState];
        }
        private void UpdateSwitchState(LocomotionStrategyState newState)
        {
            if (m_nextStrategyState == null) return;

            m_currentStrategyState?.Exit();
            //m_prevStrategyState = m_currentStrategyState; //이전 상태 저장
            m_currentStrategyState = newState;
            m_currentStrategyState?.Enter();

            Debug.Log($"Current State: {m_currentStrategyState}");
        }

        #endregion ======================================== /상태 전환 관리

        #region ======================================== Movement 관리

        /// <summary>
        /// 지면 체크
        /// </summary>
        public void UpdateCheckGround()
        {
            Vector3 centerRay = m_playerCore.m_Rigidbody.position + Vector3.up * 0.1f;
            Vector3 forwardRay = m_playerCore.transform.position + m_playerCore.transform.forward * 0.25f + m_playerCore.transform.up * 0.3f;
            Vector3 rayDir = -m_playerCore.transform.up;

            Debug.DrawRay(centerRay, rayDir * 10f, Color.red, 0.1f);
            Debug.DrawRay(forwardRay, rayDir * 10f, Color.red, 0.1f);

            // 현재는 평평 땅만 체크
            RaycastHit hit;
            if (Physics.Raycast(centerRay, rayDir, out hit, 10f, m_playerCore.m_GroundMask))
            {
                if (Mathf.Floor(hit.distance * 100) <= 12f)
                {
                    m_IsGrounded = true;
                }
                else if (Mathf.Floor(hit.distance * 100) > 13f)
                {
                    m_IsGrounded = false;
                }
            }

            //TODO : 천장 체크

            //TODO : 계단 및 내리막길 체크
        }

        /// <summary>
        /// FixedUpdate에서 호출
        /// 이동, 회전, 중력, 점프는 각 상태에서 호출
        /// </summary>
        public void HandleMove()
        {
            // 1.초기화
            Vector2 moveInput = m_playerCore.m_InputManager.m_MovementInput;    //입력값
            Vector3 m_moveDirection = m_playerCore.transform.right * moveInput.x + m_playerCore.transform.forward * moveInput.y; //방향
            m_moveDirection.Normalize(); //정규화

            // //애니메이션 블렌드 트리동작 연결 보간(Input 0.7, 1 이런식으로 들어오기에 끊기는 동작 개선)
            m_currentAniInput = Vector2.Lerp(m_currentAniInput, moveInput, Time.deltaTime * 10f);

            // 3. 이동 중일 때
            // 3.1. 이동 속도 결정은 각 상태에서 Set로 설정(스피드 종류가 많은 관계로)
            float currentSpeed = m_playerCore.m_CurrentSpeed;

            currentSpeed = Mathf.Lerp(m_prevSpeed, currentSpeed, Time.deltaTime * 10f); //이전 스피드에서 현재 스피드로 보간
            m_prevSpeed = currentSpeed;
            m_CurrentVelocity = m_moveDirection * currentSpeed;

            // 3.2 이동값 적용 (이런류의 게임은 단순 좌표보다는 물리엔진이용)
            m_playerCore.SetRigidVelocity(m_CurrentVelocity);

            //이동 애니메이션
            m_playerCore.m_AnimationManager.UpdateMovementAnimation(m_currentAniInput);
        }
        public void InitializeVelocity()
        {
            m_CurrentVelocity = Vector2.zero;
            m_CurrentVelocityY = 0;
        }

        public void HandleRotation()
        {
            // 화면 회전 중지 시
            if (m_playerCore.m_InputManager.m_IsStopBodyRot) return;

            // 현재 플레이어 방향
            Quaternion currentRotation = m_playerCore.m_Rigidbody.transform.rotation;

            // 기본적으로 바라볼 방향은 플레이어의 전방
            Vector3 targetForward = m_playerCore.m_Rigidbody.transform.forward;

            // 에임 중일 경우, 카메라 방향(에임 위치)으로 회전

            if (m_playerCore.m_InputManager.m_IsAim)
            {
                //현재의 에임 위치를 가져옴
                Vector3 aimPos = m_playerCore.m_CameraManager.UpdateAimTargetPos();
                aimPos.y = m_playerCore.transform.position.y; //캐릭터가 x,z축(위,아래로)은 돌아가지 않도록 설정하기 위해 높이 동일하게 유지

                // 조준 위치로의 방향
                Vector3 aimDirection = (aimPos - m_playerCore.transform.position).normalized;

                // 회전힘 발생 시
                if (aimDirection.sqrMagnitude > 0.01f)
                {
                    targetForward = aimDirection;
                }
            }
            else // 기본 상태
            {
                float cameraYaw = m_playerCore.m_CameraManager.m_MouseX;
                Quaternion targetRot = Quaternion.Euler(0f, cameraYaw, 0f);
                targetForward = targetRot * Vector3.forward;
            }

            //현재 바라볼 대상으로의 로테이션 값, Slerp를 위해 저장
            Quaternion targetRotation = Quaternion.LookRotation(targetForward);

            // 일반상태와 에임 상태에 따라 회전 속도 조절
            float rotationSpeed = m_playerCore.m_CurrentRotSpeed; //Aim 상태에서 스피드 다루기

            // 회전 속도 조절
            //float maxDegrees = rotationSpeed * Time.deltaTime * m_playerCore.m_rotationDamping; //m_rotationDamping : 감속 주는것

            Quaternion smoothedRotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed);

            m_playerCore.m_Rigidbody.MoveRotation(smoothedRotation);
        }

        public void HandleGravityMovement()
        {
            if (m_IsGrounded) return;

            Debug.Log("Gravity");

            // 중력 적용
            Debug.Log(Physics.gravity);
            PlayerPhysicsUtility.ApplyGravity(m_playerCore.m_Rigidbody, Physics.gravity * m_playerCore.m_AddGravity);



            /*// TODO : 위쪽 벽 충돌 처리
            // 1. 중력값 계산
            m_CurrentVelocityY += m_playerCore.m_AddGravity * Time.deltaTime;

            // 2. 낙하시 속도 제한
            m_CurrentVelocityY = Mathf.Clamp(m_CurrentVelocityY, m_playerCore.m_MinVelocityY, m_playerCore.m_MaxVelocityY);

            // 3. 중력 적용
            m_playerCore.SetRigidVelocityY(m_CurrentVelocityY);*/
        }

        #endregion ======================================== /Move & Rotation

        #region ======================================== Animation 관리

        #endregion ======================================== /Animation 호출
    }

}