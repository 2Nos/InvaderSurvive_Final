//���ͽ��� �帣�� ��� ���� �̵��� ����
//Movment ����

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

        #region ======================================== State ����
        private LocomotionStrategyState m_currentStrategyState;
        public LocomotionStrategyState m_nextStrategyState { get; set; }
        public LocomotionStrategyState m_prevStrategyState { get; private set; }
        #endregion ======================================== /State ����

        // Move ����
        private Vector2 m_currentAniInput = Vector2.zero;
        public Vector3 m_CurrentVelocity;
        public float m_CurrentVelocityY;
        private float m_prevSpeed = 0;

        public bool m_IsGrounded { get; private set; }

        // PlayerCore Start���� ȣ��
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
        #region ======================================== State ����
        public void SetNextState(LocomotionMainState locomotionMainState)
        {
            m_nextStrategyState = m_StateUtility.m_MainStrategyMap[locomotionMainState];
        }
        private void UpdateSwitchState(LocomotionStrategyState newState)
        {
            if (m_nextStrategyState == null) return;

            m_currentStrategyState?.Exit();
            //m_prevStrategyState = m_currentStrategyState; //���� ���� ����
            m_currentStrategyState = newState;
            m_currentStrategyState?.Enter();

            Debug.Log($"Current State: {m_currentStrategyState}");
        }

        #endregion ======================================== /���� ��ȯ ����

        #region ======================================== Movement ����

        /// <summary>
        /// ���� üũ
        /// </summary>
        public void UpdateCheckGround()
        {
            Vector3 centerRay = m_playerCore.m_Rigidbody.position + Vector3.up * 0.1f;
            Vector3 forwardRay = m_playerCore.transform.position + m_playerCore.transform.forward * 0.25f + m_playerCore.transform.up * 0.3f;
            Vector3 rayDir = -m_playerCore.transform.up;

            Debug.DrawRay(centerRay, rayDir * 10f, Color.red, 0.1f);
            Debug.DrawRay(forwardRay, rayDir * 10f, Color.red, 0.1f);

            // ����� ���� ���� üũ
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

            //TODO : õ�� üũ

            //TODO : ��� �� �������� üũ
        }

        /// <summary>
        /// FixedUpdate���� ȣ��
        /// �̵�, ȸ��, �߷�, ������ �� ���¿��� ȣ��
        /// </summary>
        public void HandleMove()
        {
            // 1.�ʱ�ȭ
            Vector2 moveInput = m_playerCore.m_InputManager.m_MovementInput;    //�Է°�
            Vector3 m_moveDirection = m_playerCore.transform.right * moveInput.x + m_playerCore.transform.forward * moveInput.y; //����
            m_moveDirection.Normalize(); //����ȭ

            // //�ִϸ��̼� ���� Ʈ������ ���� ����(Input 0.7, 1 �̷������� �����⿡ ����� ���� ����)
            m_currentAniInput = Vector2.Lerp(m_currentAniInput, moveInput, Time.deltaTime * 10f);

            // 3. �̵� ���� ��
            // 3.1. �̵� �ӵ� ������ �� ���¿��� Set�� ����(���ǵ� ������ ���� �����)
            float currentSpeed = m_playerCore.m_CurrentSpeed;

            currentSpeed = Mathf.Lerp(m_prevSpeed, currentSpeed, Time.deltaTime * 10f); //���� ���ǵ忡�� ���� ���ǵ�� ����
            m_prevSpeed = currentSpeed;
            m_CurrentVelocity = m_moveDirection * currentSpeed;

            // 3.2 �̵��� ���� (�̷����� ������ �ܼ� ��ǥ���ٴ� ���������̿�)
            m_playerCore.SetRigidVelocity(m_CurrentVelocity);

            //�̵� �ִϸ��̼�
            m_playerCore.m_AnimationManager.UpdateMovementAnimation(m_currentAniInput);
        }
        public void InitializeVelocity()
        {
            m_CurrentVelocity = Vector2.zero;
            m_CurrentVelocityY = 0;
        }

        public void HandleRotation()
        {
            // ȭ�� ȸ�� ���� ��
            if (m_playerCore.m_InputManager.m_IsStopBodyRot) return;

            // ���� �÷��̾� ����
            Quaternion currentRotation = m_playerCore.m_Rigidbody.transform.rotation;

            // �⺻������ �ٶ� ������ �÷��̾��� ����
            Vector3 targetForward = m_playerCore.m_Rigidbody.transform.forward;

            // ���� ���� ���, ī�޶� ����(���� ��ġ)���� ȸ��

            if (m_playerCore.m_InputManager.m_IsAim)
            {
                //������ ���� ��ġ�� ������
                Vector3 aimPos = m_playerCore.m_CameraManager.UpdateAimTargetPos();
                aimPos.y = m_playerCore.transform.position.y; //ĳ���Ͱ� x,z��(��,�Ʒ���)�� ���ư��� �ʵ��� �����ϱ� ���� ���� �����ϰ� ����

                // ���� ��ġ���� ����
                Vector3 aimDirection = (aimPos - m_playerCore.transform.position).normalized;

                // ȸ���� �߻� ��
                if (aimDirection.sqrMagnitude > 0.01f)
                {
                    targetForward = aimDirection;
                }
            }
            else // �⺻ ����
            {
                float cameraYaw = m_playerCore.m_CameraManager.m_MouseX;
                Quaternion targetRot = Quaternion.Euler(0f, cameraYaw, 0f);
                targetForward = targetRot * Vector3.forward;
            }

            //���� �ٶ� ��������� �����̼� ��, Slerp�� ���� ����
            Quaternion targetRotation = Quaternion.LookRotation(targetForward);

            // �Ϲݻ��¿� ���� ���¿� ���� ȸ�� �ӵ� ����
            float rotationSpeed = m_playerCore.m_CurrentRotSpeed; //Aim ���¿��� ���ǵ� �ٷ��

            // ȸ�� �ӵ� ����
            //float maxDegrees = rotationSpeed * Time.deltaTime * m_playerCore.m_rotationDamping; //m_rotationDamping : ���� �ִ°�

            Quaternion smoothedRotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed);

            m_playerCore.m_Rigidbody.MoveRotation(smoothedRotation);
        }

        public void HandleGravityMovement()
        {
            if (m_IsGrounded) return;

            Debug.Log("Gravity");

            // �߷� ����
            Debug.Log(Physics.gravity);
            PlayerPhysicsUtility.ApplyGravity(m_playerCore.m_Rigidbody, Physics.gravity * m_playerCore.m_AddGravity);



            /*// TODO : ���� �� �浹 ó��
            // 1. �߷°� ���
            m_CurrentVelocityY += m_playerCore.m_AddGravity * Time.deltaTime;

            // 2. ���Ͻ� �ӵ� ����
            m_CurrentVelocityY = Mathf.Clamp(m_CurrentVelocityY, m_playerCore.m_MinVelocityY, m_playerCore.m_MaxVelocityY);

            // 3. �߷� ����
            m_playerCore.SetRigidVelocityY(m_CurrentVelocityY);*/
        }

        #endregion ======================================== /Move & Rotation

        #region ======================================== Animation ����

        #endregion ======================================== /Animation ȣ��
    }

}