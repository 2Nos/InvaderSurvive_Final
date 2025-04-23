using UnityEngine;

public class InAirState : LocomotionBaseState
{
    public InAirState(PlayerLocomotion playerLocomotion) : base(playerLocomotion) { }
    public override LocomotionMainState DetermineStateType() => LocomotionMainState.InAir;

    private float m_GroundCheckTimer = 0f;

    public override void Enter()
    {
        base.Enter();
        //�ٸ� ������ �� ������
        m_PlayerLocomotion.m_IsProgress = true;
        m_GroundCheckTimer = 0;
    }

    public override void FixedUpdate()
    {
        m_GroundCheckTimer += Time.fixedDeltaTime;
        if (m_GroundCheckTimer >= 0.5f)
        {
            //�߷� ����ϰ� �����Ƿ� �ϴ��� �ּ�ó��
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
        //MoevY���� üũ ��
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
    /// ���� �� �߷� ó��
    /// </summary>
    public void UpdateGravityMovement()
    {
        // TODO : ���� �� �浹 ó��

        // 1. ������ �����̹Ƿ� �������� ����
        m_currentVelocityY += m_PlayerCore.m_Gravity * Time.deltaTime;
        //TODO : ���� �ӵ� ����
        m_currentVelocityY = Mathf.Clamp(m_currentVelocityY, -m_PlayerCore.m_MaxFallingSpeed, m_PlayerCore.m_jumpForce);

         // 4. �̵� ����
         Vector3 velocity = new Vector3(0, m_currentVelocityY, 0);

        m_PlayerCore.m_Rigidbody.MovePosition(m_PlayerCore.transform.position + velocity * Time.deltaTime);
    }

}