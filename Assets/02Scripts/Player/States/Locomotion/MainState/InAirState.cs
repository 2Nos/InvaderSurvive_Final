using UnityEngine;

public class InAirState : LocomotionBaseState
{
    public InAirState(PlayerLocomotion playerLocomotion) : base(playerLocomotion) { }
    public override LocomotionMainState DetermineStateType() => LocomotionMainState.InAir;

    private float m_GroundCheckTimer = 0f;

    public override void Enter()
    {
        base.Enter();
        //다른 움직임 이 없도록
        m_PlayerLocomotion.m_IsProgress = true;
        m_GroundCheckTimer = 0;
    }

    public override void FixedUpdate()
    {
        m_GroundCheckTimer += Time.fixedDeltaTime;
        if (m_GroundCheckTimer >= 0.5f)
        {
            //중력 사용하고 있으므로 일단은 주석처리
            //UpdateGravityMovement();

            if (m_PlayerLocomotion.m_IsGrounded)
            {
                SetMainState(LocomotionMainState.Land);
            }
        }


    }

    public override void Update()
    {
        base.Update();
        //MoevY에서 체크 중
    }
    public override void Exit()
    {
        base.Exit();
        m_GroundCheckTimer = 0;
    }

    protected override void Movement()
    {
        m_PlayerLocomotion.HandleRotation();
    }

    float m_currentVelocityY;
    /// <summary>
    /// 점프 및 중력 처리
    /// </summary>
    public void UpdateGravityMovement()
    {
        // TODO : 위쪽 벽 충돌 처리

        // 1. 공중인 상태이므로 떨어지는 값만
        m_currentVelocityY += m_PlayerCore.m_Gravity * Time.deltaTime;
        //TODO : 낙하 속도 제한
        m_currentVelocityY = Mathf.Clamp(m_currentVelocityY, -m_PlayerCore.m_MaxFallingSpeed, m_PlayerCore.m_jumpForce);

         // 4. 이동 적용
         Vector3 velocity = new Vector3(0, m_currentVelocityY, 0);

        m_PlayerCore.m_Rigidbody.MovePosition(m_PlayerCore.transform.position + velocity * Time.deltaTime);
    }

}